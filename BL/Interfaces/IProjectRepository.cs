using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;


namespace ProjectbeheerBL.Interfaces {
    public interface IProjectRepository 
    {
        //Project GeefProject(int id);
        //List<Project> GeefAlleProjecten();
        //void VoegProjectToe(Project project);
        //Voeg hier methodes toe voor specifieke filters indien nodig

        List<ProjectCombinatie> GeefProjectCombinaties();
        void VoegProjectToe(Project project);
    }
}
