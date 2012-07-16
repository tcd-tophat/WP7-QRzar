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
using com.google.zxing;
using System.IO;
using Newtonsoft.Json;

namespace QRzar
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Bar_Code.BarCodeManager.StartScan(
                // on success
                (b) => Dispatcher.BeginInvoke(() =>
                    {
                        ScannedNumber.Text = b;

                    }),
                // on error
                (ex) => Dispatcher.BeginInvoke(() => 
                    {
                        ScannedNumber.Text = "No Code Found";
                        //if (NavigationService.CanGoBack)
                          //  NavigationService.GoBack();
                    }),
                // Decode a QR Code
                BarcodeFormat.QR_CODE);
        }

        private void QRload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        		NavigationService.Navigate(new Uri("/Joining Game.xaml?Text=" + ScannedNumber.Text, UriKind.Relative));
        }

        private void Textload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        		NavigationService.Navigate(new Uri("/Joining Game.xaml?Text=" + TextNumber.Text , UriKind.Relative));
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            string email = txt_Email.Text;
            string password = txt_Password.Text;
            if (email == "" || password == "")
            {
                //Either or both of the email and password fields are empty.
                //TODO: Check format of email address before sending it
                MessageBox.Show("Please fill in an email and password please!", "Error", MessageBoxButton.OK);
                return;
            }

            //Networking.TestConnection(new DownloadStringCompletedEventHandler(Test));

            Networking.Login(email, password, eventhandler);
            return;

            //if (TextNumber.Text == "")
            //    MessageBox.Show("Please scan a QR code!");
            NavigationService.Navigate(new Uri("/Joining Game.xaml?Text=", UriKind.Relative));
        }

        private void hbtn_SignUp_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SignUp.xaml", UriKind.Relative));
        }

        private void hbtn_ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/SignUp.xaml", UriKind.Relative));
        }

        public void eventhandler(object sender, UploadStringCompletedEventArgs e)
        {
            lock (this)
            {
                if (Networking.ApiToken != "")
                {
                    NavigationService.Navigate(new Uri("/Joining Game.xaml", UriKind.Relative));
                }
            }
        }

    }
}