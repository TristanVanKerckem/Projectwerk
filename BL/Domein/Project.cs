using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public abstract class Project
    {
        public int Id { get; set; }
        public string Titel { get; set; }
        public DateTime StartDatum { get; set; }
        public string Beschrijving { get; set; }
        public ProjectStatus Status { get; set; } // Aangepast van Status
        public Locatie Locatie { get; set; }
        public List<ProjectPartner> ProjectPartners { get; set; } = new List<ProjectPartner>();

        protected Project(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie, List<ProjectPartner> projectPartners = null)
        {
            Titel = titel;
            StartDatum = startDatum;
            Beschrijving = beschrijving;
            Status = status;
            Locatie = locatie;
            this.ProjectPartners = ProjectPartners ?? new List<ProjectPartner>();
        }
    }
}
