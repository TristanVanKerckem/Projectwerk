using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class BeschikbareFaciliteiten
    {
        public int Id;
        public string Naam;
        public bool IsGeverifieerd;

        public BeschikbareFaciliteiten(int id, string naam, bool isGeverifieerd)
        {
            Id = id;
            Naam = naam;
            IsGeverifieerd = isGeverifieerd;
        }
    }
}
