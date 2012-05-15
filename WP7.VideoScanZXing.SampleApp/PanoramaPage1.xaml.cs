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
    public partial class PanoramaPage1 : PhoneApplicationPage
    {
        public PanoramaPage1()
        {
            InitializeComponent();
        }
		
		private int PlayerScore = 0;
		
		

        private void Capture_Click(object sender, RoutedEventArgs e)
			{
            WP7.ScanBarCode.BarCodeManager.StartScan(
                // on success
                (b) => Dispatcher.BeginInvoke(() => 
                    {	
						//print a code if found, incriment player score and change shown score
                        ScannedCode.Text = "code found:" + b;
						
						PlayerScore += 1;
						string score = PlayerScore.ToString();
						currentScore.Text = score;

                    }),
                // on error
                (ex) => Dispatcher.BeginInvoke(() => 
                    {
                        ScannedCode.Text = "No Code Found";
                        NavigationService.GoBack();
                    }),
                // Decode a QR Code
                BarcodeFormat.QR_CODE);
        }

        
    }
}