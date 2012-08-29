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
using com.google.zxing.common;
using com.google.zxing.qrcode;
using Microsoft.Devices;
using System.Windows.Threading;
using Tophat;

namespace QRzar
{
    public partial class GamePage : PhoneApplicationPage
    {
        PhotoCamera _cam;
        VideoBrush _videoBrush = new VideoBrush();
        DispatcherTimer _timerqsec;
        DispatcherTimer _timer1sec;


        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _reader;

        private bool camInitializing;

        private int myGameid;
        private Player myPlayer;

        // Constructor
        public GamePage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Recover state after tombstoning
            if (State.ContainsKey("gameid"))
                myGameid = (int)(State["gameid"]);
            else
            {
                myGameid = int.Parse(NavigationContext.QueryString["gameid"]);
                //Save the current game id so that the game is easily continued when the user reloads the app
                Networking.SaveData("CurrentGame", myGameid.ToString());
            }

            myPlayer = Networking.GetPlayerByGameId_Local(myGameid);

            //This timer is for scanning qrcodes 4 times a second
            _timerqsec = new DispatcherTimer();
            _timerqsec.Interval = TimeSpan.FromMilliseconds(250);

            //Set the event handler for the timer but stop it because it doesn't need to scan unless the camera is on
            _timerqsec.Tick += (o, evt) => ScanPreviewBuffer();
            _timerqsec.Stop();

            //This timer is for pinging the server every second for updates
            _timer1sec = new DispatcherTimer();
            _timer1sec.Interval = TimeSpan.FromMilliseconds(1000);


            _timer1sec.Tick += (o, evt) => Update();
            

            _timer1sec.Start();
            base.OnNavigatedTo(e);
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

        protected override void  OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            State["gameid"] = myGameid;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //If the user presses the back button, assume they are going back to the JoinGame page

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

                Dispatcher.BeginInvoke(() =>
                {
                    _timerqsec.Start();
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
                    if (QRCodeManager.IsValidQRCode(result.Text) && result.Text[0] != (myPlayer as QRPlayer).team)
                    {
                        if (!Networking.IsMakingRequest)
                        {
                            var dict = new Dictionary<string, string>();
                            dict["qrcode"] = result.Text;

                            Networking.Kill(dict, (o, e) =>
                            {
                                if (!Networking.FailedRequest)
                                {
                                    MessageBox.Show("+100");
                                    lbl_ScoreBonus.Text = "+100";
                                    lbl_ScoreBonus.Foreground = new SolidColorBrush(Color.FromArgb(0, 10, 255, 30));
                                    StoryboardBonusText.AutoReverse = true;
                                    StoryboardBonusText.Begin();
                                }
                            });
                        }
                         
                    }
                });

                _cam.Focus();
            }
            catch
            {
            }
        }


        private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            StartCamera();
        }

        private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            StopCamera();
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

                        _timerqsec.Start();

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


        private void StopCamera()
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    _cam.Initialized -= cam_Initialized;
                    _cam.CancelFocus();
                    _cam.Dispose();
                    _timerqsec.Stop();

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



        private void OnGetGame()
        {
        }

        private void Update()
        {

            //Get the game which contains the list of players
            //Networking.GetGameById<Game>(myGame.id,
            //    (o, e) => OnGetGame());
        }
    }
}