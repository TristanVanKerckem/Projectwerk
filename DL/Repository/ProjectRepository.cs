using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProjectbeheerDL.Repository {
    public class ProjectRepository : IProjectRepository {

        string connectionstring;
        public ProjectRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }

        public void voegProjectToe(Project project) {
            string query = "INSERT INTO project";
        }
    }
}
