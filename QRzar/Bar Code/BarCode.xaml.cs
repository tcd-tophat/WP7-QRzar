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
using Microsoft.Devices;
using com.google.zxing;
using com.google.zxing.oned;
using System.Windows.Threading;
using System.Threading;
using System.Diagnostics;

namespace QRzar.Bar_Code
{
    public partial class BarCode : PhoneApplicationPage
    {
        //initializer
        PhotoCamera _cam;
        VideoBrush _videoBrush = new VideoBrush();
        byte[] _buffer;
        Stopwatch watch = new Stopwatch();
        int _nbTry;
        Result result = null;

        //initialize screen
        public BarCode()
        {
            InitializeComponent();
        }
        //initialize the camera when screen is navigatedTo
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _nbTry = 0;
            _cam = new PhotoCamera();

            _cam.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
            _cam.AutoFocusCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_AutoFocusCompleted);
            video.Fill = _videoBrush;
            _videoBrush.SetSource(_cam);

            base.OnNavigatedTo(e);
        }
        //ends the camera screen and dipsoses of handlers and camera
        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _cam.CancelFocus();
            _cam.Dispose();

            base.OnNavigatingFrom(e);
        }
        //camera setup
        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            _cam.FlashMode = FlashMode.Auto;
            _cam.Focus();
        }
        //takes the video input and creates a bitmap of the image
        BinaryBitmap GetBitmapFromVideo(PhotoCamera cam)
        {
            BinaryBitmap binaryBitmap = null;

            try
            {
                // Update buffer size    
                var pixelWidth = (int)_cam.PreviewResolution.Width;
                var pixelHeight = (int)_cam.PreviewResolution.Height;

                if (_buffer == null || _buffer.Length != (pixelWidth * pixelHeight))
                {
                    _buffer = new byte[pixelWidth * pixelHeight];
                }

                _cam.GetPreviewBufferY(_buffer);

                var luminance = new RGBLuminanceSource(_buffer, pixelWidth, pixelHeight, true);
                var binarizer = new com.google.zxing.common.HybridBinarizer(luminance);

                binaryBitmap = new BinaryBitmap(binarizer);
            }
            catch
            {
            }

            return binaryBitmap;
        }
        //searches for QRCode in bitmap
        void cam_AutoFocusCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            if (result == null)
            { 
            try
            {
                _nbTry++;
                watch.Reset();
                watch.Start();

                while ((result == null) && (watch.ElapsedMilliseconds < 1500))
                {
                    var binaryBitmap = GetBitmapFromVideo(_cam);
                    if (binaryBitmap != null)
                    {
                        try
                        {
                            //decodes QRcode
                            result = BarCodeManager.ZXingReader.decode(binaryBitmap);
                        }
                        catch
                        {
                            // Wasn't able to find a barcode
                        }
                    }
                }

                if (result != null)
                {
                    BarCodeManager._onBarCodeFound(result.Text);
                }
                else
                {
                    // Try to focus again
                    if (_nbTry < BarCodeManager.MaxTry)
                    {
                        _cam.Focus();
                    }
                    else
                    {
                        BarCodeManager._onError(new VideoScanException("Nothing was found during the scan"));
                    }
                }
            }
            catch (Exception exc)
            {
                BarCodeManager._onError(exc);
            }
        }
      }
        //prevents the back button from working as it crashes the app due to an incorrect closing of the screen. 
        //if you want to go back or add a back button to this screen you will need to figure out how to close it safely.
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }  

   }
}