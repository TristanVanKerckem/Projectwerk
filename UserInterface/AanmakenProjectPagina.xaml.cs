using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerDL.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
using static System.Net.Mime.MediaTypeNames;

namespace ProjectbeheerUserInterface
{

    /// <summary>
    /// Interaction logic for AanmakenProjectPagina.xaml
    /// </summary>
    public partial class AanmakenProjectPagina : Page
    {
        public Partner Partner { get; set; }
        public List<string> Rollen { get; set; }

        public ProjectCombinatie Projecten { get; set; } = new ProjectCombinatie();
        public BeschikbareFaciliteiten BeschikbareFaciliteiten { get; set; }
        public Bouwfirma Bouwfirma { get; set; }

        public AanmakenProjectPagina()
        {
            InitializeComponent();
            StatusCombo.ItemsSource = Enum.GetValues(typeof(ProjectStatus));
            VergunningCombo.ItemsSource = Enum.GetValues(typeof(VergunningStatus));
            ToegankelijkheidCombo.ItemsSource = Enum.GetValues(typeof (Toegankelijkheid));

        }
        private void Opslaan_Click(object sender, RoutedEventArgs e)
        {
            string titel = TitelBox.Text;
            string beschrijving = BeschrijvingBox.Text;
            DateTime? startdatum = StartDatumPicker.SelectedDate;

            string wijk = WijkBox.Text;
            string straat = StraatBox.Text;
            string gemeente = GemeenteBox.Text;
            string huisnummer = HuisnummerBox.Text;
            ProjectStatus status = (ProjectStatus)StatusCombo.SelectedItem;
            if (string.IsNullOrWhiteSpace(TitelBox.Text) ||
    string.IsNullOrWhiteSpace(BeschrijvingBox.Text) ||
    StartDatumPicker.SelectedDate == null ||
    string.IsNullOrWhiteSpace(WijkBox.Text) ||
    string.IsNullOrWhiteSpace(StraatBox.Text) ||
    string.IsNullOrWhiteSpace(GemeenteBox.Text) ||
    string.IsNullOrWhiteSpace(PostcodeBox.Text) ||
    string.IsNullOrWhiteSpace(HuisnummerBox.Text))
            {
                MessageBox.Show("Gelieve alle velden in te vullen");
                return;
            } DateTime startDatum = StartDatumPicker.SelectedDate.Value;

            int postcode = 0;
            if (!int.TryParse(PostcodeBox.Text, out postcode))
            {
                MessageBox.Show("Postcode moet een getal zijn");
                return;
            }


            Locatie locatieProject = new Locatie(wijk, straat, gemeente, postcode, huisnummer);
            string connectionString = @"Data Source=Laptop_Tristan\SQLEXPRESS;Initial Catalog=Projectbeheer;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            ProjectRepo repo = new ProjectRepo(connectionString);            
            if (GroeneRuimteRB.IsChecked == true)
            {
                if (
                string.IsNullOrWhiteSpace(OppervlakteBox.Text) ||
    string.IsNullOrWhiteSpace(BiodiversiteitBox.Text) ||
    string.IsNullOrWhiteSpace(WandelpadenBox.Text) ||
    string.IsNullOrWhiteSpace(BeoordelingBox.Text))
                {
                    MessageBox.Show("Gelieve alle velden in te vullen");
                    return;
                }
                if (!double.TryParse(OppervlakteBox.Text, out double oppervlakte))
                {
                    MessageBox.Show("Oppervlakte moet een getal zijn");
                    return;
                }
                if (!double.TryParse(BiodiversiteitBox.Text, out double biodiversiteit))
                {
                    MessageBox.Show("Biodiversiteitscore moet een getal zijn");
                    return;
                }
                if (!int.TryParse(WandelpadenBox.Text, out int wandelpaden))
                {
                    MessageBox.Show("Aantal wandelpaden moet een getal zijn");
                    return;
                }
                if (!double.TryParse(BeoordelingBox.Text, out double beoordeling))
                {
                    MessageBox.Show("Beoordeling moet een getal zijn");
                    return;
                }
                bool toeristischeRoute = false;
                if (ToeristischeRouteCheck.IsChecked == true)
                {
                    toeristischeRoute = true;
                }
                List<BeschikbareFaciliteiten> faciliteiten = new List<BeschikbareFaciliteiten> { BeschikbareFaciliteiten };
                GroeneRuimte groeneruimte = new GroeneRuimte(titel, startDatum, beschrijving, status, locatieProject, oppervlakte, biodiversiteit, wandelpaden, faciliteiten, toeristischeRoute, beoordeling);
                Projecten.VoegProjectToe(groeneruimte);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        int id = repo.VoegProjectToe(
                            groeneruimte,
                            Partner,
                            locatieProject,
                            Rollen,
                            conn,
                            trans
                        );

                        trans.Commit();
                    }
                }
            }
            if (InnovatieRB.IsChecked == true)
            {
                if (
                string.IsNullOrWhiteSpace(AantalWooneenhedenBox.Text) ||
    string.IsNullOrWhiteSpace(ArchitectuurScoreBox.Text))
                {
                    MessageBox.Show("Gelieve alle velden in te vullen");
                    return;
                }
                if (!int.TryParse(AantalWooneenhedenBox.Text, out int aantalWooneenheden))
                {
                    MessageBox.Show("Aantal wooneenheden moet een getal zijn");
                    return;
                }
                if (!double.TryParse(ArchitectuurScoreBox.Text, out double architectuurScore))
                {
                    MessageBox.Show("Architectuurscore moet een getal zijn");
                    return;
                }
                bool rondleiding = (bool)RondleidingCheck.IsChecked;
                bool showcase = (bool)ShowcaseCheck.IsChecked;
                bool samenwerking = (bool)SamenwerkingCheck.IsChecked;
                InnovatieWonen innovatieWonen = new InnovatieWonen(titel, startDatum, beschrijving, status, locatieProject, aantalWooneenheden, rondleiding, showcase, architectuurScore,samenwerking);
                Projecten.VoegProjectToe(innovatieWonen);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        int id = repo.VoegProjectToe(
                            innovatieWonen,
                            Partner,
                            locatieProject,
                            Rollen,
                            conn,
                            trans
                        );

                        trans.Commit();
                    }
                }
            }
            if (StadRB.IsChecked == true)
            {
                bool bezienswaardig = (bool)BezienswaardigCheck.IsChecked;
                bool info = (bool)InfoCheck.IsChecked;
                bool architectureel = (bool)ArchitectuurWaardeCheck.IsChecked;
                VergunningStatus vergunningstatus = (VergunningStatus)VergunningCombo.SelectedItem;
                Toegankelijkheid toegankelijkheid = (Toegankelijkheid)ToegankelijkheidCombo.SelectedItem;
                List<Bouwfirma> bouwfirmas = new List<Bouwfirma> { Bouwfirma};
                Stadsontwikkeling stadsontwikkeling = new Stadsontwikkeling(titel, startDatum, beschrijving, status, locatieProject, bouwfirmas, vergunningstatus, toegankelijkheid, bezienswaardig, info, architectureel);
                Projecten.VoegProjectToe(stadsontwikkeling);
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction trans = conn.BeginTransaction())
                    {
                        int id = repo.VoegProjectToe(
                            stadsontwikkeling,
                            Partner,
                            locatieProject,
                            Rollen,
                            conn,
                            trans
                        );

                        trans.Commit();
                    }
                }
            }
            NavigationService.Navigate(new ProjectDetail(Projecten));
        }






        private void Type_Checked(object sender, RoutedEventArgs e)
        {
            if (GroeneRuimteRB.IsChecked == true)
            {
                GroeneRuimtePanel.Visibility = Visibility.Visible;
            }
            else { GroeneRuimtePanel.Visibility = Visibility.Collapsed; }
            if (InnovatieRB.IsChecked == true)
            {
                InnovatiePanel.Visibility = Visibility.Visible;
            }
            else { InnovatiePanel.Visibility = Visibility.Collapsed; }
            if (StadRB.IsChecked == true)
            {
                StadsPanel.Visibility = Visibility.Visible;
            }
            else { StadsPanel.Visibility = Visibility.Collapsed; }
            ;
        }
        private void Partner_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartnerWindow
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true)
            {
                string naam = window.Naam;
                string email = window.Email;
                string wijk = window.Wijk;
                string straat = window.Straat;
                string gemeente = window.Gemeente;
                int postcode = window.Postcode;
                string huisnummer = window.Huisnummer;
                Locatie locatie = new Locatie(wijk, straat, gemeente, postcode, huisnummer);
                List<string> rollen = new List<string> { window.Rol };
                Partner partner = new Partner(naam, email, locatie);
                this.DataContext = new { Partner = partner, Rollen = rollen , BeschikbareFaciliteiten, Bouwfirma};
                Partner = partner;
                Rollen = rollen;
                PartnerButton.Visibility = Visibility.Collapsed;
            }
        }
        private void Faciliteit_Click(object sender, RoutedEventArgs e)
        {
            var window = new FaciliteitAanmakenWindow()
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true)
            {
                BeschikbareFaciliteiten = window.Faciliteit;
                this.DataContext = new { Partner, Rollen, BeschikbareFaciliteiten , Bouwfirma};
                FaciliteitButton.Visibility = Visibility.Collapsed;
                FaciliteitenView.Visibility = Visibility.Visible;
            }
        }
        private void Bouwfirma_Click(object sender, RoutedEventArgs e)
        {
            var window = new BouwfirmaAanmaken()
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true)
            {
                Bouwfirma = window.Bouwfirma;
                this.DataContext = new { Partner, Rollen, BeschikbareFaciliteiten, Bouwfirma };
                BouwfirmaButton.Visibility = Visibility.Collapsed;
                BouwfirmaView.Visibility = Visibility.Visible;
            }
        }
    }
}
