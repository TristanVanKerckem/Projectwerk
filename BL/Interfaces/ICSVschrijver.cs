using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;


namespace ProjectbeheerBL.Interfaces {
    public interface ICSVschrijver 
    {
        public byte[] SchrijfProjectenNaarCSV(List<ProjectCombinatie> projecten);
    }
}
