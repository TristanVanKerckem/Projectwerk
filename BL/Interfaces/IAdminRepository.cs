using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Interfaces {
    public interface IAdminRepository {
        void VoegGebruikerToe(Gebruiker g);
        void VerwijderProject(Project project);
        void UpdateInformatieProject(Project project, ProjectCombinatie projecten);
        void VerwijderPartnerVanProject(Project project);
    }
}
