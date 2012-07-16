using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Newtonsoft.Json;

namespace QRzar
{
    public partial class SignUp : PhoneApplicationPage
    {
        public SignUp()
        {
            InitializeComponent();
        }

        public void CreateAccount(object sender, EventArgs e)
        {
            Networking.CreateUser(txt_Email.Text.ToLower(), txt_Password.Password, txt_Name.Text, txt_Photo.Text, eventhandler);
        }

        public void Cancel(object sender, EventArgs e)
        {
            //TODO: Possibly bad if the user hits this(by accident maybe) and hopes to return to sign up with all the entered info still intact
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        public void eventhandler(object sender, UploadStringCompletedEventArgs e)
        {
            lock (this)
            {
                NavigationService.Navigate(new Uri("/Joining Game.xaml", UriKind.Relative));
            }
        }

    }
}