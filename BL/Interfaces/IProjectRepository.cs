using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProjectbeheerBL.Interfaces {
    
    public interface IProjectRepository
    {
       
        Project GeefProject(int id);

       
        List<ProjectCombinatie> GeefAlleProjecten();

       
        List<Project> GeefProjectenMetFilters(string? type, string? wijk, ProjectStatus? status, DateTime? start, DateTime? eind, string? partner);


        void VoegProjectToe(Project project);

    }
   
}
