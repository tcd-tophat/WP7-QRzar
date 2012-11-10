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
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using com.google.zxing;
using com.google.zxing.qrcode;
using com.google.zxing.common;
using Microsoft.Devices;
using System.IO;
using Tophat;

namespace QRzar
{
    public partial class MainPage : PhoneApplicationPage
    {
        Game myGame;
        int gameCode;
        string playerCode;

        PhotoCamera _cam;
        VideoBrush _videoBrush = new VideoBrush();
        DispatcherTimer _timer;

        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _reader;

        private bool isScanningGame;
        private bool isScanningPlayer;

        private bool isGameScanned;
        private bool isPlayerScanned;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //Networking.DeleteFile("CurrentGame");
            //Networking.DeleteFile("Players");

            //Check if a game in progress has already been saved and continue it
            string data = Networking.LoadData("CurrentGame");
            if (data != "" && MessageBox.Show("Would you like to resume the game with id of " + data, "Continue Game", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                NavigationService.Navigate(new Uri("/GamePage.xaml?gameid=" + data, UriKind.Relative));
            else
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromMilliseconds(250);
                _timer.Tick += (o, arg) => ScanPreviewBuffer();
                //The timer auto-starts so it needs to be stopped here
                _timer.Stop();
            }

            //Login to the server
            Networking.Login();

            _cam = new PhotoCamera();

            _cam.Initialized += cam_Initialized;

            video.Fill = _videoBrush;
            _videoBrush.SetSource(_cam);
            _videoBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
        }

        //ends the camera screen and dipsoses of handlers and camera
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            try
            {
                _cam.Initialized -= cam_Initialized;
                _cam.CancelFocus();
                _cam.Dispose();
            }
            catch 
            {
                //Throws an exception if the camera has already been disposed
            }

            base.OnNavigatingFrom(e);

        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        //camera setup
        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(_cam.PreviewResolution.Width);
                int height = Convert.ToInt32(_cam.PreviewResolution.Height);

                _luminance = new PhotoCameraLuminanceSource(width, height);
                _reader = new QRCodeReader();

                _cam.FlashMode = FlashMode.Auto;
                _cam.Focus();
            }
            catch
            {
                //Throws an exception if the camera is disposed while this method is being called
            }
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                _cam.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                var result = _reader.decode(binBitmap);

                Dispatcher.BeginInvoke(() =>
                {
                    SuccessfulScan(result);
                });

                _cam.Focus();
            }
            catch
            {
            }
        }

        private void SuccessfulScan(Result result)
        {
            if (isScanningGame)
            {
                int temp;

                if (int.TryParse(result.Text, out temp))
                {
                    gameCode = temp;
                    isGameScanned = true;
                    btn_ScanGameID.Background = new SolidColorBrush(Colors.Green);

                    btn_ScanGameID.Opacity = 0.5;
                    video.Opacity = 0;
                    isScanningGame = false;
                    //Stop the scanner
                    _timer.Stop();
                    MessageBox.Show("You scanned the qrcode for game: " + result.Text);
                }
            }
            else
            {
                if (QRCodeManager.IsValidQRCode(result.Text))
                {
                    playerCode = result.Text;
                    isPlayerScanned = true;
                    btn_ScanPlayerID.Background = new SolidColorBrush(Colors.Green);

                    btn_ScanPlayerID.Opacity = 0.5;
                    video.Opacity = 0;
                    isScanningPlayer = false;
                    //Stop the scanner
                    _timer.Stop();
                    MessageBox.Show("You scanned the qrcode for player: " + result.Text);
                }
            }

            if (isPlayerScanned && isGameScanned)
            {
                var extras = new Dictionary<string, object>();
                extras["qrcode"] = playerCode;
                if (MessageBox.Show(gameCode.ToString(), "Would you like join the game with id of", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Networking.JoinGame<QRPlayer>("anon-WP7", gameCode, extras, (o, e) => OnJoinGame(gameCode));
                }
            }
        }


        bool presscomplete = true;

        private void btn_ScanPlayerID_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (presscomplete)
            {
                if (!isScanningPlayer)
                {
                    btn_ScanPlayerID.Opacity = 1;
                    video.Opacity = 1;
                    isScanningPlayer = true;
                    //Start the scanner
                    _timer.Start();
                }
                else
                {
                    btn_ScanPlayerID.Opacity = 0.5;
                    video.Opacity = 0;
                    isScanningPlayer = false;
                    //Stop the scanner
                    _timer.Stop();
                }
                isScanningGame = false;
                btn_ScanGameID.Opacity = 0.5;
                presscomplete = false;
            }
        }

        private void btn_ScanGameID_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (presscomplete)
            {
                if (!isScanningGame)
                {
                    btn_ScanGameID.Opacity = 1;
                    video.Opacity = 1;
                    isScanningGame = true;
                    //Start the scanner
                    _timer.Start();
                }
                else
                {
                    btn_ScanGameID.Opacity = 0.5;
                    video.Opacity = 0;
                    isScanningGame = false;
                    //Stop the scanner
                    _timer.Stop();
                }
                isScanningPlayer = false;
                btn_ScanPlayerID.Opacity = 0.5;
                presscomplete = false;
            }
            
        }

        private void btn_ScanX_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            presscomplete = true;
        }

       /* private void ScanMyCode(object sender, EventArgs e)
        {
            isScanningGame = false;
            MessageBox.Show("Please scan the QRCode on your t-shirt");
        }
        */

        #region JoiningAGame

        private void OnJoinGame(int gameid)
        {
            _timer.Tick -= (o, e) => ScanPreviewBuffer();
            if (!Networking.FailedRequest)
            {
                myGame = new Game(gameid);
                _timer.Tick += (o, e) => CheckGameStarted();
                _timer.Start();
            }
            else
            {
                switch (Networking.LastError)
                {
                    case 409:
                        isScanningGame = false;
                        MessageBox.Show("This qrcode has already been used to join this game. Please find another qrcode or game!");
                        break;
                }
            }
        }

        private void CheckGameStarted()
        {
            if (myGame.started || true)
            {
                //No need to check that its started anymore
                _timer.Tick -= (o, evt) => CheckGameStarted();
                _timer.Stop();

                //The game has started, so move into the game page
                NavigationService.Navigate(new Uri("/GamePage.xaml?gameid=" + myGame.id, UriKind.Relative));
            }
            else
            {
                Networking.GetGameById<Game>(myGame.id, (o, e) => OnGetGame());
            }
        }

        private void OnGetGame()
        {
            myGame = Networking.GetGameById_Local(myGame.id);
        }




        #endregion

    }
}