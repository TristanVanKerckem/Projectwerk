using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;


namespace ProjectbeheerBL.Interfaces {
    public interface IProjectRepository 
    {
        //Project GeefProject(int id);
        //List<Project> GeefAlleProjecten();
        //void VoegProjectToe(Project project);
        //Voeg hier methodes toe voor specifieke filters indien nodig

        int VoegProjectToe(Project project, IDbConnection conn, IDbTransaction trans);

        Project GeefProject(int id); // moet nog ProjectCombinatie worden?

        List<Project> GeefAlleProjecten(); // Moet nog List van ProjectCombinatie worden
    }
}
