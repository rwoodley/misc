using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using AVICapWrapper;

namespace AVICaptureTest
{
    public partial class Form1 : Form
    {
        Size _size = new Size(640, 480);
        int _frameRate = 10;
        public Form1()
        {
            InitializeComponent();

            aviCapture1.SetFrameRate(_frameRate);
            aviCapture1.SetVideoFormat(_size);
            aviCapture1.OnFrameReveived += new AVICapture.FrameCallbackEvent( this.FrameCallbackHandler );
            aviCapture1.StartStreaming();
            pictureBox1.Focus();
        }
        String _filename = "Snap_0.jpg";
        private void FrameCallbackHandler( IntPtr handleCapture, IntPtr videoHeader )
        {
            if (!_stopSnapping)
            {
                Bitmap bmp = aviCapture1.GetBitmapFromUnmanagedPtr(videoHeader);
                while (System.IO.File.Exists(_filename)) {
                    _snapUKey++;
                    _filename = String.Format("Snap_{0}.jpg", _snapUKey);
                }
                System.Diagnostics.Debug.WriteLine("Writing " + _filename);
                bmp.Save(_filename, ImageFormat.Jpeg);
                pictureBox1.Image = bmp;
                _stopSnapping = true;
                this.Text = _snapUKey.ToString();
            }

        }

        public delegate void Action();
        bool _stopSnapping = true;
        int _snapUKey = 0;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            _stopSnapping = false;
        }
    }
}