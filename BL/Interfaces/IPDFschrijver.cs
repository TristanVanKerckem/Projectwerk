using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Interfaces {
    public interface IPDFschrijver 
    {
        // Deze methode genereert een PDF-fiche voor een specifiek project
        //void SchrijfProjectenFiche(List<Project> projecten, string pad);

        

        // Methode voor een enkele fiche (geeft bytes terug)
        byte[] MaakPDF(ProjectCombinatie project);

        // Methode voor een lijst van projecten (indien nodig)
        byte[] MaakPDF(List<ProjectCombinatie> projecten);

        // Optioneel: Een methode voor de ProjectCombinatie zoals in je ontwerp
        //void SchrijfCombinatieFiche(ProjectCombinatie combinatie, string pad);
        //void SchrijfProjectenFiche(Project project, string pad);
    }
}
