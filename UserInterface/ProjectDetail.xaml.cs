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
        public ProjectDetail(ProjectCombinatie projectCombinatie)
        {
            InitializeComponent();
            var project = projectCombinatie.ProjectComboLijst.FirstOrDefault();            
                this.DataContext = project;            
        }
    }
}
