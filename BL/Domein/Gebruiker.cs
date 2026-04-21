using ProjectbeheerBL.Exeptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Gebruiker
    {
        private int _id;
        private string _naam;
        private string _email;
        private bool _isBeheerder;

        public int Id
        {
            get { return _id; }
            set
            {
                if (value <= 0)
                    throw new GebruikerException("Id mag niet nul of negatief zijn");
                _id = value;
            }
        }
        public string Naam
        {
            get { return _naam; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new GebruikerException("Naam mag niet leeg of null zijn");
                else
                    _naam = value;
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new GebruikerException("email mag niet leeg of null zijn");
                if (!value.Contains("@") || !value.Contains("."))
                    throw new GebruikerException("Email moet een @ en . hebben");
                _email = value;
            }
        }
        public bool IsBeheerder { get; set; }
        public Gebruiker(string naam, string email, bool isBeheerder)
        {
            Naam = naam;
            Email = email;
            IsBeheerder = isBeheerder;
        }


    }
}
