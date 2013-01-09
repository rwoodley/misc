using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace AVICaptureTest
{
    class GifMaker
    {
        // see: http://bloggingabout.net/blogs/rick/archive/2005/05/10/3830.aspx

        static FileStream _fileStream;
        static BinaryWriter _binaryWriter;
        static MemoryStream _memoryStream;
        static Byte[] buf2;
        static Byte[] buf3;
        static bool _firstTime = true;
        public static void Init(String fileName)
        {
            setupBufs();
            _firstTime = true;
            System.Diagnostics.Debug.WriteLine("Start writing gif.");
            _memoryStream = new MemoryStream();
            _fileStream = new FileStream(fileName, FileMode.Create);
            _binaryWriter = new BinaryWriter(_fileStream);
        }
        public static void SaveImage(Image image)
        {
            Byte[] buf1;
            image.Save(_memoryStream, ImageFormat.Gif);
            buf1 = _memoryStream.ToArray();

            if (_firstTime)
            {
                //only write these the first time....
                _binaryWriter.Write(buf1, 0, 781); //Header & global color table
                _binaryWriter.Write(buf2, 0, 19); //Application extension
            }
            _firstTime = false;

            _binaryWriter.Write(buf3, 0, 8); //Graphic extension
            _binaryWriter.Write(buf1, 789, buf1.Length - 790); //Image data

            _memoryStream.SetLength(0);
        }
        public static void FinishUp()
        {
            System.Diagnostics.Debug.WriteLine("Done writing gif.");
            //only write this one the last time....
            _binaryWriter.Write(";"); //Image terminator
            System.Threading.Thread.Sleep(200);
            _binaryWriter.Close();
            _fileStream.Close();
        }
        public static void setupBufs()
        {
            buf2 = new Byte[19];
            buf3 = new Byte[8];
            buf2[0] = 33;  //extension introducer
            buf2[1] = 255; //application extension
            buf2[2] = 11;  //size of block
            buf2[3] = 78;  //N
            buf2[4] = 69;  //E
            buf2[5] = 84;  //T
            buf2[6] = 83;  //S
            buf2[7] = 67;  //C
            buf2[8] = 65;  //A
            buf2[9] = 80;  //P
            buf2[10] = 69; //E
            buf2[11] = 50; //2
            buf2[12] = 46; //.
            buf2[13] = 48; //0
            buf2[14] = 3;  //Size of block
            buf2[15] = 1;  //
            buf2[16] = 0;  //
            buf2[17] = 0;  //
            buf2[18] = 0;  //Block terminator
            buf3[0] = 33;  //Extension introducer
            buf3[1] = 249; //Graphic control extension
            buf3[2] = 4;   //Size of block
            buf3[3] = 9;   //Flags: reserved, disposal method, user input, transparent color
            //buf3[4] = 10;  //Delay time low byte
            //buf3[5] = 3;   //Delay time high byte
            buf3[4] = 30;  //Delay time low byte
            buf3[5] = 0;   //Delay time high byte
            buf3[6] = 255; //Transparent color index
            buf3[7] = 0;   //Block terminator
        }
    }
}
