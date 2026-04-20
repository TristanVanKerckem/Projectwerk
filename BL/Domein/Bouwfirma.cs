using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Bouwfirma
    {
        public string Naam { get; set; }
        public string Email { get; set; }
        public string TelefoonNummer { get; set; }

        public Bouwfirma(string naam, string email, string telefoonNummer)
        {
            Naam = naam;
            Email = email;
            TelefoonNummer = telefoonNummer;
        }
    }
}
