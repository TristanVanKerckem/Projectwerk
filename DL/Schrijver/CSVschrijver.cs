using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectbeheerDL.Schrijver
{


    public class CSVschrijver : ICSVschrijver
    {
        
        public void MaakCSV(List<ProjectCombinatie> projecten, string pad)
        {
            
        
            
            string projectenComboNaam = "";
            foreach (ProjectCombinatie project in projecten)
            {
                if (project.ProjectComboLijst.Any())
                {
                    string projectNaam = project.ProjectComboLijst[0].Titel;
                    string substringNaam = char.ToUpper(projectNaam[0]) + projectNaam.Substring(1, (projectNaam.Length > 2 ? 2 : projectNaam.Length - 1));
                    projectenComboNaam += substringNaam;
                }
            }

            
            using (StreamWriter writer = new StreamWriter(Path.Combine(pad, $"AlgemeenOverzicht_{projectenComboNaam}.csv")))
            {
                writer.WriteLine("Titel;Startdatum;Beschrijving;Status Project;Locatie;Project Partners & rol;Stadsontwikkeling Project;Groene Ruimte-Project;Innovatief Wonen-Project");

                foreach (ProjectCombinatie project in projecten)
                {
                    if (!project.ProjectComboLijst.Any()) continue;

                    Project AlgemeneInfo = project.ProjectComboLijst[0];
                    string titel = AlgemeneInfo.Titel;
                    string startDatum = AlgemeneInfo.StartDatum.ToString();
                    string beschrijving = AlgemeneInfo.Beschrijving;
                    string projectStatus = AlgemeneInfo.Status.ToString();
                    string locatie = AlgemeneInfo.Locatie?.ToString() ?? "Onbekend";

                    // met de partners
                    string projectPartnersEnRollen = AlgemeneInfo.ProjectPartners == null || !AlgemeneInfo.ProjectPartners.Any()
                        ? "Geen"
                        : string.Join("|", AlgemeneInfo.ProjectPartners.Select(p => $"{p.Key.Naam}: {p.Value}"));

                    
                    string isStadsontwikkeling = project.ProjectComboLijst.Any(p => p is Stadsontwikkeling) ? "Ja" : "Nee";
                    string isGroeneRuimte = project.ProjectComboLijst.Any(p => p is GroeneRuimte) ? "Ja" : "Nee";
                    string isInnovatiefWonen = project.ProjectComboLijst.Any(p => p is InnovatieWonen) ? "Ja" : "Nee";

                    writer.WriteLine($"{titel};{startDatum};{beschrijving};{projectStatus};{locatie};{projectPartnersEnRollen};{isStadsontwikkeling};{isGroeneRuimte};{isInnovatiefWonen}");
                }
            }
            // we kunnen hier op dezelfde manier andere bestanden toevoegen (StadsontwikkelingOverzicht, enz.)
        }

        //    public class CSVschrijver : ICSVschrijver 
        //    {
        //        //public string SchrijfProjectenNaarCSV(List<ProjectCombinatie> projecten, string pad) {
        //        public void SchrijfProjectenNaarCSV(List<ProjectCombinatie> projecten, string pad) {

        //        //    // Voor de naam van PDF bestand worden telkens de eerste 3 letters van de Titel van het Project genomen.
        //        //    string projectenComboNaam = "";
        //        //    foreach (ProjectCombinatie project in projecten) {
        //        //        string projectNaam = project.ProjectComboLijst[0].Titel;
        //        //        string substringNaam = char.ToUpper(projectNaam[0]) + projectNaam.Substring(1, 2); // voorste letter in CAPS en dan nog 2 letters erna

        //        //        projectenComboNaam += substringNaam;
        //        //    }

        //        //    using (StreamWriter writer = File.CreateText($"AlgemeenOverzicht_{projectenComboNaam}.csv")) {
        //        //        // header csv hoofdbestand
        //        //        writer.WriteLine("Titel;Startdatum;Beschrijving;Status Project;Locatie;Project Partners & rol;Stadsontwikkeling Project;Groene Ruimte-Project;Innovatief Wonen-Project");
        //        //        foreach (ProjectCombinatie project in projecten) {
        //        //            Project AlgemeneInfo = project.ProjectComboLijst[0]; // Er is altijd 1 kindproject aanwezig --> dus de eerste in de lijst heeft altijd de info
        //        //            string titel = AlgemeneInfo.Titel;
        //        //            string startDatum = AlgemeneInfo.StartDatum.ToString();
        //        //            string beschrijving = AlgemeneInfo.Beschrijving;
        //        //            string projectStatus = AlgemeneInfo.Status.ToString(); // niet vergeten om de .ToString aan te passen in domein
        //        //            string locatie = AlgemeneInfo.Locatie.ToString(); // idem hier

        //        //            string projectPartnersEnRollen = "";
        //        //            if (AlgemeneInfo.ProjectPartners.Count() == 0) {
        //        //                projectPartnersEnRollen += "Geen";
        //        //            } else {
        //        //                foreach (ProjectPartner partner in AlgemeneInfo.ProjectPartners) {
        //        //                    projectPartnersEnRollen += $"|{partner.Partner.Naam}: {partner.Rollen}"; // rollen gaan mogelijks nog verder uitgewerkt moeten worden (aangezien er meerdere kunnen zijn per project)
        //        //                }
        //        //                projectPartnersEnRollen = projectPartnersEnRollen.Substring(1); // om de voorste | weg te halen
        //        //            }
        //                    //string projectPartnersEnRollen = "";
        //                    //if (AlgemeneInfo.ProjectPartners.Count() == 0) {
        //                    //    projectPartnersEnRollen += "Geen";
        //                    //} else {
        //                    //    foreach (ProjectPartner partner in AlgemeneInfo.ProjectPartners) {
        //                    //        projectPartnersEnRollen += $"|{partner.Partner.Naam}: {partner.Rollen}"; // rollen gaan mogelijks nog verder uitgewerkt moeten worden (aangezien er meerdere kunnen zijn per project)
        //                    //    }
        //                    //    projectPartnersEnRollen = projectPartnersEnRollen.Substring(1); // om de voorste | weg te halen
        //                    //}

        //        //            string isStadsontwikkeling = "Nee";
        //        //            string isGroeneRuimte = "Nee";
        //        //            string isInnovatiefWonen = "Nee";

        //        //            foreach (Project kindProject in project.ProjectComboLijst) {
        //        //                switch (kindProject) {
        //        //                    case Stadsontwikkeling:
        //        //                        isStadsontwikkeling = "Ja";
        //        //                        break;
        //        //                    case GroeneRuimte:
        //        //                        isGroeneRuimte = "Ja";
        //        //                        break;
        //        //                    case InnovatieWonen:
        //        //                        isInnovatiefWonen = "Ja";
        //        //                        break;
        //        //                }
        //        //            }

        //        //            writer.WriteLine($"{titel};{startDatum};{beschrijving};{projectStatus};{locatie};{projectPartnersEnRollen};{isStadsontwikkeling};{isGroeneRuimte};{isInnovatiefWonen}");
        //        //        }

        //        //    }
        //                    //writer.WriteLine($"{titel};{startDatum};{beschrijving};{projectStatus};{locatie};{projectPartnersEnRollen};{isStadsontwikkeling};{isGroeneRuimte};{isInnovatiefWonen}");
        //                }

        //        //    using (StreamWriter writer = File.CreateText($"StadsontwikkelingOverzicht_{projectenComboNaam}.csv")) {
        //        //        // header csv stadsontwikkelingsbestand --> voor ieder project aparte csv met details
        //        //        writer.WriteLine("Titel;Bouwfirmas;Status Vergunning;Toegankelijkheid;Bezienswaardigheid;Voorziening uitlegbord/infowandeling;Architecturale Waarde");
        //        //    }

        //        //    using (StreamWriter writer = File.CreateText($"GroeneRuimteOverzicht_{projectenComboNaam}.csv")) {
        //        //        // header csv Groene Ruimte
        //        //        writer.WriteLine("Titel;Oppervlakte;Score Biodiversiteit;Aantal Wandelpaden;Beschikbare Faciliteiten;Opgenomen in Toer. Wandelroute;Beoordeling Bezoekers");
        //        //    }

        //        //    using (StreamWriter writer = File.CreateText($"InnovatiefWonenOverzicht_{projectenComboNaam}.csv")) {
        //        //        // header csv Innovatief Wonen
        //        //        writer.WriteLine("Titel;Aantal Wooneenheden;Types Woonvormen;Mogelijkheid Rondleiding;Showwoning voor Bezoekers;Score Arch. Innovatie;Samenwerking erfgoed/toerisme Gent");

        //        //    }

        //        //    return "";
        //        //}
        //    }
        //}
    }
}
