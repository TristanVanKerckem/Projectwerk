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
using ProjectbeheerBL.Domein;

namespace ProjectbeheerUserInterface
{
    /// <summary>
    /// Interaction logic for BouwfirmaAanmaken.xaml
    /// </summary>
    public partial class BouwfirmaAanmaken : Window
    {
        public Bouwfirma Bouwfirma { get; set; }

        public BouwfirmaAanmaken()
        {
            InitializeComponent();
        }
        private void Toevoegen_Click(object sender, RoutedEventArgs e)
        {
            string naam = NaamBox.Text;
            string email = EmailBox.Text;
            string telefoonnummer = TelefoonnummerBox.Text;

            Bouwfirma = new Bouwfirma(naam, email, telefoonnummer);
            this.DialogResult = true;
        }
        private void Annuleren_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
