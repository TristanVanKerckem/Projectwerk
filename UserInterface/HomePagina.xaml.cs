using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerBL.Interfaces;
using ProjectbeheerDL.Repository;
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
            string connectionString = @"Data Source=Laptop_Tristan\SQLEXPRESS;Initial Catalog=Projectbeheer;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
            ProjectRepo repo = new ProjectRepo(connectionString);

            List<ProjectCombinatie> projectenGekregen = repo.GeefAlleProjecten();            
            DataGrid.ItemsSource = projectenGekregen;
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
        public void Project_Aanmaken_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new AanmakenProjectPagina());
        }
    }
}
