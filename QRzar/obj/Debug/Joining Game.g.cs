﻿#pragma checksum "C:\Users\Eoin\WP7-QRzar\WP7-QRzar\QRzar\Joining Game.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3963C959BC6CABF9590076C4C626F494"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace QRzar {
    
    
    public partial class PhonePage1 : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.Button nextScreen;
        
        internal System.Windows.Controls.TextBlock gameNumberText;
        
        internal System.Windows.Controls.TextBlock teamColorText;
        
        internal System.Windows.Controls.TextBlock playerNumberText;
        
        internal System.Windows.Controls.TextBlock PlayerCode;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/QRzar;component/Joining%20Game.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.nextScreen = ((System.Windows.Controls.Button)(this.FindName("nextScreen")));
            this.gameNumberText = ((System.Windows.Controls.TextBlock)(this.FindName("gameNumberText")));
            this.teamColorText = ((System.Windows.Controls.TextBlock)(this.FindName("teamColorText")));
            this.playerNumberText = ((System.Windows.Controls.TextBlock)(this.FindName("playerNumberText")));
            this.PlayerCode = ((System.Windows.Controls.TextBlock)(this.FindName("PlayerCode")));
        }
    }
}

