using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Partner
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public Locatie Vestiging { get; set; }
        public List<Project> Projecten { get; set; } = new List<Project>();

        public Partner(string naam, string email, Locatie vestiging)
        {
            Naam = naam;
            Email = email;
            Vestiging = vestiging;
        }

        public Partner() { }
    }
}
