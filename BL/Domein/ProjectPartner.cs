using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class ProjectPartner
    {
        public int Id { get; set; }
        public List<string> Rollen { get; set; } = new List<string>();
        public Partner Partner { get; set; }
        public Project Project { get; set; }

        public ProjectPartner(List<string> rollen, Partner partner, Project project)
        {
            Rollen = rollen;
            Partner = partner;
            Project = project;
        }

        public ProjectPartner() { }

        public void VoegRolToe(string rol)
        {
            if (!string.IsNullOrWhiteSpace(rol) && !Rollen.Contains(rol))
            {
                Rollen.Add(rol);
            }
        }
    }
}
