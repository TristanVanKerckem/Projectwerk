using ProjectbeheerBL.Domein;
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
using System.Windows.Shapes;

namespace ProjectbeheerUserInterface
{
    /// <summary>
    /// Interaction logic for FaciliteitAanmakenWindow.xaml
    /// </summary>
    public partial class FaciliteitAanmakenWindow : Window
    {
        public BeschikbareFaciliteiten Faciliteit {  get; set; }
        public FaciliteitAanmakenWindow()
        {
            InitializeComponent();
        }
        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            string naam = NaamBox.Text;
            bool isGeverifeerd = (bool)IsGeverfieerdCheck.IsChecked;
            
            Faciliteit = new BeschikbareFaciliteiten(naam, isGeverifeerd);
            this.DialogResult = true;
        }
        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
