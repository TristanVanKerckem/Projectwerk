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
    /// Interaction logic for LocatieAanpassenPagina.xaml
    /// </summary>
    public partial class LocatieAanpassenPagina : Window
    {
        public string Wijk;
        public string Straat;
        public string Gemeente;
        public int Postcode;
        public string Huisnummer;
        public LocatieAanpassenPagina(Locatie locatie)
        {
            InitializeComponent();
            this.DataContext = (locatie);
        }
        private void Pas_Aan_Click(object sender, RoutedEventArgs e)
        {
            Locatie locatie = (Locatie)DataContext;

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

        private void Verzenden_Click(object sender, RoutedEventArgs e)
        {
            if (WijkEditBox.Text != null)
            {
                Wijk = WijkEditBox.Text;
            }
            if (StraatEditBox.Text != null)
            {
                Straat = StraatEditBox.Text;
            }
            if (GemeenteEditBox.Text != null)
            {
                Gemeente = GemeenteEditBox.Text;
            }
            if (int.TryParse(PostcodeEditBox.Text,out Postcode))
            {

            }
            if (HuisnummerEditBox.Text != null)
            {
                Huisnummer = HuisnummerEditBox.Text;
            }
            this.DialogResult = true;
        }
        private void Annuleer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
