﻿#pragma checksum "C:\Users\Eoin\WP7-QRzar\WP7-QRzar\QRzar\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "5EDA208D4307D758353000ECE278519C"
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
using Microsoft.Phone.Shell;
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Media.Animation.Storyboard StoryboardRevealTop;
        
        internal System.Windows.Media.Animation.Storyboard StoryboardRevealTop_Reverse;
        
        internal System.Windows.Media.Animation.Storyboard StoryboardText;
        
        internal System.Windows.Media.Animation.Storyboard StoryboardText_Reverse;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Shapes.Rectangle video;
        
        internal System.Windows.Controls.TextBlock txt_Body;
        
        internal System.Windows.Controls.TextBlock txt_Header;
        
        internal System.Windows.Shapes.Rectangle rectangle;
        
        internal System.Windows.Controls.Image image;
        
        internal System.Windows.Controls.Button btn_Rank;
        
        internal System.Windows.Controls.Button btn_Info;
        
        internal System.Windows.Controls.Button btn_QR;
        
        internal System.Windows.Controls.Image image1;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton btn_ScanMyCode;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/QRzar;component/MainPage.xaml", System.UriKind.Relative));
            this.StoryboardRevealTop = ((System.Windows.Media.Animation.Storyboard)(this.FindName("StoryboardRevealTop")));
            this.StoryboardRevealTop_Reverse = ((System.Windows.Media.Animation.Storyboard)(this.FindName("StoryboardRevealTop_Reverse")));
            this.StoryboardText = ((System.Windows.Media.Animation.Storyboard)(this.FindName("StoryboardText")));
            this.StoryboardText_Reverse = ((System.Windows.Media.Animation.Storyboard)(this.FindName("StoryboardText_Reverse")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.video = ((System.Windows.Shapes.Rectangle)(this.FindName("video")));
            this.txt_Body = ((System.Windows.Controls.TextBlock)(this.FindName("txt_Body")));
            this.txt_Header = ((System.Windows.Controls.TextBlock)(this.FindName("txt_Header")));
            this.rectangle = ((System.Windows.Shapes.Rectangle)(this.FindName("rectangle")));
            this.image = ((System.Windows.Controls.Image)(this.FindName("image")));
            this.btn_Rank = ((System.Windows.Controls.Button)(this.FindName("btn_Rank")));
            this.btn_Info = ((System.Windows.Controls.Button)(this.FindName("btn_Info")));
            this.btn_QR = ((System.Windows.Controls.Button)(this.FindName("btn_QR")));
            this.image1 = ((System.Windows.Controls.Image)(this.FindName("image1")));
            this.btn_ScanMyCode = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("btn_ScanMyCode")));
        }
    }
}

