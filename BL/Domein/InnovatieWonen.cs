using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System.Net.NetworkInformation;
using System.Timers;

namespace ProjectbeheerBL.Domein
{
    public class InnovatieWonen : Project
    {
        public int AantalWooneenheden { get; set; }
        public List<WoonvormType> WoonvormTypes { get; set; } = new List<WoonvormType>();
        public bool HeeftRondleiding { get; set; }
        public bool HeeftShowcase { get; set; }
        public double ArchitectuurScore { get; set; }
        public bool HeeftSamenwerkingErfgoedOfToerisme { get; set; }

        public InnovatieWonen(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie, int aantalWooneenheden, bool heeftRondleiding, bool heeftShowcase, double architectuurScore, bool heeftSamenwerkingErfgoedOfToerisme)
            : base(titel, startDatum, beschrijving, status, locatie)
        {
            AantalWooneenheden = aantalWooneenheden;
            HeeftRondleiding = heeftRondleiding;
            HeeftShowcase = heeftShowcase;
            ArchitectuurScore = architectuurScore;
            HeeftSamenwerkingErfgoedOfToerisme = heeftSamenwerkingErfgoedOfToerisme;
        }

        public InnovatieWonen(string titel, DateTime startDatum, string beschrijving, ProjectStatus status, Locatie locatie) : base(titel, startDatum, beschrijving, status, locatie) { }

        public void voegWoonvormTypeToe(WoonvormType type) {
            if (!WoonvormTypes.Contains(type)) {
                WoonvormTypes.Add(type);
            }
        }
    }
}
