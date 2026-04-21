using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;


namespace ProjectbeheerBL.Interfaces {
    public interface ICSVschrijver 
    {
        // void SchrijfProjectenNaarCSV(List<Project> projecten, string pad);
         void MaakCSV(List<ProjectCombinatie> projecten, string basisPad);
    }
}
