namespace Apuntado.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.IO;   
    using Apuntado.SQLite;
    using Models;
    using GalaSoft.MvvmLight.Command;

    public class GamesViewModel : BaseViewModel
    {

        #region Attriubutes
        public ObservableCollection<GamesItemsModel> games;
        private Sqlite sqlcon;
        public bool isRunning;
        private bool refreshCommand;
        private bool isVisiblePop;
        private bool isRefreshing;
        private MainViewModel mainViewModel;
        private string gameName;
        private string msggError;       
        #endregion

        #region Properties
        public ObservableCollection<GamesItemsModel> Games
        {
            get { return this.games; }
            set { SetValue(ref this.games, value); }
        }
        public bool IsRefreshing
        {
            get { return this.isRefreshing; }
            set { SetValue(ref this.isRefreshing, value); }
        }
        public bool IsVisiblePop
        {
            get { return this.isVisiblePop; }
            set { SetValue(ref this.isVisiblePop, value); }
        }
        public string GameName
        {
            get { return this.gameName; }
            set { SetValue(ref this.gameName, value); }
        }
        public string MsggError
        {
            get { return this.msggError; }
            set { SetValue(ref this.msggError, value); }
        }
        #endregion

        #region Constructors
        public GamesViewModel()
        {
            // Instancia de la MainViewModel
            mainViewModel = MainViewModel.GetInstance();

            // Connection to DB
            string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Apuntado.db3");

            sqlcon = new Sqlite(dbPath);

            this.CreateTables();

            this.LoadGames();
            this.IsVisiblePop = false;
            this.IsRefreshing = false;
        }        
        #endregion

        #region Methods

        private async void LoadGames()
        {
            this.IsRefreshing = true;
            // get complete table
            var response = await this.sqlcon.GetAllTable<Games>( );
            mainViewModel.GamesList = (List<Games>)response.Result;
            this.Games = new ObservableCollection<GamesItemsModel>(this.ToGamesItemsModel());
            this.IsRefreshing = false;
        }

        private void CreateTables()
        {

            // Games Table
            sqlcon.CreateTable<Games>();

        }
        private IEnumerable<GamesItemsModel> ToGamesItemsModel()
        {
            // Transformar la lista Games a GamesItemsModel que hereda Games del Model
            return mainViewModel.GamesList.Select(g => new GamesItemsModel
            {
                IdGame = g.IdGame,
                Name   = g.Name,
                Date   = g.Date
            });
        }   

        private async void NewGame()
        {          
            this.MsggError = string.Empty;

            var NewGame = this.GameName;
            if (string.IsNullOrEmpty(NewGame))
            {              
                this.MsggError = "Ingresar nombre del juego";
                return;
            }

            // Validar si el nombre ya existe
            var game_l = mainViewModel.GamesList.Where(g => g.Name == NewGame).FirstOrDefault();

            if (game_l != null)
            {
                this.MsggError = "El nombre ingresado ya existe";
                return;
            }
            else
            {
                var objgame = new Games{
                    Name = NewGame,
                    Date = DateTime.Now };

                // Create New Register in BD
                var response = await this.sqlcon.AddNewReg(objgame);

                if (!response.IsSuccess)
                {
                    this.MsggError = response.Message;
                    return;
                }

                // Update ObservableCollection                
                mainViewModel.GamesList.Add(new Games
                {
                    Name = NewGame,
                    Date = DateTime.Now,
                });
                this.Games = new ObservableCollection<GamesItemsModel>(this.ToGamesItemsModel());
                this.IsVisiblePop = false;
                this.MsggError = string.Empty;
                this.GameName = string.Empty;
            }

            this.MsggError = string.Empty;

        }

        private void ToIsVisiblePop_f()
        {
            this.IsVisiblePop = false;
        }

        private void ToIsVisiblePop_t()
        {
            this.IsVisiblePop = true;
        }

        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(LoadGames);
            }
        }

        public ICommand NewGamePop
        {
            get
            {                 
                return new RelayCommand(ToIsVisiblePop_t);
            }
        }

        public ICommand CreateGame
        {
            get
            {

                return new RelayCommand(NewGame);
            }
        }

        public ICommand CloseCreate
        {
            get
            {

                return new RelayCommand(ToIsVisiblePop_f);
            }
        }
        #endregion

    }
}
