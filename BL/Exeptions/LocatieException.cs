using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class LocatieException : Exception
    {
        public LocatieException()
        {
        }

        public LocatieException(string? message) : base(message)
        {
        }

        public LocatieException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}