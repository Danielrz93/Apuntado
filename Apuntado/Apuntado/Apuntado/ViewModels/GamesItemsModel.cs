namespace Apuntado.ViewModels
{
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using Apuntado.Views;

    public class GamesItemsModel : Games
    {
        #region Attributes
        //private MainViewModel mainViewModel;
        #endregion

        #region Commands
        public ICommand DeleteGame
        {
            get
            {
                return new RelayCommand(DeleteGameL);
            }
        }
        public ICommand GamePlay
        {
            get
            {
                return new RelayCommand(GamePlayL);
            }
        }
        #endregion

        #region Contructor
        public GamesItemsModel()
        {
            
        }

        #endregion

        #region Methods
        private async void DeleteGameL()
        {

            // ask delete register                   
            var answer = await Application.Current.MainPage.DisplayAlert(
                                  "Eliminar Juego",
                                  "¿Desea eliminar el juego seleccionado?",
                                  "Si",
                                  "Cancelar");

            if (!answer)
            {
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();            

            //  Delete Game in BD
            var sqlcon = mainViewModel.Games.sqlcon;

            var objdelete = new Games
            {
                IdGame = this.IdGame,
                Name = this.Name,
                Date = this.Date,
                ScoreMax = this.ScoreMax
            };

            var obj = (Games)this;

           
            var response = await sqlcon.DeleteReg(objdelete);

            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                   "Error",
                   response.Message,
                   "Accept");
                return;
            }
            else
            {
                // Delete players of game
                var strSQL = "DELETE FROM Players WHERE IdGame = '" + this.IdGame + "'";
                var res = await sqlcon.GetQuery<Players>(strSQL);

                mainViewModel.Games.Games.Remove(this);
                mainViewModel.GamesList.RemoveAll(g => g.IdGame == this.IdGame);
            }

        }

        private async void GamePlayL()
        {
            MainViewModel.GetInstance().Game = new GameViewModel(this);
            var main = MainViewModel.GetInstance();
            await Application.Current.MainPage.Navigation.PushAsync(new GamePage());
        }
        #endregion
    }
}
