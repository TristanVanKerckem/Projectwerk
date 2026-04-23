using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for AanpassenPagina.xaml
    /// </summary>
    public partial class AanpassenPagina : Page
    {
        public ProjectCombinatie Projecten { get; set; }
        public AanpassenPagina(ProjectCombinatie projectCombinatie)
        {
            Projecten = projectCombinatie;
            InitializeComponent();
            StatusCombo.ItemsSource = Enum.GetValues(typeof(ProjectStatus));
            this.DataContext = projectCombinatie.ProjectComboLijst;
        }
        public void OnSubmit(object sender, EventArgs e)
        {
            Projecten.ProjectComboLijst[0].Titel = TitelView.Text;
            Projecten.ProjectComboLijst[0].Beschrijving = BeschrijvingView.Text;
            Projecten.ProjectComboLijst[0].Locatie.Wijk = WijkView.Text;

            NavigationService.Navigate(new ProjectDetail(Projecten));
        }
        private void Pas_Aan_Click(object sender, RoutedEventArgs e)
        {
            var projecten = (List<Project>)DataContext;
            Project project = projecten[0];

            var button = (Button)sender;
            string field = button.Tag.ToString();

            TextBlock view = (TextBlock)this.FindName(field + "View");
            TextBox edit = (TextBox)this.FindName(field + "EditBox");
            if (view.Visibility == Visibility.Visible)
            {
                edit.Text = view.Text;
                edit.Visibility = Visibility.Visible;
                view.Visibility = Visibility.Collapsed;
            }
            else
            {
                view.Visibility = Visibility.Visible;
                edit.Visibility = Visibility.Collapsed;
                view.Text = edit.Text;                
            }

        }
        private void StartDatum_Click(object sender, RoutedEventArgs e)
        {
            List<Project> projecten = (List<Project>)DataContext;
            Project project = projecten[0];

            Button button = (Button)sender;
            string field = button.Tag.ToString();

            TextBlock view = (TextBlock)this.FindName(field + "View");
            DatePicker edit = (DatePicker)this.FindName(field + "EditBox");
            if (view.Visibility == Visibility.Visible)
            {
                edit.SelectedDate = project.StartDatum;

                edit.Visibility = Visibility.Visible;
                view.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (edit.SelectedDate.HasValue)
                {
                    project.StartDatum = edit.SelectedDate.Value;
                }

                view.Text = project.StartDatum.ToString("dd/MM/yyyy");
                Projecten.ProjectComboLijst[0].StartDatum = project.StartDatum;
                view.Visibility = Visibility.Visible;
                edit.Visibility = Visibility.Collapsed;
            }
        }
        private void Partner_Click(object sender, RoutedEventArgs e)
        {
            var window = new PartnerWindow
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true)
            {
                Project project = ((List<Project>)DataContext)[0];
                string naam = window.Naam;
                string email = window.Email;
                string wijk = window.Wijk;
                string straat = window.Straat;
                string gemeente = window.Gemeente;
                int postcode = window.Postcode;
                string huisnummer = window.Huisnummer;
                List<string> rollen = new List<string> { window.Rol };
                Partner partner = new Partner(naam, email, project.Locatie);
                project.VoegPartnerToe(partner, rollen);
                Partners.Items.Refresh();  
            }
        }
        private void Locatie_Click(object sender, RoutedEventArgs e)
        {

            Locatie locatie = Projecten.ProjectComboLijst[0].Locatie;
            var window = new LocatieAanpassenPagina(locatie)
            {
                Owner = Window.GetWindow(this)
            };
            if (window.ShowDialog() == true)
            {
                UpdateIfNotEmpty(window.Wijk, v => locatie.Wijk = v);
                UpdateIfNotEmpty(window.Straat, v => locatie.Straat = v);
                UpdateIfNotEmpty(window.Gemeente, v => locatie.Gemeente = v);
                UpdateIfNotEmpty(window.Huisnummer, v => locatie.HuisNummer = v);
                if (window.Postcode != 0)
                {
                    locatie.Postcode = window.Postcode;
                }
                Projecten.ProjectComboLijst[0].Locatie = locatie;
                NavigationService.Navigate(new AanpassenPagina(Projecten));
            }
        }
        void UpdateIfNotEmpty(string newValue, Action<string> setAction)
        {
            if (!string.IsNullOrWhiteSpace(newValue))
                setAction(newValue);
        }
    }
}

