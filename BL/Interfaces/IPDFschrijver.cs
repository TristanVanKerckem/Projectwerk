using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Interfaces {
    public interface IPDFschrijver 
    {
        // Deze methode genereert een PDF-fiche voor een specifiek project
        //void SchrijfProjectenFiche(List<Project> projecten, string pad);

        void MaakPDF(List<ProjectCombinatie> projecten, string pad);
        void MaakPDFEenProject(ProjectCombinatie project, string pad);

        // Optioneel: Een methode voor de ProjectCombinatie zoals in je ontwerp
        //void SchrijfCombinatieFiche(ProjectCombinatie combinatie, string pad);
        //void SchrijfProjectenFiche(Project project, string pad);
    }
}
