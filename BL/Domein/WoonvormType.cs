using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class WoonvormType {
        public WoonvormType(int id, string naam, bool isGeverifieerd) {
            Id = id;
            Naam = naam;
            this.isGeverifieerd = isGeverifieerd;
        }

        public int Id { get; set; }
        public string Naam { get; set; }

        public bool isGeverifieerd { get; set; }
    }
}
