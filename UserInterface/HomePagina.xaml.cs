using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectbeheerUserInterface
{
    /// <summary>
    /// Interaction logic for HomePagina.xaml
    /// </summary>
    public partial class HomePagina : Page
    {
        public HomePagina()
        {
            InitializeComponent();
            BeschikbareFaciliteiten faciliteit1 = new BeschikbareFaciliteiten(1, "Faciliteit1", true);
            BeschikbareFaciliteiten faciliteit2 = new BeschikbareFaciliteiten(1, "Faciliteit2", true);
            BeschikbareFaciliteiten faciliteit3 = new BeschikbareFaciliteiten(1, "Faciliteit3", true);
            List<BeschikbareFaciliteiten> faciliteiten = new List<BeschikbareFaciliteiten>{faciliteit1,faciliteit2,faciliteit3};
            Bouwfirma bouwfirma1 = new Bouwfirma("Bouwfirma1", "Example@gmail.com", "0471561847");
            Bouwfirma bouwfirma2 = new Bouwfirma("Bouwfirma2", "Example@gmail.com", "0471561847");
            Bouwfirma bouwfirma3 = new Bouwfirma("Bouwfirma3", "Example@gmail.com", "0471561847");
            List<Bouwfirma> bouwfirmas = new List<Bouwfirma> { bouwfirma1, bouwfirma2, bouwfirma3 };
            Locatie locatie = new Locatie(1,"Goeferdinge", "Waaienberg", "Geraardsbergen", 9500, "77C");
            Partner partner1 = new Partner("Tristan","Tristan.Van.Kerckem@gmail.com",locatie);
            Partner partner2 = new Partner("Laure", "ZIHDB", locatie);
            
            {

            }

            InnovatieWonen innovatieWonen = new InnovatieWonen("Titel", DateTime.Now, "Beschrijving", ProjectStatus.Afgerond, locatie, 2, true, true, 4.2, true);
            GroeneRuimte groeneRuimteProject = new GroeneRuimte("TestProject", DateTime.Now, "Een testproject", ProjectStatus.Planning, locatie, 20, 8.3, 4, faciliteiten, true, 7);
            Stadsontwikkeling stadsontwikkeling = new Stadsontwikkeling("Stad", DateTime.Now, "Stadbeschrijving", ProjectStatus.Uitvoering, locatie, bouwfirmas, VergunningStatus.Goedgekeurd, Toegankelijkheid.Gedeeltelijk, true, true, true);
            List<Project> projectenInCombinatie = new List<Project> {innovatieWonen,groeneRuimteProject };
            List<string> rollen = new List<string> {"Niks","Noppes" };

            groeneRuimteProject.VoegPartnerToe(partner1,rollen);
            groeneRuimteProject.VoegPartnerToe(partner2, rollen);

            ProjectCombinatie projectCombinatie = new ProjectCombinatie(projectenInCombinatie);
            ProjectCombinatie innovatie = new ProjectCombinatie(innovatieWonen);
            ProjectCombinatie groeneRuimte = new ProjectCombinatie(groeneRuimteProject);
            ProjectCombinatie groeneRuimte1 = new ProjectCombinatie(groeneRuimteProject);
            ProjectCombinatie stads = new ProjectCombinatie(stadsontwikkeling);
            

            List<ProjectCombinatie> projecten = new List<ProjectCombinatie> { groeneRuimte, innovatie, stads , projectCombinatie, groeneRuimte1 };
            DataGrid.ItemsSource = projecten;
        }
        public void Meer_Info_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            ProjectCombinatie project = button.Tag as ProjectCombinatie;
            if (project != null)
            {
                NavigationService.Navigate(new ProjectDetail(project));
            }
        }
    }
}
