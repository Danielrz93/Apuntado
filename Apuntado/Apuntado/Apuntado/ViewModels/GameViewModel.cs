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

    public class GameViewModel : BaseViewModel
    {
        #region Attributes
        private ObservableCollection<GameItemsViewModel> players;
        private Games gameSelected;
        private bool isVisibleUsr;
        private string playerName;
        private string msggError;
        private Sqlite sqlCon;
        #endregion

        #region Properties
        public ObservableCollection<GameItemsViewModel> Players
        {
            get { return this.players;  }
            set { SetValue(ref this.players, value); }
        }
        public Games GameSelected
        {
            get  
            {
                return this.gameSelected;
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
            set { SetValue(ref this.isVisibleUsr, value);  }
            
        }
        #endregion

        #region Constructor
        public GameViewModel(Games game)
        {
            this.gameSelected = game;
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
                var player_in = mainViewModel.PlayerList.Where(p => p.Namep == this.PlayerName).FirstOrDefault();

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
                var ItemPlayer = new GameItemsViewModel
                {
                    IdGame = this.GameSelected.IdGame,
                    Namep = this.PlayerName.ToUpper()
                };
                this.Players.Add(ItemPlayer);
                
                this.IsVisibleUsr = false;
                this.MsggError = string.Empty;
                this.PlayerName = string.Empty;
            }
        }

        private IEnumerable<GameItemsViewModel> ToPlayersItemsModel()
        {
            // Transformar la lista Games a GamesItemsModel que hereda Games del Model
            return MainViewModel.GetInstance().PlayerList.Select(g => new GameItemsViewModel
            {                
                IdGame = g.IdGame,
                Namep  = g.Namep,
                Points = g.Points
            });
        }
        #endregion
    }
}
