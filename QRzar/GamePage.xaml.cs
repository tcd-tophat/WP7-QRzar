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
using System.Device.Location;
using System.Windows.Threading;
using Tophat;

namespace QRzar
{
    public partial class GamePage : PhoneApplicationPage
    {
        bool firstalive = true;

        PhotoCamera _cam;
        VideoBrush _videoBrush = new VideoBrush();
        DispatcherTimer _timerqsec;
        DispatcherTimer _timer1sec;
        GeoCoordinateWatcher watcher;

        private PhotoCameraLuminanceSource _luminance;
        private QRCodeReader _reader;

        private int myGameid;
        private QRPlayer myPlayer;
        private int timeleft;
        private bool isKilling;

        private Team[] teams;
        // Constructor
        public GamePage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Recover state after tombstoning
            if (State.ContainsKey("gameid"))
            {
                myGameid = (int)State["gameid"];
                timeleft = (int)((((DateTime)State["endtime"]).Ticks - DateTime.Now.Ticks) / 10000000);
            }
            //Starting a new game
            else
            {
                myGameid = int.Parse(NavigationContext.QueryString["gameid"]);
                //Save the current game id so that the game is easily continued when the user reloads the app
                Networking.SaveData("CurrentGame", myGameid.ToString());
            }


            myPlayer = GetPlayerByGameId(myGameid) as QRPlayer;
            isKilling = false;

            //Setup the camera and video brush
            _cam = new PhotoCamera();

            _cam.Initialized += cam_Initialized;

            video.Fill = _videoBrush;
            _videoBrush.SetSource(_cam);
            _videoBrush.RelativeTransform = new CompositeTransform() { CenterX = 0.5, CenterY = 0.5, Rotation = 90 };

            //Setup the GPS tracker
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.StatusChanged += watcher_StatusChanged;
            watcher.PositionChanged += watcher_PositionChanged;

            //This timer is for scanning qrcodes 4 times a second
            _timerqsec = new DispatcherTimer();
            _timerqsec.Interval = TimeSpan.FromMilliseconds(250);

            //Set the event handler for the timer but stop it because it doesn't need to scan unless the camera is on
            _timerqsec.Tick += (o, evt) => ScanPreviewBuffer();
            _timerqsec.Stop();

            //This timer is for pinging the server every second for updates
            _timer1sec = new DispatcherTimer();
            _timer1sec.Interval = TimeSpan.FromMilliseconds(1000);


            Networking.GetGameById<QRGame>(myGameid, (obj,evt) => OnGetGame());
            
            _timer1sec.Start();
            base.OnNavigatedTo(e);
        }

        //Exits the game screen and disposes of handlers and camera
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _timer1sec.Stop();
            _timerqsec.Stop();

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

        //Tombstoning handled here
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            QRGame game = Networking.GetGameById_Local(myGameid) as QRGame;

            if (game != null)
            {
                State["gameid"] = game.id;
                State["endtime"] = game.endTime;
            }
            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit this game?", "Quit Game", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Networking.Logout();
                //The timers continue even after navigating from the page
                //so they must be stopped first
                _timer1sec.Stop();
                _timerqsec.Stop();
            }
            else
                e.Cancel = true;

            base.OnBackKeyPress(e);
        }

        private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            if (e.Status == GeoPositionStatus.Disabled)
            {
                MessageBox.Show("Your device does not support location tracking");
            }
        }

        private void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (!double.IsNaN(e.Position.Location.Latitude))
            {
                Networking.UpdateGPS(myPlayer.id, e.Position.Location.Latitude,
                   e.Position.Location.Longitude);
            }
        }

        public Player GetPlayerByGameId(int GameId)
        {
            for (int i = 0; i < Networking.Players.Count; i++)
            {
                Team team = (Networking.Players[i] as QRPlayer).team as Team;
                if (team.game != null && team.game.id == GameId)
                {
                    return Networking.Players[i];
                }
            }
            return null;
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
                    if(myPlayer.alive)
                    {
                        if (QRCodeManager.IsValidQRCode(result.Text) && (result.Text[0] != (myPlayer as QRPlayer).qrcode[0]))
                        {
                            if (!isKilling)
                            {
                                isKilling = true;
                                var dict = new Dictionary<string, object>();

                                dict["killer"] = myPlayer;
                                dict["victim_qrcode"] = result.Text;

                                Networking.Kill(dict, (o, e) =>
                                {
                                    if (!Networking.FailedRequest)
                                    {
                                        DisplayBonus_Green("+100");
                                    }
                                    else
                                    {
                                        switch (Networking.LastError)
                                        {
                                            case 404:
                                                DisplayMessage_Red("That player isn't playing this game!");
                                                break;
                                            case 409:
                                                DisplayMessage_Red("That player is already dead!");
                                                break;
                                            //case 410:
                                            //DisplayMessage_Red("You are dead!");
                                            //break;
                                            default:
                                                Dispatcher.BeginInvoke(() => MessageBox.Show(Networking.LastError.ToString()));
                                                break;
                                        }
                                    }
                                    isKilling = false;
                                });

                            }
                        }
                    }
                    else
                    {
                        if (QRCodeManager.IsValidQRCode(result.Text) && (result.Text[1] == 'E'))
                        {
                            //If the player isn't alive, they must scan their respawn code instead
                            Networking.Respawn(result.Text, myPlayer.id, null);
                        }
                        else
                            MessageBox.Show("Please scan your respawn code in order to continue playing the game", "You are dead!", MessageBoxButton.OK);
                    }
                });

                _cam.Focus();
            }
            catch
            {
            }
        }

        private void OnRespawn(IAsyncResult e)
        {
            if (!Networking.FailedRequest)
            {
                //StoryboardRevive.Begin();
            }
        }


        private void DisplayBonus_Green(string text)
        {
            lbl_Bonus.Text = text;
            lbl_Bonus.Foreground = new SolidColorBrush(Color.FromArgb(255, 10, 255, 30));
            StoryboardBonusText.AutoReverse = true;
            StoryboardBonusText.FillBehavior = FillBehavior.Stop;
            StoryboardBonusText.Begin();
        }

        private void DisplayMessage_Green(string text)
        {
            lbl_Status.Text = text;
            lbl_Status.Foreground = new SolidColorBrush(Color.FromArgb(255, 10, 255, 30));
            StoryboardStatusText.AutoReverse = true;
            StoryboardStatusText.FillBehavior = FillBehavior.Stop;
            StoryboardStatusText.Begin();
        }
        private void DisplayMessage_Orange(string text)
        {
            lbl_Status.Text = text;
            lbl_Status.Foreground = new SolidColorBrush(Color.FromArgb(255, 150, 80, 10));
            StoryboardStatusText.AutoReverse = true;
            StoryboardStatusText.FillBehavior = FillBehavior.Stop;
            StoryboardStatusText.Begin();
        }
        private void DisplayMessage_Red(string text)
        {
            lbl_Status.Text = text;
            lbl_Status.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 30, 10));
            StoryboardStatusText.AutoReverse = true;
            StoryboardStatusText.FillBehavior = FillBehavior.Stop;
            StoryboardStatusText.Begin();
        }

        private void Button_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            _timerqsec.Start();
        }

        private void Button_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            _timerqsec.Stop();
        }

        private void Update()
        {
            timeleft--;
            var time = TimeSpan.FromSeconds(timeleft);

            Networking.Apitoken = Networking.Apitoken;

            //Display the hours only if there are any
            if (time.TotalHours < 1)
                lbl_TimeRemaining.Text = String.Format("{0:00}:{1:00}", (int)time.TotalMinutes, time.Seconds);
            else
                lbl_TimeRemaining.Text = String.Format("{0}:{1:00}:{2:00}", (int)time.TotalHours, (int)(time.TotalMinutes) % 60, time.Seconds);

            Networking.GET((o, evt) => OnGetAlive(evt), "/alive/" + myPlayer.id, true);

            //Only get team scores every 5 seconds
            if (timeleft % 5 == 0)
            {
                for (int i = 0; i < teams.Length; i++)
                {
                    Team x = teams[i];
                    Networking.GetTeamScores(teams[i].id, (o, e) => OnGetTeamScore(x));
                }
            }
        }

        private void OnGetAlive(DownloadStringCompletedEventArgs e)
        {
            if (!Networking.FailedRequest)
            {
                string q = e.Result;

                bool newalive = (bool)Networking.Results["alive"];

                if (!firstalive)
                {
                    if (myPlayer.alive)
                    {
                        if (!newalive)
                        {
                            Shader.Opacity = 0.3f;
                            Dispatcher.BeginInvoke(() =>
                                {
                                    MessageBox.Show("Please head to your base to respawn", "You Were Tagged!", MessageBoxButton.OK);
                                }
                            );
                        }
                    }
                    else
                    {
                        if (newalive)
                        {
                            Shader.Opacity = 0;
                            DisplayMessage_Green("Respawn Successful!");
                        }
                    }
                }

                firstalive = false;
                myPlayer.alive = newalive;
                string x = Networking.Results["alive"].ToString();
            }
        }

        private void OnGetGame()
        {
            if (!Networking.FailedRequest)
            {
                QRGame game = Networking.GetGameById_Local(myGameid) as QRGame;
                
                //Get the list of teams in the game
                teams = game.teams;

                //Get the time remaining
                timeleft = (int)((game.endTime.Ticks - DateTime.Now.Ticks) / 10000000);

                _timer1sec.Tick += (o, e) => Update();
            }
            else
            {
                Networking.GetGameById<QRGame>(myGameid, (o, e) => OnGetGame());
            }
        }

        private void OnGetTeamScore(Team team)
        {
            string score = Networking.Results["score"].ToString();
            if (team.id == myPlayer.team.id)
            {
                lbl_Team1Score.Text = score;
                lbl_Team1Score.Foreground = new SolidColorBrush(GetColourFromCharacter(team.reference_code));
            }
            else
            {
                lbl_Team2Score.Text = score;
                lbl_Team2Score.Foreground = new SolidColorBrush(GetColourFromCharacter(team.reference_code));
            }
        }

        private void btn_Powerup_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (!double.IsNaN(watcher.Position.Location.Latitude))
            {
                Networking.UpdateGPS(myPlayer.id, watcher.Position.Location.Latitude,
                   watcher.Position.Location.Longitude);
            }
            DisplayMessage_Red("You don't have any powerups!");
        }

        private Color GetColourFromCharacter(char ch)
        {
            switch (ch)
            {
                case 'B':
                    return Color.FromArgb(255, 50, 120, 255);
                case 'R':
                    return Color.FromArgb(255, 255, 50, 30);
                default:
                    return Color.FromArgb(255, 255, 255, 255);

            }
        }
    }
}