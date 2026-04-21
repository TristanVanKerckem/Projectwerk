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



        public void VoegProjectToe(Project project) {
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
                cmd.Parameters.AddWithValue("@status", project.Status);
                cmd.Parameters.AddWithValue("@locId", (int)project.Locatie.Id);

                conn.Open();
                project.Id = (int)cmd.ExecuteScalar();
            }
        }

        public List<Project> GeefAlleProjecten() {
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

       public Project GeefProject(int id) {
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

        public List<ProjectCombinatie> GeefProjectCombinaties()
        {
            Dictionary<int, ProjectCombinatie> combinatiesDict = new Dictionary<int, ProjectCombinatie>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                // SQL met LEFT JOINs om alle types (Stads, Groen, Wonen) tegelijk te laden
                string sql = @"SELECT pc.Id AS ComboId, p.*, s.VergunningStatus, s.HeeftArchitecturaleWaarde, 
                       g.Oppervlakte, i.AantalWooneenheden, i.HeeftRondleiding 
                       FROM ProjectCombinatie pc
                       JOIN Project p ON pc.Id = p.CombinatieId 
                       LEFT JOIN StadsOntwikkeling s ON p.Id = s.Id
                       LEFT JOIN GroeneRuimte g ON p.Id = g.Id
                       LEFT JOIN InnovatieWonen i ON p.Id = i.Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int cId = (int)reader["ComboId"];
                        if (!combinatiesDict.ContainsKey(cId))
                            combinatiesDict.Add(cId, new ProjectCombinatie { Id = cId, ProjectComboLijst = new List<Project>() });

                        Project p;
                        if (reader["VergunningStatus"] != DBNull.Value)
                            p = new Stadsontwikkeling { HeeftArchitecturaleWaarde = (bool)reader["HeeftArchitecturaleWaarde"] };
                        else if (reader["Oppervlakte"] != DBNull.Value)
                            p = new GroeneRuimte { Oppervlakte = (double)reader["Oppervlakte"] };
                        else
                            p = new InnovatieWonen { HeeftRondleiding = (bool)reader["HeeftRondleiding"] };

                        p.Id = (int)reader["Id"];
                        p.Titel = reader["Titel"].ToString();
                        combinatiesDict[cId].ProjectComboLijst.Add(p);
                    }
                }
            }
            return combinatiesDict.Values.ToList();
        }

        //public List<ProjectCombinatie> GeefProjectCombinaties()
        //{
        //    // We gebruiken een Dictionary om combinaties uniek bij te houden op basis van hun ID
        //    Dictionary<int, ProjectCombinatie> combinatiesDict = new Dictionary<int, ProjectCombinatie>();

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        // SQL-query om projecten en hun specifieke sub-type gegevens op te halen via LEFT JOINs
        //        // We selecteren ook de CombinatieId om de koppeling te kunnen maken
        //        string sql = @"SELECT pc.Id AS ComboId, p.*, 
        //                           s.VergunningStatus, s.HeeftArchitecturaleWaarde, s.Toegankelijkheid,
        //                           g.Oppervlakte, g.Biodiversiteit,
        //                           i.AantalWooneenheden, i.HeeftRondleiding
        //                    FROM ProjectCombinatie pc
        //                    JOIN Project p ON pc.Id = p.CombinatieId 
        //                    LEFT JOIN StadsOntwikkeling s ON p.Id = s.Id
        //                    LEFT JOIN GroeneRuimte g ON p.Id = g.Id
        //                    LEFT JOIN InnovatieWonen i ON p.Id = i.Id
        //                    ORDER BY pc.Id";

        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        conn.Open();

        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                int comboId = (int)reader["ComboId"];

        //                // Controleer of de combinatie al in onze Dictionary zit, anders maken we een nieuwe aan
        //                if (!combinatiesDict.ContainsKey(comboId))
        //                {
        //                    combinatiesDict.Add(comboId, new ProjectCombinatie
        //                    {
        //                        Id = comboId,
        //                        ProjectComboLijst = new List<Project>()
        //                    });
        //                }

        //                // Initialiseer een variabele voor het project-object
        //                Project project = null;

        //                // Mapping: We bepalen het type project op basis van de aanwezige data (NULL-checks)
        //                if (reader["VergunningStatus"] != DBNull.Value)
        //                {
        //                    // Het is een Stadsontwikkeling project
        //                    project = new Stadsontwikkeling
        //                    {
        //                        HeeftArchitecturaleWaarde = (bool)reader["HeeftArchitecturaleWaarde"],
        //                        Toegankelijkheid = (Toegankelijkheid)reader["Toegankelijkheid"]
        //                    };
        //                }
        //                else if (reader["Oppervlakte"] != DBNull.Value)
        //                {
        //                    // Het is een Groene Ruimte project
        //                    project = new GroeneRuimte
        //                    {
        //                        Oppervlakte = (double)reader["Oppervlakte"],
        //                        Biodiversiteit = (int)reader["Biodiversiteit"]
        //                    };
        //                }
        //                else if (reader["AantalWooneenheden"] != DBNull.Value)
        //                {
        //                    // Het is een Innovatief Wonen project
        //                    project = new InnovatieWonen
        //                    {
        //                        AantalWooneenheden = (int)reader["AantalWooneenheden"],
        //                        HeeftRondleiding = (bool)reader["HeeftRondleiding"]
        //                    };
        //                }
        //                else
        //                {
        //                    // Indien geen specifiek type, maken we een basis Project aan
        //                    project = new Project();
        //                }

        //                // Algemene projecteigenschappen mappen (overeenkomstig met feedback Tommy/Wim)
        //                project.Id = (int)reader["Id"];
        //                project.Titel = reader["Titel"].ToString();
        //                project.StartDatum = (DateTime)reader["StartDatum"];
        //                project.Beschrijving = reader["Beschrijving"].ToString();
        //                project.Status = (ProjectStatus)reader["Status"];

        //                // Voeg het gemapte project toe aan de lijst van de juiste combinatie
        //                combinatiesDict[comboId].ProjectComboLijst.Add(project);
        //            }
        //        }
        //    }
        //    // Zet de Dictionary waarden om naar een List voor het resultaat
        //    return combinatiesDict.Values.ToList();
        //}

    }
}