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
            this.isBezienswaardig = isBezienswaardig;
            this.heeftInfo = heeftInfo;
            this.heeftArchitecturaleWaarde = heeftArchitecturaleWaarde;
        }

        public List<Bouwfirma> Bouwfirmas { get; set; }
        public VergunningStatus VergunningStatus { get; set; }
        public Toegankelijkheid Toegankelijkheid { get; set; }
        public bool isBezienswaardig { get; set; }
        public bool heeftInfo { get; set; }
        public bool heeftArchitecturaleWaarde { get; set; }
    }
}
