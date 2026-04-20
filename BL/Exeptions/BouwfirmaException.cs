using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class BouwfirmaException : Exception
    {
        public BouwfirmaException()
        {
        }

        public BouwfirmaException(string? message) : base(message)
        {
        }

        public BouwfirmaException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}