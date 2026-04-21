using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Data.SqlClient;

namespace ProjectbeheerDL.Repository {
    public class ProjectRepository : IProjectRepository {

        string connectionstring;
        public ProjectRepository(string connectionstring) {
            this.connectionstring = connectionstring;
        }



        public virtual void VoegProjectToe(Project project) {
            // Code om project toe te voegen aan database
            using(sqlConnection conn = new sqlConnection(connectionstring)) {
                // SQL code om project toe te voegen
                // De ID wordt gegenereerd door de database (Identity)
                string sql = "INSERT INTO Project (titel, startDatum, beschrijving, status, locatieId) " +
                             "OUTPUT INSERTED.id VALUES (@titel, @start, @desc, @status, @locId)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@titel", project.Titel);
                cmd.Parameters.AddWithValue("@start", project.StartDatum);
                cmd.Parameters.AddWithValue("@desc", project.Beschrijving);
                cmd.Parameters.AddWithValue("@status", (int)project.Fase);
                cmd.Parameters.AddWithValue("@locId", project.Locatie.Id);

                conn.Open();
                project.Id = (int)cmd.ExecuteScalar();
            }
        }

        public virtual List<Project> GeefAlleProjecten() {
            List<Project> projecten = new List<Project>();
            using (sqlConnection conn = new sqlConnection(connectionstring)) {
                string sql = "SELECT * FROM Project";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    // Code om project te maken van database gegevens
                    // Locatie moet ook opgehaald worden
                    int locId = (int)reader["locatieId"];
                    Locatie locatie = GeefLocatie(locId); // Methode om locatie op te halen
                    Project project = new Project(
                        (string)reader["titel"],            :// meteen met index werken ipv zoeken op naam
                        (DateTime)reader["startDatum"],
                        (string)reader["beschrijving"],
                        (ProjectStatus)(int)reader["status"],
                        locatie
                    );
                    project.Id = (int)reader["id"];
                    projecten.Add(project);
                }
            }
            return projecten;
        }

       public virtual Project GeefProject(int id) {
            Project project = null;
            using (sqlConnection conn = new sqlConnection(connectionstring)) {
                string sql = "SELECT * FROM Project WHERE id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    int locId = (int)reader["locatieId"];
                    Locatie locatie = GeefLocatie(locId);
                    project = new Project(
                        (string)reader["titel"],
                        (DateTime)reader["startDatum"],
                        (string)reader["beschrijving"],
                        (ProjectStatus)(int)reader["status"],
                        locatie
                    );
                    project.Id = (int)reader["id"];
                }
            }
            return project;
        }
        
    }
}