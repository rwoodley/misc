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
        public Form1()
        {
            InitializeComponent();

            videoFormatBox.Items.Add( new Size( 160, 120 ));
            videoFormatBox.Items.Add( new Size( 320, 240 ));
            videoFormatBox.Items.Add( new Size( 640, 480 ));
            videoFormatBox.SelectedIndex = 1;
            //videoFormatBox_SelectedIndexChanged(null, null);
            Size newSize = (Size)videoFormatBox.Items[videoFormatBox.SelectedIndex];
            cameraGroupBox.Size = new Size(newSize.Width + 15, newSize.Height + 30);

            videoFormatBox.SelectedIndexChanged += new EventHandler( videoFormatBox_SelectedIndexChanged );

            frameRateBar.Value = 10;
            frameRateBar_Scroll( this, new EventArgs() );
            
            aviCapture1.SetVideoFormat(( Size ) videoFormatBox.Items[videoFormatBox.SelectedIndex] );
            aviCapture1.OnFrameReveived += new AVICapture.FrameCallbackEvent( this.FrameCallbackHandler );
            aviCapture1.StartStreaming();

            frameRateBar.Value = 10;
        }

        void videoFormatBox_SelectedIndexChanged( object sender, EventArgs ee )
        {
            Size newSize = ( Size ) videoFormatBox.Items[videoFormatBox.SelectedIndex];

            aviCapture1.StopStreaming();
            aviCapture1.SetVideoFormat( newSize );
            
            // Setting surrounding Group Box Size
            cameraGroupBox.Size = new Size( newSize.Width + 15, newSize.Height + 30 );

            aviCapture1.StartStreaming();
        }

        private void FrameCallbackHandler( IntPtr handleCapture, IntPtr videoHeader )
        {
            Bitmap bmp = aviCapture1.GetBitmapFromUnmanagedPtr( videoHeader );

            if (!_stopSnapping)
            {
                GifMaker.SaveImage(bmp);
                String filename = String.Format("Snap_{0}_{1}.tiff", _seriesUKey, _snapUKey++);
                if (_takeThumbnail)
                {
                    System.Diagnostics.Debug.WriteLine("Writing " + filename);
                    bmp.Save(filename, ImageFormat.Tiff);
                    _takeThumbnail = false;
                }
                
                System.Threading.Thread.Sleep(150);
            }

        }

        private void frameRateBar_Scroll( object sender, EventArgs e )
        {
            textBox1.Text = frameRateBar.Value.ToString();
            aviCapture1.SetFrameRate( frameRateBar.Value );
        }
        public delegate void Action();
        bool _stopSnapping = true;
        bool _takeThumbnail = false;
        int _seriesUKey = 0;
        int _snapUKey = 0;
        private void GoButton_Click(object sender, EventArgs e)
        {
            _stopSnapping = true; _takeThumbnail = false;
            Action runMotorAction = () =>
            {
                System.Diagnostics.Debug.WriteLine("Start Delegate");
                BrickControl.RunForward();
            };
            String fileName = "";
            while (fileName == "" || System.IO.File.Exists(fileName))
            {
                _seriesUKey++;
                fileName = "Animation_" + _seriesUKey.ToString() + ".gif";
                System.Diagnostics.Debug.WriteLine("Trying " + fileName);
            }
            _snapUKey = 0;
            _stopSnapping = false; _takeThumbnail = true;
            runMotorAction.BeginInvoke(
                (c) =>
                {
                    _stopSnapping = true;
                    GifMaker.FinishUp();
                    System.Diagnostics.Debug.WriteLine("Calling EndInvoke");
                    runMotorAction.EndInvoke((IAsyncResult)c);
                }, 
                null);
            GifMaker.Init(fileName);
        }
    }
}