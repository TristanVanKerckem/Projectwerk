using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class GroeneRuimteException : Exception
    {
        public GroeneRuimteException()
        {
        }

        public GroeneRuimteException(string? message) : base(message)
        {
        }

        public GroeneRuimteException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}