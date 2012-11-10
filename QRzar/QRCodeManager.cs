using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using com.google.zxing;
using com.google.zxing.qrcode;
using WP7_Barcode_Library;
using System.Collections.Generic;

namespace QRzar
{
    public static class QRCodeManager
    {
        public static bool IsValidQRCode(string qrcode)
        {
            return (qrcode.Length == 6
                && (qrcode[0] == 'R'
                    || qrcode[0] == 'G'
                    || qrcode[0] == 'B'
                    || qrcode[0] == 'Y')
                );
        }

        public static bool IsValidRespawnCode(string qrcode)
        {
            return qrcode.Length == 6;
        }
    }


    public class PhotoCameraLuminanceSource : LuminanceSource
    {
        public byte[] PreviewBufferY { get; private set; }
        public new bool CropSupported { get; protected set; }
        public new bool RotateSupported { get; protected set; }

        public PhotoCameraLuminanceSource(int width, int height)
            : base(width, height)
        {
            PreviewBufferY = new byte[width * height];
            CropSupported = true;
            RotateSupported = true;
        }

        public override sbyte[] Matrix
        {
            get { return (sbyte[])(Array)PreviewBufferY; }
        }

        public override sbyte[] getRow(int y, sbyte[] row)
        {
            if (row == null || row.Length < Width)
            {
                row = new sbyte[Width];
            }

            for (int i = 0; i < Height; i++)
                row[i] = (sbyte)PreviewBufferY[i * Width + y];

            return row;
        }
    }
}
