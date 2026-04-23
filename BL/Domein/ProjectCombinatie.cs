using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class ProjectCombinatie
    {
        public int Id { get; set; }
        public Gebruiker Gebruiker { get; set; }
        public List<Project> ProjectComboLijst { get; set; } = new List<Project>();
        public ProjectCombinatie() 
        {
            ProjectComboLijst = new List<Project>(); 
        }

        public string ReadFiche(List<Project> projectComboLijst)
        {

            foreach (Project project in projectComboLijst)
            {

            }
            return $"Fiche bevat {projectComboLijst.Count} projecten voor gebruiker {Gebruiker?.Naam}.";
        }
        public ProjectCombinatie(int id)
        {
            Id = id ;
        }
        public void VoegProjectToe(Project project)
        {
            ProjectComboLijst.Add(project);
        }
        public ProjectCombinatie(Project project)
        {
            ProjectComboLijst.Add(project);
        }
        public ProjectCombinatie(List<Project> projectComboLijst)
        {
            ProjectComboLijst = projectComboLijst;
        }
    }
}