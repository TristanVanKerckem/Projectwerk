using ProjectbeheerBL.Domein;
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
    /// Interaction logic for AanpassenPagina.xaml
    /// </summary>
    public partial class AanpassenPagina : Page
    {
        public AanpassenPagina(ProjectCombinatie projectCombinatie)
        {
            this.DataContext = projectCombinatie.ProjectComboLijst;
        }
        public void OnSubmit(object sender, EventArgs e)
        {

        }
    }
}
