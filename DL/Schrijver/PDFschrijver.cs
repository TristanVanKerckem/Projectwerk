using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ProjectbeheerDL.Schrijver {


    public class PDFSchrijver  {
        public PDFSchrijver() {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Byte[] maakPDF(List<ProjectCombinatie> projecten) {

            // Voor de naam van PDF bestand worden telkens de eerste 3 letters van de Titel van het Project genomen.
            string projectenComboNaam = "";
            foreach (ProjectCombinatie project in projecten) {
            string projectNaam = project.ProjectComboLijst[0].Titel;
            string substringNaam = char.ToUpper(projectNaam[0]) + projectNaam.Substring(1, 2); // voorste letter in CAPS en dan nog 2 letters erna
            
            projectenComboNaam += substringNaam;
            }

            var document = Document.Create(container => {
                container.Page(pagina => {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(72, Unit.Point); //Standaard pt marge voor documenten
                    pagina.PageColor(Colors.White);
                    pagina.DefaultTextStyle(tekst => tekst.FontSize(12).FontFamily(Fonts.Verdana));

                    pagina.Header().Text("Overzicht Projecten").FontSize(25).SemiBold();

                    pagina.Content().PaddingVertical(15).Column(col => {
                        foreach (ProjectCombinatie project in projecten) {
                            // invullen met de info die nodig is voor het overzicht vd projecten
                        }
                    });

                    pagina.Footer().AlignRight().AlignBottom().Text(pagNr => {
                        pagNr.CurrentPageNumber();
                    });
                });
            });

            byte[] pdf = document.GeneratePdf(); // naam zou toegevoegd moeten worden aan de controller (nog uit te zoeken)

            return pdf;
        }

    }
}
