namespace Apuntado.ViewModels
{
    using Apuntado.Models;
    using System.Collections.Generic;

    public class MainViewModel
    {
        #region Properties
        public List<Games> GamesList
        {
            get;
            set;
        }
        #endregion

        #region ViewModels
        public GamesViewModel   Games { get; set; }
        public GameViewModel    Game { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            instance = this;
            this.Games = new GamesViewModel();
        }
        #endregion

        #region Singlenton
        private static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }

            return instance;
        }
        #endregion

    }
}
