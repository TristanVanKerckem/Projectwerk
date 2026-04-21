using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerBL.Exeptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace ProjectbeheerBL.Domein
{
    public class GroeneRuimte : Project
    {
        private double _oppervlakte;
        private double _biodiversiteit;
        private int _aantalWandelpaden;
        private double _beoordeling;
        private bool _isInToeristWandelroute;

        public double Oppervlakte
        {
            get { return _oppervlakte; }
            set
            {
                if (value <= 0)
                    throw new GroeneRuimteException("Oppervlakte mag niet 0 of minder zijn");
                else
                    _oppervlakte = value;
            }
        }

        public double Biodiversiteit
        {
            get { return _biodiversiteit; }
            set
            {
                if (value <= 0)
                    throw new GroeneRuimteException("Biodiversiteit mag niet 0 of minder zijn");
                else
                    _biodiversiteit= value;
            }
        }
        public int AantalWandelpaden
        {
            get { return _aantalWandelpaden; }
            set
            {
                if (value <= 0)
                    throw new GroeneRuimteException("Oppervlakte mag niet 0 of minder zijn");
                else
                    _oppervlakte = value;
            }
        }
        public List<BeschikbareFaciliteiten> BeschikbareFaciliteiten { get; set; }
        public bool IsInToeristWandelroute { get; set; }
        public double Beoordeling
        {
            get { return _beoordeling; }

            set
            {
                if (value <= 0)
                    throw new GroeneRuimteException("Beoordeling mag niet 0 of minder zijn");
                else
                    _oppervlakte = value;
            }
        }
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
