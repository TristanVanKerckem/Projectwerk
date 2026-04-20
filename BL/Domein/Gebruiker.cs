using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Gebruiker
    {
        public string Naam {  get; set; }
        public string Email { get; set; }
        public bool IsBeheerder { get; set; }
        public Gebruiker(string naam, string email, bool isBeheerder)
        {
            Naam = naam;
            Email = email;
            IsBeheerder = isBeheerder;
        }

    }
}
