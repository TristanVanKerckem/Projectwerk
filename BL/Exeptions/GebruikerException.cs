using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class GebruikerException : Exception
    {
        public GebruikerException()
        {
        }

        public GebruikerException(string? message) : base(message)
        {
        }

        public GebruikerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}