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
        Project GeefProject(int id);
        List<ProjectCombinatie> GeefAlleProjecten();
        void VoegProjectToe(Project project);
    }
}
