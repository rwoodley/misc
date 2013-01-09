using System;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AVICapWrapper
{
    public class AVICapture : Panel
    {
        public const uint WS_VISIBLE = 0x10000000;
        public const uint WS_CHILD = 0x40000000;
        public const uint WM_CAP_DRIVER_CONNECT = 0x40A;
        public const uint WM_CAP_DRIVER_DISCONNECT = 0x40B;
        public const uint WM_CAP_SET_PREVIEWRATE = 0x434;
        public const uint WM_CAP_SET_PREVIEW = 0x432;
        public const uint WM_CAP_SET_CALLBACK_FRAME = 0x405;
        public const uint WM_CAP_SET_VIDEOFORMAT = 0x42D;

        private int m_DriverIndex;
        private int m_FrameDelay;
        private IntPtr m_HandleCaptureWindow;
        private BITMAPINFO m_VideoFormat;

        // Events
        public event FrameCallbackEvent OnFrameReveived;

        // Delegate for Frame Callback
        public delegate void FrameCallbackEvent( IntPtr handleCapture, IntPtr videoHeader );
        public FrameCallbackEvent m_FrameCallback;

        [DllImport( "avicap32.dll", SetLastError=true )]
        public static extern IntPtr capCreateCaptureWindowW( string windowTitle, uint style, int x, int y, int width, int height, IntPtr parentWindowHandle, int id );

        [DllImport( "user32.dll", SetLastError=true )]
        public static extern int SendMessage( IntPtr handleWindow, uint message, ushort wParam, ushort lParam );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern int SendMessage( IntPtr handleWindow, uint message, ushort wParam, FrameCallbackEvent callback );

        [DllImport( "user32.dll", SetLastError = true )]
        public static extern int SendMessage( IntPtr handleWindow, uint message, int wParam, ref BITMAPINFO videoformat );

        [DllImport( "avicap32.dll", SetLastError=true )]
        public static extern bool capGetDriverDescription( int index, StringBuilder driverName, int driverNameLen, StringBuilder driverDescr, int driverDescrLen );

        /*
        ** Constructors
        */
        public AVICapture() : this( 0, 10, new Size( 320, 240 ))
        {
        }

        public AVICapture( int driverIndex, int frameRate, Size videoSize )
        {
            SetVideoFormat( videoSize );
            SetFrameRate( frameRate );
            m_DriverIndex = driverIndex;
        }

        public void SetFrameRate( int frameRate )
        {
            m_FrameDelay = ( int ) 1000 / frameRate; 
            if( m_HandleCaptureWindow != IntPtr.Zero )
            {
                SendMessage( m_HandleCaptureWindow, WM_CAP_SET_PREVIEWRATE, ( ushort ) m_FrameDelay, 0 );
            }
        }

        public void SetVideoFormat( Size videoSize )
        {
            this.Size = videoSize;
            m_VideoFormat = new BITMAPINFO();
            m_VideoFormat.bmiHeader.biSize = Marshal.SizeOf( m_VideoFormat.bmiHeader );
            m_VideoFormat.bmiHeader.biWidth = videoSize.Width;
            m_VideoFormat.bmiHeader.biHeight = videoSize.Height;
            m_VideoFormat.bmiHeader.biPlanes = 1;
            m_VideoFormat.bmiHeader.biBitCount = 24;
        }

        public static List<AVIDriver> GetInstalledDrivers()
        {
            List<AVIDriver> drivers = new List<AVIDriver>();

            for( int index = 0; index < 10; index ++ )
            {
                StringBuilder driverName = new StringBuilder( 100 );
                StringBuilder driverDescr = new StringBuilder( 100 );
                if( capGetDriverDescription( index, driverName, driverName.Capacity, driverDescr, driverDescr.Capacity ))
                {
                    AVIDriver driver = new AVIDriver();
                    driver.DriverIndex = index;
                    driver.DriverName = driverName.ToString();
                    driver.DriverDescription = driverDescr.ToString();
                    drivers.Add( driver );
                }
            }

            return drivers;
        }

        public void StartStreaming()
        {
            Connect(); // Connecting the Video for Windows Driver to our Window

            SendMessage( m_HandleCaptureWindow, WM_CAP_SET_PREVIEWRATE, ( ushort ) m_FrameDelay, 0 );
            SendMessage( m_HandleCaptureWindow, WM_CAP_SET_PREVIEW, 1, 0 );

            // Setting up Video Format
            AVICapture.SendMessage( m_HandleCaptureWindow, AVICapture.WM_CAP_SET_VIDEOFORMAT, Marshal.SizeOf( m_VideoFormat ), ref m_VideoFormat );

            // Setting up Event Handlers
            m_FrameCallback = new FrameCallbackEvent( this.FrameCallbackHandler );
            SendMessage( m_HandleCaptureWindow, WM_CAP_SET_CALLBACK_FRAME, 0, m_FrameCallback );
        }

        public void StopStreaming()
        {
            Disconnect();
        }

        private void Disconnect()
        {
            if( m_HandleCaptureWindow != IntPtr.Zero )
            {
                SendMessage( m_HandleCaptureWindow, WM_CAP_DRIVER_DISCONNECT, 0, 0 );
            }
        }

        private void Connect()
        {
            if( ( m_HandleCaptureWindow = capCreateCaptureWindowW( "Capture", ( AVICapture.WS_CHILD | AVICapture.WS_VISIBLE ), 0, 0, m_VideoFormat.bmiHeader.biWidth, m_VideoFormat.bmiHeader.biHeight, this.Handle, 0 ) ) == IntPtr.Zero )
            {
                throw( new AVIException( "Failed to create capture Window" ));
            }

            SendMessage( m_HandleCaptureWindow, WM_CAP_DRIVER_CONNECT, ( ushort ) m_DriverIndex, 0 );
        }

        public Bitmap GetBitmapFromUnmanagedPtr( IntPtr videoHeader )
        {
            VIDEOHDR vhdr = new VIDEOHDR();
            vhdr = ( VIDEOHDR ) Marshal.PtrToStructure( videoHeader, vhdr.GetType() );

            byte[] imageData = new byte[vhdr.dwBytesUsed];
            Marshal.Copy( new IntPtr( vhdr.lpData ), imageData, 0, imageData.Length );

            System.Drawing.Bitmap bmp = new Bitmap( m_VideoFormat.bmiHeader.biWidth, m_VideoFormat.bmiHeader.biHeight );

            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits( new Rectangle( 0, 0, m_VideoFormat.bmiHeader.biWidth, m_VideoFormat.bmiHeader.biHeight ), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb );

            int startAddr = bmpData.Scan0.ToInt32();
            for( int row = m_VideoFormat.bmiHeader.biHeight-1; row >= 0; row -- )
            {                                
                // ** Woot! Dirty trick, may not work on all platforms.... egrath
                Marshal.Copy( imageData, ( row * m_VideoFormat.bmiHeader.biWidth * 3 ), new IntPtr( startAddr ), m_VideoFormat.bmiHeader.biWidth * 3 );
                startAddr += m_VideoFormat.bmiHeader.biWidth * 3;
            }
            bmp.UnlockBits( bmpData );

            return bmp;
        }

        private void FrameCallbackHandler( IntPtr handleCapture, IntPtr videoHeader )
        {
            if( OnFrameReveived != null )
            {
                OnFrameReveived( handleCapture, videoHeader );
            }
        }
    }

    public struct AVIDriver
    {
        public int DriverIndex;
        public string DriverName;
        public string DriverDescription;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct VIDEOHDR
    {
        [MarshalAs( UnmanagedType.I4 )]
        public int lpData;
        [MarshalAs( UnmanagedType.I4 )]
        public int dwBufferLength;
        [MarshalAs( UnmanagedType.I4 )]
        public int dwBytesUsed;
        [MarshalAs( UnmanagedType.I4 )]
        public int dwTimeCaptured;
        [MarshalAs( UnmanagedType.I4 )]
        public int dwUser;
        [MarshalAs( UnmanagedType.I4 )]
        public int dwFlags;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 4 )]
        public int[] dwReserved;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct BITMAPINFOHEADER
    {
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biSize;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biWidth;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biHeight;
        [MarshalAs( UnmanagedType.I2 )]
        public short biPlanes;
        [MarshalAs( UnmanagedType.I2 )]
        public short biBitCount;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biCompression;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biSizeImage;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biXPelsPerMeter;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biYPelsPerMeter;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biClrUsed;
        [MarshalAs( UnmanagedType.I4 )]
        public Int32 biClrImportant;
    }

    [StructLayout( LayoutKind.Sequential )]
    public struct BITMAPINFO
    {
        [MarshalAs( UnmanagedType.Struct, SizeConst = 40 )]
        public BITMAPINFOHEADER bmiHeader;
        [MarshalAs( UnmanagedType.ByValArray, SizeConst = 1024 )]
        public Int32[] bmiColors;
    }
}
