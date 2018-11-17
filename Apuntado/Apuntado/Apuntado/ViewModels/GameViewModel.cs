namespace Apuntado.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System;
    using SQLite;
    using System.Linq;
    using System.Windows.Input;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using Xamarin.Forms;

    public class GameViewModel : BaseViewModel
    {
        #region Attributes
        private ObservableCollection<GameItemsViewModel> players;
        private Games gameSelected;
        private bool isVisibleUsr;
        private string playerName;
        private string msggError;
        private Sqlite sqlCon;
        private int ponits_A;
        private bool isEnable_P;
        private double isLose;
        private List<GameItemsViewModel> selectd;
        private object isSelectD;
        #endregion

        #region Properties
        public ObservableCollection<GameItemsViewModel> Players
        {
            get { return this.players; }
            set { SetValue(ref this.players, value); }
        }
        public Games GameSelected
        {
            get
            {
                return this.gameSelected;
            }
            set
            {
                this.gameSelected = value;
            }
        }
        public string PlayerName
        {
            get;
            set;
        }
        public string MsggError
        {
            get { return this.msggError; }
            set { SetValue(ref this.msggError, value); }
        }
        public bool IsVisibleUsr
        {
            get { return this.isVisibleUsr; }
            set { SetValue(ref this.isVisibleUsr, value); }

        }
        public int Ponits_A
        {
            get { return this.ponits_A; }
            set { SetValue(ref this.ponits_A, value); }
        }
        public bool IsEnable_P
        {
            get { return this.isEnable_P; }
            set { SetValue(ref this.isEnable_P, value); }
        }
        public double IsLose
        {
            get { return this.isLose; }
            set { SetValue(ref this.isLose, value); }
        }
        public object IsSelectD
        {
            get { return this.isSelectD; }
            set { SetValue(ref this.isSelectD, value); }
        }
        #endregion

        #region Constructor
        public GameViewModel(Games game)
        {
            this.GameSelected = game;
            this.IsVisibleUsr = false;
            this.sqlCon = MainViewModel.GetInstance().Games.sqlcon;

            // Load player this game
            this.LoadPLayers();
        }
        #endregion

        #region Command
        public ICommand NewPLayerCommand
        {
            get
            {
                return new RelayCommand(NewPlayerPop_T);
            }

        }

        public ICommand DeletePLayerCommand
        {
            get
            {
                return new RelayCommand(DeletePLayer);
            }

        }        

        public ICommand CreatePlayer
        {
            get
            {
                return new RelayCommand(NewPLayer);
            }
        }

        public ICommand CancelPlayer
        {
            get
            {
                return new RelayCommand(NewPlayerPop_F);
            }
        }
        #endregion

        #region Methods
        private void NewPlayerPop_T()
        {
            this.IsVisibleUsr = true;
        }

        private void NewPlayerPop_F()
        {
            this.IsVisibleUsr = false;
        }

        private async void LoadPLayers()
        {
            var mainModel = MainViewModel.GetInstance();
            // get complete table
            //var response = await this.sqlCon.GetAllTable<Players>();
            var strSQL = "SELECT * FROM Players WHERE IdGame = '" + this.GameSelected.IdGame +"'";
            var response = await this.sqlCon.GetQuery<Players>(strSQL);

            if (response.IsSuccess)
            {
                mainModel.PlayerList = (List<Players>)response.Result;
                this.Players = new ObservableCollection<GameItemsViewModel>(this.ToPlayersItemsModel());
            }
            
        }       

        private async void NewPLayer()
        {

            MainViewModel mainViewModel = MainViewModel.GetInstance();            
            // Same Name
            if (string.IsNullOrEmpty(this.PlayerName))
            {
                this.MsggError = "Se debe ingresar el juegador a crear";
                return;
            }


            if (mainViewModel.PlayerList != null)
            {
                var player_in = mainViewModel.PlayerList.Where(
                                                    p => p.Namep.ToUpper() == this.PlayerName.ToUpper()).
                                                    FirstOrDefault();

                if (player_in != null)
                {
                    this.MsggError = "Ya existe el jugador ingresado";
                    return;
                }
            }
            

            // Create player in BD
            var objPlayer = new Players
            {
                IdGame = this.GameSelected.IdGame,
                Namep = this.PlayerName.ToUpper()
            };

            var response = await this.sqlCon.AddNewReg(objPlayer);

            if (!response.IsSuccess)
            {
                this.MsggError = response.Message;
                return;
            }
            else
            {
                mainViewModel.PlayerList.Add(objPlayer);

                var ItemPlayer = new GameItemsViewModel(this.GameSelected)
                {
                    IdGame = this.GameSelected.IdGame,
                    Namep = this.PlayerName.ToUpper()
                };
                this.Players.Add(ItemPlayer);

                this.PlayerName = string.Empty;                
                this.MsggError = string.Empty;
                this.IsVisibleUsr = false;

            }
        }

        private async void DeletePLayer()
        {
            // get player selected

            var player_d = (GameItemsViewModel)this.IsSelectD;                

            if (player_d == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                            "Jugador no seleccionado",
                            "No se selecciono ningún jugador para eliminar",
                            "Aceptar");
                return;
            }             
    
            var action = await Application.Current.MainPage.DisplayAlert(
                            "Eliminar jugador",
                            "Desea eliminar el jugador " + player_d.Namep + " Seleccionado",
                            "Si",
                            "Cancelar");

            if (!action)
            {
                return;
            }

            // Delete player in BD
            var mainViewModel = MainViewModel.GetInstance();
            var sqlcon = mainViewModel.Games.sqlcon;

            var player_delete = new Players
            {
                IdGame = player_d.IdGame,
                IdPLayer = player_d.IdPLayer
            };

            var response = await sqlCon.DeleteReg(player_delete);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                            "Error",
                            response.Message,
                            "Aceptar");
            }
            else
            {
                mainViewModel.PlayerList.Remove(player_delete);
                mainViewModel.Game.Players.Remove(player_d);
            }


        }

        private IEnumerable<GameItemsViewModel> ToPlayersItemsModel()
        {
            // Transformar la lista Games a GamesItemsModel que hereda Games del Model
            return MainViewModel.GetInstance().PlayerList.Select(g => new GameItemsViewModel(this.GameSelected)
            {                
                IdGame = g.IdGame,
                IdPLayer = g.IdPLayer,
                Namep  = g.Namep,
                Points = g.Points
            });
        }
        #endregion
    }
}
