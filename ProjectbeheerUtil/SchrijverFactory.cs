using ProjectbeheerBL.Interfaces;
using ProjectbeheerDL.Schrijver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectbeheerUtil
{


    public static class SchrijverFactory
    {
        public static IPDFschrijver GeefPDFProcessor(string fileType)
        {
            if (fileType.ToLower() == "pdf")
                
                return new PDFSchrijver();
            return null;
        }

        public static ICSVschrijver GeefCSVProcessor(string fileType)
        {
            if (fileType.ToLower() == "csv")
               
                return new CSVschrijver();
            return null;
        }
    }

}
