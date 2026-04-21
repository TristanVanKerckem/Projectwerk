using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class BeschikbareFaciliteiten
    {
        public int Id { get; set; }
        public string Naam { get; set; }
        public bool IsGeverifieerd { get; set; }

        public BeschikbareFaciliteiten(int id, string naam, bool isGeverifieerd)
        {
            Id = id;
            Naam = naam;
            IsGeverifieerd = isGeverifieerd;
        }
    }
}
