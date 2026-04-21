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
            Locatie locatie = new Locatie("Goeferdinge", "Waaienberg", "Geraardsbergen", 9500, "77C");
            GroeneRuimte groeneRuimteProject = new GroeneRuimte("TestProject", DateTime.Now, "Een testproject", ProjectStatus.Planning, locatie, 20, 8.3, 4, faciliteiten, true, 7);
            GroeneRuimte groeneRuimteProject1 = new GroeneRuimte("TestProject", DateTime.Now, "Een testproject", ProjectStatus.Planning, locatie, 21, 8.3, 4, faciliteiten, true, 7);
            List<GroeneRuimte> projecten = new List<GroeneRuimte> { groeneRuimteProject, groeneRuimteProject1};
            DataGrid.ItemsSource = projecten;
        }
        public void Meer_Info_Click(object sender, EventArgs e)
        {
            BeschikbareFaciliteiten faciliteit1 = new BeschikbareFaciliteiten(1, "Faciliteit1", true);
            BeschikbareFaciliteiten faciliteit2 = new BeschikbareFaciliteiten(1, "Faciliteit2", true);
            BeschikbareFaciliteiten faciliteit3 = new BeschikbareFaciliteiten(1, "Faciliteit3", true);
            List<BeschikbareFaciliteiten> faciliteiten = new List<BeschikbareFaciliteiten> { faciliteit1, faciliteit2, faciliteit3 };
            Locatie locatie = new Locatie("Goeferdinge", "Waaienberg", "Geraardsbergen", 9500, "77C");
            GroeneRuimte groeneRuimteProject = new GroeneRuimte("TestProject", DateTime.Now, "Een testproject", ProjectStatus.Planning, locatie, 20, 8.3, 4, faciliteiten, true, 7);
            NavigationService.Navigate(new DetailPagina(groeneRuimteProject));
        }
    }
}
