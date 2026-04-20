using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class BeschikbareFaciliteiten
    {
        public int Id;
        public string Naam;
        public bool BeschikbareFaciliteit;

        public BeschikbareFaciliteiten(int id, string naam, bool beschikbareFaciliteit)
        {
            Id = id;
            Naam = naam;
            BeschikbareFaciliteit = beschikbareFaciliteit;
        }
    }
}
