using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Locatie
    {
        public int Id { get; set; }
        public string Wijk { get; set; }
        public string Straat { get; set; }
        public string Gemeente { get; set; }
        public int Postcode { get; set; }
        public string HuisNummer { get; set; }

       
        public Locatie() { } 

        public Locatie(int id, string wijk, string straat, string gemeente, int postcode, string huisNummer)
        {
            Id = id;
            Wijk = wijk;
            Straat = straat;
            Gemeente = gemeente;
            Postcode = postcode;
            HuisNummer = huisNummer;
        }

        public override string ToString()
        {
            return $"{Straat} {HuisNummer}, {Postcode} {Gemeente}";
        }
    }
}
