using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    public class StadsontwikkelingException : Exception 
    {
       
            public StadsontwikkelingException()
            {
            }

            public StadsontwikkelingException(string? message) : base(message)
            {
            }

            public StadsontwikkelingException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }

