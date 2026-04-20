using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Partner
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public string Email { get; set; }
        public Locatie Vestiging { get; set; }
        public List<ProjectPartner> Projecten { get; set; } = new List<ProjectPartner>();

        public Partner(string naam, string email, Locatie vestiging)
        {
            Naam = naam;
            Email = email;
            Vestiging = vestiging;
        }

        public Partner() { }
    }
}
