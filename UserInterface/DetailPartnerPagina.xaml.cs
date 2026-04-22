using ProjectbeheerBL.Domein;
using System;
using System.Collections.Generic;
using System.Reflection;
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
    /// Interaction logic for DetailPartnerPagina.xaml
    /// </summary>
    public partial class DetailPartnerPagina : Window
    {
        public DetailPartnerPagina(Partner partner, List<string> rollen)
        {
            Dictionary<Partner, List<string>> partners = new Dictionary<Partner, List<string>> ();
            partners[partner] = rollen;
            InitializeComponent();
            this.DataContext = (partners);            
        }
    }
}
