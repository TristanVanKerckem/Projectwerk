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
        public ProjectStatus Status { get; set; }
        public Locatie Locatie { get; set; }
        public Dictionary<Partner, List<string>> ProjectPartners { get; set; } = new Dictionary<Partner, List<string>>();

        protected Project(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie, Dictionary<Partner, List<String>> projectPartners)
        {
            Titel = titel;
            StartDatum = startDatum;
            Beschrijving = beschrijving;
            Status = status;
            Locatie = locatie;
            ProjectPartners = projectPartners;
        }
        protected Project(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie)
        {
            Titel = titel;
            StartDatum = startDatum;
            Beschrijving = beschrijving;
            Status = status;
            Locatie = locatie;
        }
        public void VoegPartnerToe(Partner partner, List<string> rollen)
        {
            if (!ProjectPartners.ContainsKey(partner))
            {
                ProjectPartners[partner] = new List<string>();
            }
            foreach (var rol in rollen)
            {
                if (!ProjectPartners[partner].Contains(rol))
                {
                    ProjectPartners[partner].Add(rol);
                }
            }
        }
        public void VoegRolToe(Partner partner, string rol)
        {
            if (!ProjectPartners[partner].Contains(rol))
            {
                ProjectPartners[partner].Add(rol);
            }
        }
    }
}
