using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ProjectDetail : Page
    {
        public ProjectCombinatie ProjectCombinatie { get; set; }
        public ProjectDetail(ProjectCombinatie projectCombinatie)
        {
            ProjectCombinatie = projectCombinatie;
            InitializeComponent();
            this.DataContext = projectCombinatie.ProjectComboLijst;
        }
        

        private void Aanpassen_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AanpassenPagina(ProjectCombinatie));
        }
        private void Card_Click(object sender, RoutedEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var item = (KeyValuePair<Partner, List<string>>)border.DataContext;

            Partner partner = item.Key;
            List<string> rollen = item.Value;

            var window = new DetailPartnerPagina(partner, rollen);
            window.Show();
        }
        private void Keer_Terug(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HomePagina());
        }
    }
}
