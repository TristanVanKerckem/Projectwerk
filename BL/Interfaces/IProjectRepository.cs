using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProjectbeheerBL.Interfaces {
    //public interface IProjectRepository 
    //{
    //    //Project GeefProject(int id);
    //    //List<Project> GeefAlleProjecten();
    //    //void VoegProjectToe(Project project);
    //    //Voeg hier methodes toe voor specifieke filters indien nodig


    //    List<ProjectCombinatie> GeefProjectCombinaties();
    //    ProjectCombinatie GeefProject(int id);
    //    List<ProjectCombinatie> GeefAlleProjecten();
    //    void VoegProjectToe(Project project);
    //}
    public interface IProjectRepository
    {
       
        Project GeefProject(int id);

       
        List<ProjectCombinatie> GeefAlleProjecten();

       
        List<Project> GeefProjectenMetFilters(string? type, string? wijk, ProjectStatus? status, DateTime? start, DateTime? eind, string? partner);


        void VoegProjectToe(Project project);

        //Project GeefProject(int id); // moet nog ProjectCombinatie worden?

        //List<Project> GeefAlleProjecten(); // Moet nog List van ProjectCombinatie worden
    }
   
}
