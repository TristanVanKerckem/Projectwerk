using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class ProjectPartnerException : Exception
    {
        public ProjectPartnerException() { }



        public ProjectPartnerException(string? message) : base(message)
        {
        }

        public ProjectPartnerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}