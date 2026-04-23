using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums; 
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


        // OPHALEN METHODES 



       



        public Stadsontwikkeling GeefStadsOntwikkelingProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, s.* FROM Project p JOIN StadsOntwikkeling s ON p.id = s.id WHERE p.id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var s = new Stadsontwikkeling(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), (ProjectStatus)r["status"], null, null, (VergunningStatus)r["verguningsStatus"], (Toegankelijkheid)r["toegankelijkheid"], (bool)r["bezienswaardigheid"], (bool)r["info"], (bool)r["archtitectueleWaarde"]);
                    s.Id = (int)r["id"];
                    return s;
                }
            }
        }

        public GroeneRuimte GeefGroeneRuimteproject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, g.* FROM Project p JOIN GroeneRuimte g ON p.id = g.id WHERE p.id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var g = new GroeneRuimte(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), (ProjectStatus)r["status"], null, (double)r["oppervlakte"], (double)r["biodiversiteit"], (int)r["aantalWandelpaden"], null, (bool)r["inToeristischeWandelroute"], (double)r["beoordeling"]);
                    g.Id = (int)r["id"];
                    return g;
                }
            }
        }
       
    

        public InnovatieWonen GeefInnovatieWonenProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT p.*, i.* FROM Project p JOIN InnovatieWonen i ON p.id = i.id WHERE p.id = @id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    var i = new InnovatieWonen(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), (ProjectStatus)r["status"], null, (int)r["aantalWooneenheden"], (bool)r["rondleiding"], (bool)r["showwoning"], (double)r["architectuurInnovatieScore"], (bool)r["samenwerkingErfgoedOfToerisme"]);
                    i.Id = (int)r["id"];
                    return i;
                }
            }
        }


        public Project GeefProject(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                string sql = @"SELECT p.*, l.*, 
                              s.verguningsStatus, s.toegankelijkheid, s.bezienswaardigheid, s.info, s.archtitectueleWaarde,
                              g.oppervlakte, g.biodiversiteit, g.aantalWandelpaden, g.inToeristischeWandelroute, g.beoordeling,
                              i.aantalWooneenheden, i.rondleiding, i.showwoning, i.architectuurInnovatieScore, i.samenwerkingErfgoedOfToerisme,
                              pr.naam AS PartnerNaam, pr.email AS PartnerEmail, pp.rol
                       FROM Project p 
                       JOIN Locatie l ON p.locatieId = l.id
                       LEFT JOIN StadsOntwikkeling s ON p.id = s.projectId
                       LEFT JOIN GroeneRuimte g ON p.id = g.projectId
                       LEFT JOIN InnovatieWonen i ON p.id = i.projectId
                       LEFT JOIN ProjectPartner pp ON p.id = pp.projectId
                       LEFT JOIN Partner pr ON pp.partnerId = pr.id
                       WHERE p.id = @id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    Locatie loc = new Locatie(
                        (int)r["locatieId"],
                        r["wijk"].ToString(),
                        r["straat"].ToString(),
                        r["gemeente"].ToString(),
                        (int)r["postcode"],
                        r["huisnummer"].ToString()
                    );

                    ProjectStatus status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), r["status"].ToString());
                    Project projectResult = null;

                   
                    if (r["verguningsStatus"] != DBNull.Value)
                    {
                        projectResult = new Stadsontwikkeling(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, null,
                            (VergunningStatus)Enum.Parse(typeof(VergunningStatus), r["verguningsStatus"].ToString()),
                            (Toegankelijkheid)Enum.Parse(typeof(Toegankelijkheid), r["toegankelijkheid"].ToString()),
                            (bool)r["bezienswaardigheid"], (bool)r["info"], (bool)r["archtitectueleWaarde"])
                        { Id = id };
                    }
                    else if (r["oppervlakte"] != DBNull.Value)
                    {
                        projectResult = new GroeneRuimte(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (double)r["oppervlakte"], (double)r["biodiversiteit"], (int)r["aantalWandelpaden"], null, (bool)r["inToeristischeWandelroute"], (double)r["beoordeling"]) { Id = id };
                    }
                    else
                    {
                        projectResult = new InnovatieWonen(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (int)r["aantalWooneenheden"], (bool)r["rondleiding"], (bool)r["showwoning"], (double)r["architectuurInnovatieScore"], (bool)r["samenwerkingErfgoedOfToerisme"]) { Id = id };
                    }

                    
                    do
                    {
                        if (r["PartnerNaam"] != DBNull.Value)
                        { 
                            
                            
                            Partner p = new Partner(r["PartnerNaam"].ToString(), r["PartnerEmail"].ToString(), null);
                            string rol = r["rol"].ToString();

                            var existingPartner = projectResult.ProjectPartners.Keys.FirstOrDefault(k => k.Naam == p.Naam);

                            if (existingPartner == null)
                            {
                                projectResult.ProjectPartners.Add(p, new List<string> { rol });
                            }
                            else
                            {
                                if (!projectResult.ProjectPartners[existingPartner].Contains(rol))
                                {
                                    projectResult.ProjectPartners[existingPartner].Add(rol);
                                }
                            }
                        }
                    } while (r.Read()); 

                    return projectResult;
                }
            }
        }


        


        public List<ProjectCombinatie> GeefAlleProjecten()
        {
            List<ProjectCombinatie> resultaat = new List<ProjectCombinatie>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "SELECT id FROM Project";
                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        // We maken voor elk project een virtuele combinatie aan voor de UI
                        var project = GeefProject((int)r["id"]);
                        var combo = new ProjectCombinatie { Id = project.Id };
                        combo.ProjectComboLijst.Add(project);
                        resultaat.Add(combo);
                    }
                }
            }
            return resultaat;
        }



        public List<Project> GeefProjectenMetFilters(string? type, string? wijk, ProjectStatus? status, DateTime? start, DateTime? eind, string? partner)
        {
            List<Project> gefilterdeLijst = new List<Project>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                string sql = @"SELECT p.*, l.*, 
                              s.verguningsStatus, s.toegankelijkheid, s.bezienswaardigheid, s.info, s.archtitectueleWaarde,
                              g.oppervlakte, g.biodiversiteit, g.aantalWandelpaden, g.inToeristischeWandelroute, g.beoordeling,
                              i.aantalWooneenheden, i.rondleiding, i.showwoning, i.architectuurInnovatieScore, i.samenwerkingErfgoedOfToerisme
                       FROM Project p 
                       JOIN Locatie l ON p.locatieId = l.id
                       LEFT JOIN StadsOntwikkeling s ON p.id = s.projectId
                       LEFT JOIN GroeneRuimte g ON p.id = g.projectId
                       LEFT JOIN InnovatieWonen i ON p.id = i.projectId 
                       LEFT JOIN ProjectPartner pp ON p.id = pp.projectId
                       LEFT JOIN Partner pr ON pp.partnerId = pr.id
                       WHERE 1=1";

                SqlCommand cmd = new SqlCommand { Connection = conn };

    
                if (!string.IsNullOrEmpty(type))
                {
                    if (type.ToLower().Contains("stads")) sql += " AND s.projectId IS NOT NULL";
                    else if (type.ToLower().Contains("groen")) sql += " AND g.projectId IS NOT NULL";
                    else if (type.ToLower().Contains("woon")) sql += " AND i.projectId IS NOT NULL";
                }
                if (!string.IsNullOrEmpty(wijk)) { sql += " AND (l.gemeente LIKE @loc OR l.wijk LIKE @loc)"; cmd.Parameters.AddWithValue("@loc", "%" + wijk + "%"); }
                if (status.HasValue) { sql += " AND p.status = @status"; cmd.Parameters.AddWithValue("@status", status.Value.ToString()); }

                cmd.CommandText = sql;
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
       
                        int id = (int)r["id"];

                       
                        if (!gefilterdeLijst.Any(p => p.Id == id))
                        {
                            Locatie loc = new Locatie(
                                (int)r["locatieId"],
                                r["wijk"].ToString(),
                                r["straat"].ToString(),
                                r["gemeente"].ToString(),
                                (int)r["postcode"],
                                r["huisnummer"].ToString()
                            ); 

                    Project p = null;
                            ProjectStatus pStatus = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), r["status"].ToString());

                    if (r["verguningsStatus"] != DBNull.Value)
                                p = new Stadsontwikkeling(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), pStatus, loc, null, (VergunningStatus)Enum.Parse(typeof(VergunningStatus), r["verguningsStatus"].ToString()), (Toegankelijkheid)Enum.Parse(typeof(Toegankelijkheid), r["toegankelijkheid"].ToString()), (bool)r["bezienswaardigheid"], (bool)r["info"], (bool)r["archtitectueleWaarde"]) { Id = id };
                            else if (r["oppervlakte"] != DBNull.Value)
                                p = new GroeneRuimte(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), pStatus, loc, (double)r["oppervlakte"], (double)r["biodiversiteit"], (int)r["aantalWandelpaden"], null, (bool)r["inToeristischeWandelroute"], (double)r["beoordeling"]) { Id = id };
                            else
                                p = new InnovatieWonen(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), pStatus, loc, (int)r["aantalWooneenheden"], (bool)r["rondleiding"], (bool)r["showwoning"], (double)r["architectuurInnovatieScore"], (bool)r["samenwerkingErfgoedOfToerisme"]) { Id = id }; 

                    gefilterdeLijst.Add(p);
                        }
                    }
                }
            }
            return gefilterdeLijst;
        }

        
    }

}





