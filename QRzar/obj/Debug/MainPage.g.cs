﻿#pragma checksum "C:\Users\Eoin\WP7-QRzar\WP7-QRzar\QRzar\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C592C1E377FA96017831210348DC71EE"
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
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel TitlePanel;
        
        internal System.Windows.Controls.TextBlock ApplicationTitle;
        
        internal System.Windows.Controls.TextBlock PageTitle;
        
        internal System.Windows.Controls.Grid ContentPanel;
        
        internal System.Windows.Controls.TextBlock tbScanResultBarCode;
        
        internal System.Windows.Controls.TextBlock ScannedNumber;
        
        internal System.Windows.Controls.TextBlock TextNumber;
        
        internal System.Windows.Controls.TextBlock lbl_Email;
        
        internal System.Windows.Controls.TextBox txt_Email;
        
        internal System.Windows.Controls.TextBlock lbl_Password;
        
        internal System.Windows.Controls.TextBox txt_Password;
        
        internal System.Windows.Controls.Button btn_Login;
        
        internal System.Windows.Controls.HyperlinkButton hbtn_SignUp;
        
        internal System.Windows.Controls.HyperlinkButton hbtn_ForgotPassword;
        
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
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.TitlePanel = ((System.Windows.Controls.StackPanel)(this.FindName("TitlePanel")));
            this.ApplicationTitle = ((System.Windows.Controls.TextBlock)(this.FindName("ApplicationTitle")));
            this.PageTitle = ((System.Windows.Controls.TextBlock)(this.FindName("PageTitle")));
            this.ContentPanel = ((System.Windows.Controls.Grid)(this.FindName("ContentPanel")));
            this.tbScanResultBarCode = ((System.Windows.Controls.TextBlock)(this.FindName("tbScanResultBarCode")));
            this.ScannedNumber = ((System.Windows.Controls.TextBlock)(this.FindName("ScannedNumber")));
            this.TextNumber = ((System.Windows.Controls.TextBlock)(this.FindName("TextNumber")));
            this.lbl_Email = ((System.Windows.Controls.TextBlock)(this.FindName("lbl_Email")));
            this.txt_Email = ((System.Windows.Controls.TextBox)(this.FindName("txt_Email")));
            this.lbl_Password = ((System.Windows.Controls.TextBlock)(this.FindName("lbl_Password")));
            this.txt_Password = ((System.Windows.Controls.TextBox)(this.FindName("txt_Password")));
            this.btn_Login = ((System.Windows.Controls.Button)(this.FindName("btn_Login")));
            this.hbtn_SignUp = ((System.Windows.Controls.HyperlinkButton)(this.FindName("hbtn_SignUp")));
            this.hbtn_ForgotPassword = ((System.Windows.Controls.HyperlinkButton)(this.FindName("hbtn_ForgotPassword")));
        }
    }
}

