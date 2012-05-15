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

namespace WP7.VideoScanZXing.SampleApp
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
            WP7.ScanBarCode.BarCodeManager.StartScan(
                // on success
                (b) => Dispatcher.BeginInvoke(() => 
                    {
                        ScannedNumber.Text = b;

                    }),
                // on error
                (ex) => Dispatcher.BeginInvoke(() => 
                    {
                        ScannedNumber.Text = "No Code Found";
                        NavigationService.GoBack();
                    }),
                // Decode a QR Code
                BarcodeFormat.QR_CODE);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	
        }

        private void QRload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        		NavigationService.Navigate(new Uri("/Joining Game.xaml?Text=" + ScannedNumber.Text, UriKind.Relative));
        }

        private void Textload_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        		NavigationService.Navigate(new Uri("/Joining Game.xaml?Text=" + TextNumber.Text , UriKind.Relative));
        }
		

    }
}