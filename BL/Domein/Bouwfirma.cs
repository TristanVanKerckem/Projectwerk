using ProjectbeheerBL.Exeptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Bouwfirma
    {
        private string _naam;
        private string _email;
        private string _telefoonNummer;
        public string Naam
        {
            get { return _naam; } 
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new BouwfirmaException("Naam mag niet leeg of null zijn");
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
                    throw new BouwfirmaException("email mag niet leeg of null zijn");
                if (!value.Contains("@") || !value.Contains("."))
                    throw new BouwfirmaException("Email moet een @ en . hebben");
                _email = value;
            }
        }
        public string TelefoonNummer
        {
            get { return _telefoonNummer; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new BouwfirmaException("Telefoonnummer mag niet leeg of null zijn");

                if (!value.All(char.IsDigit))
                    throw new BouwfirmaException("Telefoonnummer mag alleen cijfers bevatten");
                _telefoonNummer = value;
            }
        }

        public Bouwfirma(string naam, string email, string telefoonNummer)
        {
            Naam = naam;
            Email = email;
            TelefoonNummer = telefoonNummer;
        }
        public override string ToString()
        {
            return $"{_naam}, Email: {_email}, Telefoonnummer: {_telefoonNummer}";
        }
    }
}
