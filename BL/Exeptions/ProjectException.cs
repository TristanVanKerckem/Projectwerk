using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class ProjectException : Exception
    {
        public ProjectException()
        {
        }

        public ProjectException(string? message) : base(message)
        {
        }

        public ProjectException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}