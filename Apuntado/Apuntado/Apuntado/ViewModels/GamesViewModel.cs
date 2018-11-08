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
        public Sqlite sqlcon;
        public bool isRunning;
        private bool refreshCommand;
        private bool isVisiblePop;
        private bool isRefreshing;
        private MainViewModel mainViewModel;
        private string gameName;
        private int scoreNew;
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
        public int ScoreNew
        {
            get { return this.scoreNew; }
            set { SetValue(ref this.scoreNew, value); }
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
            this.ScoreNew = 101;
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

            // PLayer Table
            try
            {
                //sqlcon.DeleteTable<Players>();
                sqlcon.CreateTable<Players>();
            }
            catch (Exception ex)
            {
                
            }
           

            // Games Table
            sqlcon.CreateTable<Games>();

            

        }

        private IEnumerable<GamesItemsModel> ToGamesItemsModel()
        {
            // Transformar la lista Games a GamesItemsModel que hereda Games del Model
            return mainViewModel.GamesList.Select(g => new GamesItemsModel
            {
                IdGame = g.IdGame,
                Name = g.Name,
                Date = g.Date,
                ScoreMax = g.ScoreMax
            });
        }   

        private async void NewGame()
        {          
            this.MsggError = string.Empty;

            var NewGame = this.GameName;
            var ScoreGame = this.ScoreNew;

            if (string.IsNullOrEmpty(NewGame))
            {              
                this.MsggError = "Ingresar nombre del juego";
                return;
            }

            if (ScoreGame == null || ScoreGame <= 0)
            {
                this.MsggError = "El puntaje maximo es obligatorio";
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
                    Date = DateTime.Now,
                    ScoreMax = ScoreGame  };

                // Create New Register in BD
                var response = await this.sqlcon.AddNewReg(objgame);

                if (!response.IsSuccess)
                {
                    this.MsggError = response.Message;
                    return;
                }

                // Update ObservableCollection 
                // Get key game
                var strSQL = "SELECT MAX(IdGame) as IdGame FROM Games";
                var resp = await this.sqlcon.GetQuery<Games>(strSQL);
                var game_s = (List<Games>)sqlcon.GetQuery<Games>(strSQL).Result.Result;

                if (!resp.IsSuccess)
                {
                    this.sqlcon.Commit_or_Rollback("Rollback");
                    return;
                } 

                mainViewModel.GamesList.Add(new Games
                {
                    IdGame = game_s[0].IdGame,
                    Name = NewGame,
                    Date = DateTime.Now,
                    ScoreMax = ScoreGame
                });
                this.Games = new ObservableCollection<GamesItemsModel>(this.ToGamesItemsModel());
                this.IsVisiblePop = false;
                this.MsggError = string.Empty;
                this.GameName = string.Empty;
                this.ScoreNew =  0;
            }

            this.MsggError = string.Empty;

        }

        private void ToIsVisiblePop_F()
        {
            this.IsVisiblePop = false;
        }

        private void ToIsVisiblePop_T()
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
                return new RelayCommand(ToIsVisiblePop_T);
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
                return new RelayCommand(ToIsVisiblePop_F);
            }
        }
        #endregion

    }
}
