using System;
using System.Collections.Generic;
using System.Text;

namespace AVICapWrapper
{
    public class AVIException : Exception
    {
        public AVIException( string message ) : base( message )
        {
        }
    }
}
