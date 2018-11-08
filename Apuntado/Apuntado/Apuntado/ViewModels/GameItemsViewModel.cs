namespace Apuntado.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class GameItemsViewModel : Players
    {

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Attributes
        private int ponits_A;
        MainViewModel mainViewModel;
        private bool isEnable_P;
        private double isLose;
        private Games gameSelected;
        #endregion

        #region Properties
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
        #endregion

        #region Constructor
        public GameItemsViewModel(Games game)
        {
            this.GameSelected = game;
            mainViewModel = MainViewModel.GetInstance();

            Validate_State(this.Points);
        }       
        #endregion

        #region Commands
        public ICommand RestCommand
        {
            get
            {
                    return new RelayCommand(RestValue);
            }
        }
        public ICommand PlusCommand
        {
            get
            {
                    return new RelayCommand(PlusVlaue);
            }
        }
        #endregion

        #region Methods
        private async void RestValue()
        {
            //get input points
            var points = this.Ponits_A;

            // Get actual points
            int score_t = this.Points;

            score_t = score_t - points;

            // Save BD
            var sqlCon = mainViewModel.Games.sqlcon;

            var objPlayer = new Players
            {
                IdGame = this.IdGame
            };

            var index = mainViewModel.PlayerList.FindIndex(
                                                    p => p.IdGame == this.IdGame &&
                                                         p.IdPLayer == this.IdPLayer);
            // Update score player 
            mainViewModel.Game.Players.RemoveAt(index);
            this.Points = score_t;
            mainViewModel.Game.Players.Insert(index , this);
            this.Ponits_A = 0;
        }

        private async void PlusVlaue()
        {
            //get input points
            var points = this.Ponits_A;

            // Get actual points
            int score_t = this.Points;

            score_t = score_t + points;

            var index = mainViewModel.PlayerList.FindIndex(
                                                    p => p.IdGame == this.IdGame &&
                                                         p.IdPLayer == this.IdPLayer);
            // Update score player 
            mainViewModel.Game.Players.RemoveAt(index);
            this.Points = score_t;
            mainViewModel.Game.Players.Insert(index, this);
            this.Ponits_A = 0;

            // Validate score
            Validate_State(score_t);
        }

        private void Validate_State(int score_a)
        {
            // Validate score
            var score_max = this.GameSelected.ScoreMax;
            if (score_a >= score_max)
            {
                this.Removed();
            }
            else
            {

                this.Entry();
            }
        }

        private void Removed()
        {
            this.IsEnable_P = false;
            this.IsLose = 0.3;
        }

        private void Entry()
        {
            this.IsEnable_P = true;
            this.IsLose = 1;
        }
        #endregion
    }
}
