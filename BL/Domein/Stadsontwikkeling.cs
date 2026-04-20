using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerBL.Domein
{
    public class Stadsontwikkeling : Project {
        public Stadsontwikkeling(string titel, DateTime startDatum, string beschrijving, ProjectStatus projectStatus, Locatie locatie, List<Bouwfirma> bouwfirmas, VergunningStatus vergunningStatus, Toegankelijkheid toegankelijkheid, bool isBezienswaardig, bool heeftInfo, bool heeftArchitecturaleWaarde) : base(titel, startDatum, beschrijving, projectStatus, locatie) {
            Bouwfirmas = bouwfirmas;
            VergunningStatus = vergunningStatus;
            Toegankelijkheid = toegankelijkheid;
            IsBezienswaardig = isBezienswaardig;
            HeeftInfo = heeftInfo;
            HeeftArchitecturaleWaarde = heeftArchitecturaleWaarde;
        }

        public List<Bouwfirma> Bouwfirmas { get; set; }
        public VergunningStatus VergunningStatus { get; set; }
        public Toegankelijkheid Toegankelijkheid { get; set; }
        public bool IsBezienswaardig { get; set; }
        public bool HeeftInfo { get; set; }
        public bool HeeftArchitecturaleWaarde { get; set; }
    }
}
