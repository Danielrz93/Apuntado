﻿namespace Apuntado.ViewModels
{
    using Apuntado.SQLite;
    using GalaSoft.MvvmLight.Command;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class GameItemsViewModel : Players
    {

        #region Attributes
        private int ponits_A;
        MainViewModel mainViewModel;
        private bool isEnable_P;
        private double isLose;
        private Games gameSelected;
        private bool isEnable_R;
        private double isReturn;
        public Sqlite sqlcon;
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
        public bool IsEnable_R
        {
            get { return this.isEnable_R; }
            set { SetValue(ref this.isEnable_R, value); }
        }
        public double IsReturn
        {
            get { return this.isReturn; }
            set { SetValue(ref this.isReturn, value); }
        }
        #endregion

        #region Constructor
        public GameItemsViewModel(Games game , Players player)
        {
            this.GameSelected = game;
            mainViewModel = MainViewModel.GetInstance();

            // Connection to DB
            string dbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                        "Apuntado.db3");

            sqlcon = new Sqlite(dbPath);

            if (player != null)
            {
                Validate_State(player.Points);
            }
            else
            {
                Validate_State(0);
            }            
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
        public ICommand ReturnGame
        {
            get
            {
                    return new RelayCommand(ReturnPlayer);
            }
        }
        public ICommand WinMatch
        {
            get
            {
                    return new RelayCommand(PlayerWins);
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

            var index = mainViewModel.PlayerList.FindIndex(
                                                    p => p.IdGame == this.IdGame &&
                                                         p.IdPLayer == this.IdPLayer);
            // Update score player 
            mainViewModel.Game.Players.RemoveAt(index);
            this.Points = score_t;
            mainViewModel.Game.Players.Insert(index , this);            

            // Guardar resultado 
            var objPlayer = new Players
            {
                IdGame   = this.IdGame,
                IdPLayer = this.IdPLayer, 
                Namep    = this.Namep,
                Points   = this.Points
            };
            var resp = await this.sqlcon.Update<Players>(objPlayer);
            this.Ponits_A = 0;

            // Validate score
            Validate_State(score_t);
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

            // Save result
            // Guardar resultado 
            var objPlayer = new Players
            {
                IdGame = this.IdGame,
                IdPLayer = this.IdPLayer,
                Namep = this.Namep,
                Points = this.Points
            };
            var resp = await this.sqlcon.Update<Players>(objPlayer);
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
            this.IsEnable_R = true;
            this.IsReturn = 1;
        }

        private void Entry()
        {
            this.IsEnable_P = true;
            this.IsLose = 1;
            this.IsEnable_R = false;
            this.IsReturn = 0.3;
        }

        private async void ReturnPlayer()
        {
            int scoremax;
            // Validate score max
            var ListP = mainViewModel.Game.Players.Where<GameItemsViewModel>(
                                                           p => p.IdPLayer != this.IdPLayer &&
                                                                p.Points   < this.GameSelected.ScoreMax );

            var cantPlayer = ListP.Count<GameItemsViewModel>();

            if (cantPlayer <= 0)
            {
                scoremax = this.GameSelected.ScoreMax - 1;
            }
            else
            {
                scoremax = ListP.Max<GameItemsViewModel>(p => p.Points);
            }

            // Update score player 
            var index = mainViewModel.PlayerList.FindIndex(
                                                    p => p.IdGame == this.IdGame &&
                                                         p.IdPLayer == this.IdPLayer );

            mainViewModel.Game.Players.RemoveAt(index);
            this.Points = scoremax;
            mainViewModel.Game.Players.Insert(index, this);

            // Save result
            // Guardar resultado 
            var objPlayer = new Players
            {
                IdGame = this.IdGame,
                IdPLayer = this.IdPLayer,
                Namep = this.Namep,
                Points = this.Points
            };
            var resp = await this.sqlcon.Update<Players>(objPlayer);
            this.Ponits_A = 0;

            this.Validate_State(scoremax);
        }

        private async void PlayerWins()
        {
            var action = await Application.Current.MainPage.DisplayActionSheet(
                  "Gana Ronda",
                  "Cancelar",
                  null,
                  "25" , "10");


            // Cancela action
            if (action == "Cancelar")
            {
                return;
            }

            this.Ponits_A = Convert.ToInt32(action);
            this.RestValue();
            this.Ponits_A = 0;
        }
       
        #endregion
    }
}
