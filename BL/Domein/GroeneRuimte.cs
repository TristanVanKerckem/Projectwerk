using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ProjectbeheerBL.Domein
{
    public class GroeneRuimte : Project
    {
        public double Oppervlakte { get; set; }
        public double Biodiversiteit { get; set; }
        public int AantalWandelpaden { get; set; }
        public List<BeschikbareFaciliteiten> BeschikbareFaciliteiten { get; set; }
        public bool IsInToeristWandelroute { get; set; }
        public double Beoordeling { get; set; }
        public GroeneRuimte(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie, double oppervlakte, double biodiversiteit, int aantalWandelpaden, List<BeschikbareFaciliteiten> beschikbareFaciliteiten, bool isInToeristWandelroute, double beoordeling) : base(titel, startDatum, beschrijving, status, locatie) {
            Oppervlakte = oppervlakte;
            Biodiversiteit = biodiversiteit;
            AantalWandelpaden = aantalWandelpaden;
            BeschikbareFaciliteiten = beschikbareFaciliteiten;
            IsInToeristWandelroute = isInToeristWandelroute;
            Beoordeling = beoordeling;
        }
    }
}
