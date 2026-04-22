using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums; // Zorg dat de namespace voor enums klopt
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProjectbeheerDL.Repository
{
    public class ProjectRepo : IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void VoegProjectToe(Project project)
        {
            if (project is Stadsontwikkeling s) VoegStadsOntwikkelingToe(s, project, s.Locatie);
            else if (project is GroeneRuimte g) VoegGroeneRuimteToe(g, project, g.Locatie);
            else if (project is InnovatieWonen i) VoegInnovatieWonenToe(i, project, i.Locatie);
            else throw new Exception("Onbekend projecttype.");
        }

        // Belangrijk om apart toe te voegen --> Je kan geen connectie openen als er al 1 geopend is, anders kunnen we geen check doen of er al algemene ProjectInfo in de Database ingevuld staat door een ander kindproject
        public int VoegProjectInfoToe(Project p, Locatie l, SqlConnection conn, SqlTransaction trans) {
            string queryProject = "INSERT INTO Project (titel, startDatum, beschrijving, status, locatieId) VALUES (@titel, @startDatum, @beschrijving, @status, @locatieId); SELECT SCOPE_IDENTITY();";
            // We gaan voor nu de Locatie al hier aanmaken, als het software project uitgewerkt wordt en we voegen Locatie toe voor een Partner best in aparte Repository
            // Op dit moment ook wordt er telkens een nieuwe Locatie aangemaakt, er wordt niet gecheckt of deze al in de database staat
            string queryLocatie = "INSERT INTO Locatie (wijk, straat, gemeente, postcode, huisnummer) VALUES (@wijk, @straat, @gemeente, @postcode, @huisnummer); SELECT SCOPE_IDENTITY();";
            using (SqlCommand cmd1 = new SqlCommand(queryProject, conn, trans))
            using (SqlCommand cmd2 = new SqlCommand(queryLocatie, conn, trans)) {
                cmd1.CommandText = queryProject;
                cmd2.CommandText = queryLocatie;

                try {
                    int databaseLocatieId;
                    // Voeg Locatie Toe
                    cmd2.Parameters.Clear();
                    cmd2.Parameters.AddWithValue("@wijk", l.Wijk);
                    cmd2.Parameters.AddWithValue("@straat", l.Straat);
                    cmd2.Parameters.AddWithValue("@gemeente", l.Gemeente);
                    cmd2.Parameters.AddWithValue("@postcode", l.Postcode);
                    cmd2.Parameters.AddWithValue("@huisnummer", l.HuisNummer);

                    databaseLocatieId = Convert.ToInt32(cmd2.ExecuteScalar()); // kree
                    // Voeg Project Toe
                    cmd1.Parameters.Clear();
                    cmd1.Parameters.AddWithValue("@titel", p.Titel);
                    cmd1.Parameters.AddWithValue("@startDatum", p.StartDatum);
                    cmd1.Parameters.AddWithValue("@beschrijving", p.Beschrijving);
                    cmd1.Parameters.AddWithValue("@status", p.Status);
                    cmd1.Parameters.AddWithValue("@locatieId", databaseLocatieId);

       
                    return Convert.ToInt32(cmd1.ExecuteScalar());
                } catch (Exception ex) {
                    
                    throw new Exception();
                }
            }
        }

        // Nodig voor te checken of ProjectInfo al is aangemaakt door een kindklasse van hetzelfde Project
        public bool ProjectInfoBestaat(int projectId, SqlConnection conn, SqlTransaction trans) { //conn en trans meegeven, anders error door de connectie als je nieuwe start
            string query = "SELECT COUNT(1) FROM Project WHERE id = @projectId";
            using (SqlCommand cmd = new SqlCommand(query, conn, trans)) {
                try {
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }

        public void VoegStadsOntwikkelingToe(Stadsontwikkeling s, Project p, Locatie l)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int databaseProjectId;

                        // check of Project al bestaat
                        bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                        // Toevoegen indien deze nog niet bestaat
                        if (!projectBestaatInDataBank) {
                            databaseProjectId = VoegProjectInfoToe(p, l, conn, trans); // return de id + vult Project verder aan
                        } else {
                            databaseProjectId = p.Id;
                        }
                        //int id = InvoegenBasisProject(s, conn, trans);
                        string sql = @"INSERT INTO StadsOntwikkeling (verguningsStatus, archtitectueleWaarde, toegankelijkheid, bezienswaardigheid, info, projectId) 
                                       VALUES (@verg,@arch, @toeg, @beziens, @info, @projectId)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans)) {
                            //cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@verg", s.VergunningStatus);
                            cmd.Parameters.AddWithValue("@toeg", s.Toegankelijkheid);
                            cmd.Parameters.AddWithValue("@beziens", s.IsBezienswaardig);
                            cmd.Parameters.AddWithValue("@info", s.HeeftInfo);
                            cmd.Parameters.AddWithValue("@arch", s.HeeftArchitecturaleWaarde);
                            cmd.Parameters.AddWithValue("@projectId", databaseProjectId);

                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }

        public void VoegGroeneRuimteToe(GroeneRuimte g, Project p, Locatie l)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        int databaseProjectId;

                        // check of Project al bestaat
                        bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                        // Toevoegen indien deze nog niet bestaat
                        if (!projectBestaatInDataBank) {
                            databaseProjectId = VoegProjectInfoToe(p, l, conn, trans); // return de id + vult Project verder aan
                        } else {
                            databaseProjectId = p.Id;
                        }
                        //int id = InvoegenBasisProject(g, conn, trans);
                        string sql = @"INSERT INTO GroeneRuimte (oppervlakte, biodiversiteit, aantalWandelpaden, inToeristischeWandelroute, beoordeling, projectId) 
                                       VALUES (@opp, @bio, @wandel, @route, @beoordeling, @projectId)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans)) {
                            //cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@opp", g.Oppervlakte);
                            cmd.Parameters.AddWithValue("@bio", g.Biodiversiteit);
                            cmd.Parameters.AddWithValue("@wandel", g.AantalWandelpaden);
                            cmd.Parameters.AddWithValue("@route", g.IsInToeristWandelroute);
                            cmd.Parameters.AddWithValue("@beoordeling", g.Beoordeling);
                            cmd.Parameters.AddWithValue("@projectId", databaseProjectId);

                            cmd.ExecuteNonQuery();
                            
                        }
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }

        public void VoegInnovatieWonenToe(InnovatieWonen i, Project p, Locatie l)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try {
                        int databaseProjectId;
                        // check of Project al bestaat
                        bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                        // Toevoegen indien deze nog niet bestaat
                        if (!projectBestaatInDataBank) {
                            databaseProjectId = VoegProjectInfoToe(p, l, conn, trans); // return de id + vult Project verder aan
                        } else {
                            databaseProjectId = p.Id;
                        }

                        //int id = InvoegenBasisProject(i, conn, trans);
                        string sql = @"INSERT INTO InnovatieWonen (aantalWooneenheden, rondleiding, showwoning, architectuurInnovatieScore, samenwerkingErfgoed, samenwerkingToerisme, projectId) 
                                       VALUES (@aantal, @rond, @show, @score, @samenE, @samenT, @projectId)"; // !!!NOG AANPASSEN DAT ERFGOED EN TOERISME 1 BOOL ZIJN!!!


                        using (SqlCommand cmd = new SqlCommand(sql, conn, trans)) {

                            //cmd.Parameters.AddWithValue("@id", id);
                            cmd.Parameters.AddWithValue("@aantal", i.AantalWooneenheden);
                            cmd.Parameters.AddWithValue("@rond", i.HeeftRondleiding);
                            cmd.Parameters.AddWithValue("@show", i.HeeftShowcase);
                            cmd.Parameters.AddWithValue("@score", i.ArchitectuurScore);
                            cmd.Parameters.AddWithValue("@samenE", i.HeeftSamenwerkingErfgoedOfToerisme);
                            cmd.Parameters.AddWithValue("@samenT", i.HeeftSamenwerkingErfgoedOfToerisme);
                            cmd.Parameters.AddWithValue("@projectId", databaseProjectId);

                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    } catch { trans.Rollback(); throw; }
                }
            }
        }

        //private int InvoegenBasisProject(Project p, SqlConnection conn, SqlTransaction trans)
        //{
        //    string sql = "INSERT INTO Project (Titel, StartDatum, Beschrijving, Status) OUTPUT INSERTED.Id VALUES (@titel, @datum, @desc, @status)";
        //    SqlCommand cmd = new SqlCommand(sql, conn, trans);
        //    cmd.Parameters.AddWithValue("@titel", p.Titel);
        //    cmd.Parameters.AddWithValue("@datum", p.StartDatum);
        //    cmd.Parameters.AddWithValue("@desc", p.Beschrijving);
        //    cmd.Parameters.AddWithValue("@status", (int)p.Status);
        //    return (int)cmd.ExecuteScalar();
        //}

        // ===========================================================================
        // 2. OPHALEN METHODES (Verkeersregelaar)
        // ===========================================================================

        public Project GeefProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT 
                    CASE 
                        WHEN EXISTS (SELECT 1 FROM StadsOntwikkeling WHERE Id = @id) THEN 'STADS'
                        WHEN EXISTS (SELECT 1 FROM GroeneRuimte WHERE Id = @id) THEN 'GROEN'
                        WHEN EXISTS (SELECT 1 FROM InnovatieWonen WHERE Id = @id) THEN 'WONEN'
                        ELSE 'BASIS'
                    END";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                string type = (string)cmd.ExecuteScalar();

                return type switch
                {
                    "STADS" => GeefStadsOntwikkelingProject(id),
                    "GROEN" => GeefGroeneRuimteproject(id),
                    "WONEN" => GeefInnovatieWonenProject(id),
                    _ => null
                };
            }
        }

        public Stadsontwikkeling GeefStadsOntwikkelingProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, s.* FROM Project p JOIN StadsOntwikkeling s ON p.Id = s.Id WHERE p.Id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var s = new Stadsontwikkeling(r["Titel"].ToString(), (DateTime)r["StartDatum"], r["Beschrijving"].ToString(), (ProjectStatus)r["Status"], null, null, (VergunningStatus)r["VergunningStatus"], (Toegankelijkheid)r["Toegankelijkheid"], (bool)r["IsBezienswaardig"], (bool)r["HeeftInfo"], (bool)r["HeeftArchitecturaleWaarde"]);
                    s.Id = (int)r["Id"];
                    return s;
                }
            }
        }

        public GroeneRuimte GeefGroeneRuimteproject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, g.* FROM Project p JOIN GroeneRuimte g ON p.Id = g.Id WHERE p.Id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var g = new GroeneRuimte(r["Titel"].ToString(), (DateTime)r["StartDatum"], r["Beschrijving"].ToString(), (ProjectStatus)r["Status"], null, (double)r["Oppervlakte"], (double)r["Biodiversiteit"], (int)r["AantalWandelpaden"], null, (bool)r["IsInToeristWandelroute"], (double)r["Beoordeling"]);
                    g.Id = (int)r["Id"];
                    return g;
                }
            }
        }

        public InnovatieWonen GeefInnovatieWonenProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, i.* FROM Project p JOIN InnovatieWonen i ON p.Id = i.Id WHERE p.Id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var i = new InnovatieWonen(r["Titel"].ToString(), (DateTime)r["StartDatum"], r["Beschrijving"].ToString(), (ProjectStatus)r["Status"], null, (int)r["AantalWooneenheden"], (bool)r["HeeftRondleiding"], (bool)r["HeeftShowcase"], (double)r["ArchitectuurScore"], (bool)r["HeeftSamenwerkingErfgoedOfToerisme"]);
                    i.Id = (int)r["Id"];
                    return i;
                }
            }
        }

        public List<Project> GeefAlleProjecten()
        {
            List<Project> lijst = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id FROM Project";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lijst.Add(GeefProject((int)r["Id"]));
                }
            }
            return lijst;
        }

        // ===========================================================================
        // 3. FILTER METHODES
        // ===========================================================================

        public List<Project> GeefProjectFilterType(string type)
        {
            string tabel = type.ToLower() switch { "stads" => "StadsOntwikkeling", "groen" => "GroeneRuimte", "wonen" => "InnovatieWonen", _ => "Project" };
            List<Project> lijst = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = $"SELECT Id FROM {tabel}";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lijst.Add(GeefProject((int)r["Id"]));
                }
            }
            return lijst;
        }

        public List<Project> GeefProjectFilterStatus(string status)
        {
            List<Project> lijst = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id FROM Project WHERE Status = @status";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@status", status);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lijst.Add(GeefProject((int)r["Id"]));
                }
            }
            return lijst;
        }

        public List<Project> GeefProjectFilterStartDatum(DateTime startDatum1, DateTime startDatum2)
        {
            List<Project> lijst = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT Id FROM Project WHERE StartDatum BETWEEN @d1 AND @d2";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@d1", startDatum1);
                cmd.Parameters.AddWithValue("@d2", startDatum2);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read()) lijst.Add(GeefProject((int)r["Id"]));
                }
            }
            return lijst;
        }

        public List<ProjectCombinatie> GeefProjectenMetFilters(ProjectStatus? status, string? wijk, Project? project, DateTime? voorsteDatum, DateTime? laatsteDatum, string? partnerNaam) {
            if (status != null) {

            }

            string query = "SELECT * FROM Project, ";

            List<ProjectCombinatie> projectjes = new List<ProjectCombinatie>();
            return projectjes;
        }
    }
}





// ===========================================================================
// de oude versies van de methodes zijn hier nog 
// ===========================================================================

// public void VoegProjectToe(Project project) {
//     // Code om project toe te voegen aan database
//     using(sqlConnection conn = new sqlConnection(connectionstring)) {
//         // SQL code om project toe te voegen
//         // De ID wordt gegenereerd door de database (Identity)
//         string sql = "INSERT INTO Project (titel, startDatum, beschrijving, status, locatieId) " +
//                      "OUTPUT INSERTED.id VALUES (@titel, @start, @desc, @status, @locId)";

//         SqlCommand cmd = new SqlCommand(sql, conn);
//         cmd.Parameters.AddWithValue("@titel", project.Titel);
//         cmd.Parameters.AddWithValue("@start", project.StartDatum);
//         cmd.Parameters.AddWithValue("@desc", project.Beschrijving);
//         cmd.Parameters.AddWithValue("@status", project.Status);
//         cmd.Parameters.AddWithValue("@locId", (int)project.Locatie.Id);

//         conn.Open();
//         project.Id = (int)cmd.ExecuteScalar();
//     }
// }

// public List<Project> GeefAlleProjecten() {
//     List<Project> projecten = new List<Project>();
//     using (sqlConnection conn = new sqlConnection(connectionstring)) {
//         string sql = "SELECT * FROM Project";
//         SqlCommand cmd = new SqlCommand(sql, conn);
//         conn.Open();
//         SqlDataReader reader = cmd.ExecuteReader();
//         while (reader.Read()) {
//             // Code om project te maken van database gegevens
//             // Locatie moet ook opgehaald worden
//             int locId = (int)reader["locatieId"];
//             Locatie locatie = GeefLocatie(locId); // Methode om locatie op te halen
//             Project project = new Project(
//                 (string)reader["titel"],            :// meteen met index werken ipv zoeken op naam
//                 (DateTime)reader["startDatum"],
//                 (string)reader["beschrijving"],
//                 (ProjectStatus)(int)reader["status"],
//                 locatie
//             );
//             project.Id = (int)reader["id"];
//             projecten.Add(project);
//         }
//     }
//     return projecten;
// }

//public Project GeefProject(int id) {
//     Project project = null;
//     using (sqlConnection conn = new sqlConnection(connectionstring)) {
//         string sql = "SELECT * FROM Project WHERE id = @id";
//         SqlCommand cmd = new SqlCommand(sql, conn);
//         cmd.Parameters.AddWithValue("@id", id);
//         conn.Open();
//         SqlDataReader reader = cmd.ExecuteReader();
//         if (reader.Read()) {
//             int locId = (int)reader["locatieId"];
//             Locatie locatie = GeefLocatie(locId);
//             project = new Project(
//                 (string)reader["titel"],
//                 (DateTime)reader["startDatum"],
//                 (string)reader["beschrijving"],
//                 (ProjectStatus)(int)reader["status"],
//                 locatie
//             );
//             project.Id = (int)reader["id"];
//         }
//     }
//     return project;
// }

// public List<ProjectCombinatie> GeefProjectCombinaties()
// {
//     Dictionary<int, ProjectCombinatie> combinatiesDict = new Dictionary<int, ProjectCombinatie>();
//     using (SqlConnection conn = new SqlConnection(_connectionString))
//     {
//         // SQL met LEFT JOINs om alle types (Stads, Groen, Wonen) tegelijk te laden
//         string sql = @"SELECT pc.Id AS ComboId, p.*, s.VergunningStatus, s.HeeftArchitecturaleWaarde, 
//                g.Oppervlakte, i.AantalWooneenheden, i.HeeftRondleiding 
//                FROM ProjectCombinatie pc
//                JOIN Project p ON pc.Id = p.CombinatieId 
//                LEFT JOIN StadsOntwikkeling s ON p.Id = s.Id
//                LEFT JOIN GroeneRuimte g ON p.Id = g.Id
//                LEFT JOIN InnovatieWonen i ON p.Id = i.Id";

//         SqlCommand cmd = new SqlCommand(sql, conn);
//         conn.Open();
//         using (SqlDataReader reader = cmd.ExecuteReader())
//         {
//             while (reader.Read())
//             {
//                 int cId = (int)reader["ComboId"];
//                 if (!combinatiesDict.ContainsKey(cId))
//                     combinatiesDict.Add(cId, new ProjectCombinatie { Id = cId, ProjectComboLijst = new List<Project>() });

//                 Project p;
//                 if (reader["VergunningStatus"] != DBNull.Value)
//                     p = new Stadsontwikkeling { HeeftArchitecturaleWaarde = (bool)reader["HeeftArchitecturaleWaarde"] };
//                 else if (reader["Oppervlakte"] != DBNull.Value)
//                     p = new GroeneRuimte { Oppervlakte = (double)reader["Oppervlakte"] };
//                 else
//                     p = new InnovatieWonen { HeeftRondleiding = (bool)reader["HeeftRondleiding"] };

//                 p.Id = (int)reader["Id"];
//                 p.Titel = reader["Titel"].ToString();
//                 combinatiesDict[cId].ProjectComboLijst.Add(p);
//             }
//         }
//     }
//     return combinatiesDict.Values.ToList();
// }

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
//}
//}


