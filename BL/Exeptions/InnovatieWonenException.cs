using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class InnovatieWonenException : Exception
    {
        public InnovatieWonenException()
        {
        }

        public InnovatieWonenException(string? message) : base(message)
        {
        }

        public InnovatieWonenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}