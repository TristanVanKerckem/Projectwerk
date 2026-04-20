using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Exeptions
{
    
        public class PartnerException : Exception
        {
            public PartnerException()
            {
            }

            public PartnerException(string? message) : base(message)
            {
            }

            public PartnerException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
    }
