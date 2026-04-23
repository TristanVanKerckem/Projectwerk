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
using System.Windows.Shapes;

namespace ProjectbeheerUserInterface
{
    /// <summary>
    /// Interaction logic for Partner.xaml
    /// </summary>
    public partial class PartnerWindow : Window
    {
        public string Naam { get; private set; }
        public string Email { get; private set; }
        public string Wijk { get; private set; }
        public string Straat { get; private set; }
        public string Gemeente { get; private set; }
        public int Postcode { get; set; }
        public string Huisnummer { get; private set; }
        public string Rol { get; private set; }
        public PartnerWindow()
        {
            InitializeComponent();
        }                   

        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            Naam = NaamBox.Text;
            Email = EmailBox.Text;
            Wijk = WijkBox.Text;
            Straat = StraatBox.Text;
            Gemeente = GemeenteBox.Text;
            if (int.TryParse(PostcodeBox.Text, out int postcode))
            {
                Postcode = postcode;
            }
            else
            {
                MessageBox.Show("Postcode moet een getal zijn");
            }
            Huisnummer = HuisnummerBox.Text;
            Rol = RolBox.Text;
            this.DialogResult = true;
        }
        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
