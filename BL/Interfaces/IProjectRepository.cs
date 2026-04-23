using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;


namespace ProjectbeheerBL.Interfaces {
    
    public interface IProjectRepository
    {
       
        Project GeefProject(int id);

       
        List<ProjectCombinatie> GeefAlleProjecten();

       
        List<Project> GeefProjectenMetFilters(string? type, string? wijk, ProjectStatus? status, DateTime? start, DateTime? eind, string? partner);


        int VoegProjectToe(Project project, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection conn, IDbTransaction trans);

        List<string> GeefBeschikbareFaciliteiten();

        
        List<string> GeefBouwfirmas();

       
        List<string> GeefWoonvormTypes();

        bool BestaatBouwfirma(string naam, IDbConnection conn, IDbTransaction trans);

        int? HaalBouwfirmaIdOp(string naam, IDbConnection con, IDbTransaction transaction);

    }
   
}
