using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Interfaces {
    
    public interface IPDFschrijver
    {
        
        byte[] MaakPDF(List<Project> projecten);

        byte[] MaakPDF(ProjectCombinatie project);
        byte[] MaakPDF(List<ProjectCombinatie> projecten);
    }
}
