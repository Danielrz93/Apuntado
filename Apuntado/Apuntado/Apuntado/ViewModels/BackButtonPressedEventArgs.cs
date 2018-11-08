using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apuntado.ViewModels
{
    public class BackButtonPressedEventArgs : EventArgs
    {
        public bool Handled { get; set; }

        public BackButtonPressedEventArgs()
        {
           
        }

    }
}
