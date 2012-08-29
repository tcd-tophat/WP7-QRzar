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
        string QRCode;

        PhotoCamera _cam;
        VideoBrush _videoBrush = new VideoBrush();
        DispatcherTimer _timer;

        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _reader;
        private bool camInitializing;

        private bool isScanningGame;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Networking.DeleteFile("data\\CurrentGame");
            Networking.DeleteFile("data\\Players");

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
                MessageBox.Show("Please scan the QRCode on your t-shirt");
            }

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
        //camera setup
        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            try
            {
                int width = Convert.ToInt32(_cam.PreviewResolution.Width);
                int height = Convert.ToInt32(_cam.PreviewResolution.Height);

                _luminance = new PhotoCameraLuminanceSource(width, height);
                _reader = new QRCodeReader();

                Dispatcher.BeginInvoke(() =>
                {
                    _timer.Start();
                });

                _cam.FlashMode = FlashMode.Auto;
                _cam.Focus();


            }
            catch
            {
                //Throws an exception if the camera is disposed while this method is being called
            }

            //The camera is now done intializing 
            camInitializing = false;
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
                    //Stop the timer so that it doesn't scan the same qrcode multiple times
                    _timer.Stop();
                    if (isScanningGame)
                        SuccessfulGameScan(result);
                    else
                        SuccessfulScan(result);
                    //Start up the timer again after the scan has been completed
                    _timer.Start();
                });

                _cam.Focus();

            }
            catch
            {
            }
        }

        private void SuccessfulGameScan(Result result)
        {
            try
            {
                var extras = new Dictionary<string, object>();
                extras["qrcode"] = QRCode;

                int gameid = int.Parse(result.Text);
                if (MessageBox.Show(gameid.ToString(), "Would you like join the game with id of", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Networking.JoinGame<QRPlayer>("anon-WP7", gameid, extras, (o, e) => OnJoinGame(gameid));
                }
            }
            catch
            {
            }
        }

        private void SuccessfulScan(Result result)
        {
            try
            {
                MessageBox.Show(result.Text);
                if (QRCodeManager.IsValidQRCode(result.Text))
                {
                    if (!Networking.IsMakingRequest) //Make sure only 1 request gets sent
                    {
                        if (MessageBox.Show(result.Text, "Would you like to continue with the QRcode", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            //Login to the server
                            Networking.Login((o, e) =>
                            {
                                if (!Networking.FailedRequest)
                                {
                                    QRCode = result.Text;
                                    isScanningGame = true;
                                    MessageBox.Show("Now scan the QRcode for the game you want to join");
                                }
                                else if (Networking.isStopped)
                                    MessageBox.Show("Couldn't log in");
                            });
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Exception");
            }
        }




        private void btn_QR_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            StoryboardRevealTop_Reverse.Stop();
            StoryboardRevealTop.Begin();

            video.Opacity = 100;
            StartCamera();
        }

        private void btn_QR_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //If a request is being made, we want to leave the camera visible no matter what
            //Then if the login was successful, we leave the text box exposed for the user to be able to see whats
            //happening before the game starts
            if (!Networking.IsMakingRequest)
            {
                StoryboardRevealTop.Stop();
                StoryboardRevealTop_Reverse.Begin();

                video.Opacity = 0;
                StopCamera();
            }
        }

        private void btn_Rank_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            StoryboardRevealTop_Reverse.Stop();
            StoryboardRevealTop.Begin();
        }

        private void btn_Rank_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            StoryboardRevealTop.Stop();
            StoryboardRevealTop_Reverse.Begin();
        }

        private void btn_Info_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            StoryboardText.Begin();
            StoryboardText_Reverse.Stop();

            StoryboardRevealTop_Reverse.Stop();
            StoryboardRevealTop.Begin();
        }

        private void btn_Info_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            StoryboardText.Stop();
            StoryboardText_Reverse.Begin();

            StoryboardRevealTop.Stop();
            StoryboardRevealTop_Reverse.Begin();
        }

        private void ScanMyCode(object sender, EventArgs e)
        {
            isScanningGame = false;
            MessageBox.Show("Please scan the QRCode on your t-shirt");
        }


        private void StopCamera()
        {
            Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        _cam.Initialized -= cam_Initialized;
                        _cam.CancelFocus();
                        _cam.Dispose();
                        _timer.Stop();

                        //The camera may be still initalizing while this code is called, so set initializing to false
                        //in order to prevent it from being stuck initializing forever
                        camInitializing = false;
                    }
                    catch
                    {
                        //Throws an exception if the camera hasnt been initialised yet
                    }
                });
        }

        private void StartCamera()
        {
            //Start it in a new thread so that the ui isn't blocked
            Dispatcher.BeginInvoke(() =>
            {
                    try
                    {
                        if (!camInitializing) //Prevents double initalization of the camera(which throws an exception)
                        {
                            camInitializing = true;
                            _cam = new PhotoCamera();

                            _cam.Initialized += cam_Initialized;

                            _timer.Start();

                            video.Fill = _videoBrush;
                            _videoBrush.SetSource(_cam);
                            _videoBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };
                        }
                    }
                    catch
                    {
                        //Throws an exception if the camera hasnt been initialised yet
                    }

                });
        }


        #region JoiningAGame



        private void OnGetGames()
        {
            if (!Networking.FailedRequest)
            {
                if (myGame == null)
                {
                    MessageBox.Show("There are no games to join at the moment");
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
            else
            {
                if (MessageBox.Show("Failed to download the list of games.\nPlease check your internet connection and try again.\n\nWould you like to try again?", "Error", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    Networking.GetGames<Game>((o, evt) => OnGetGames());
            }
        }

        private void OnJoinGame(int gameid)
        {
            if (!Networking.FailedRequest)
            {
                myGame = new Game(gameid);
                _timer.Tick += (o, evt) => CheckGameStarted();
            }
        }

        private void CheckGameStarted()
        {
            if (myGame.started || true)
            {
                //No need to check that its started anymore
                _timer.Tick -= (o, e) => CheckGameStarted();

                //The game has started, so move into the game page
                NavigationService.Navigate(new Uri("/GamePage.xaml?gameid=" + myGame.id, UriKind.Relative));
            }
            else
            {
                Networking.GetGameById<Game>(myGame.id,
                    (o, e) => OnGetGame());
            }
        }

        private void OnGetGame()
        {
            myGame = Networking.GetGameById_Local(myGame.id);
        }




        #endregion

    }
}