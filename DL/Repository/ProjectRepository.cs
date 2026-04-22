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

        
        // TOEVOEGEN METHODES 
       

        public void VoegProjectToe(Project project)
        {
            if (project is Stadsontwikkeling s) VoegStadsOntwikkelingToe(s);
            else if (project is GroeneRuimte g) VoegGroeneRuimteToe(g);
            else if (project is InnovatieWonen i) VoegInnovatieWonenToe(i);
            else throw new Exception("Onbekend projecttype.");
        }

        public void VoegStadsOntwikkelingToe(Stadsontwikkeling s)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //int id = InvoegenBasisProject(s, conn, trans);
                        string sql = @"INSERT INTO StadsOntwikkeling (vergunningStatus, toegankelijkheid, bezienswaardigheid, info, archtitectueleWaarde) 
                                       VALUES (@verg, @toeg, @beziens, @info, @arch)";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        //cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@verg", (int)s.VergunningStatus);
                        cmd.Parameters.AddWithValue("@toeg", (int)s.Toegankelijkheid);
                        cmd.Parameters.AddWithValue("@beziens", s.IsBezienswaardig);
                        cmd.Parameters.AddWithValue("@info", s.HeeftInfo);
                        cmd.Parameters.AddWithValue("@arch", s.HeeftArchitecturaleWaarde);
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }

        public void VoegGroeneRuimteToe(GroeneRuimte g)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //int id = InvoegenBasisProject(g, conn, trans);
                        string sql = @"INSERT INTO GroeneRuimte (Id, Oppervlakte, Biodiversiteit, AantalWandelpaden, IsInToeristWandelroute, Beoordeling) 
                                       VALUES (@id, @opp, @bio, @wandel, @route, @beoordeling)";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        //cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@opp", g.Oppervlakte);
                        cmd.Parameters.AddWithValue("@bio", g.Biodiversiteit);
                        cmd.Parameters.AddWithValue("@wandel", g.AantalWandelpaden);
                        cmd.Parameters.AddWithValue("@route", g.IsInToeristWandelroute);
                        cmd.Parameters.AddWithValue("@beoordeling", g.Beoordeling);
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
                }
            }
        }

        public void VoegInnovatieWonenToe(InnovatieWonen i)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        //int id = InvoegenBasisProject(i, conn, trans);
                        string sql = @"INSERT INTO InnovatieWonen (aantalWooneenheden, rondleiding, showwoning, ArchitectuurScore, HeeftSamenwerkingErfgoedOfToerisme) 
                                       VALUES (@aantal, @rond, @show, @score, @samen)";
                        SqlCommand cmd = new SqlCommand(sql, conn, trans);
                        //cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@aantal", i.AantalWooneenheden);
                        cmd.Parameters.AddWithValue("@rond", i.HeeftRondleiding);
                        cmd.Parameters.AddWithValue("@show", i.HeeftShowcase);
                        cmd.Parameters.AddWithValue("@score", i.ArchitectuurScore);
                        cmd.Parameters.AddWithValue("@samen", i.HeeftSamenwerkingErfgoedOfToerisme);
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                    }
                    catch { trans.Rollback(); throw; }
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


        //public Project GeefProject(int id)
        //{
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {

        //        string sql = @"SELECT p.*, l.*, 
        //                              s.verguningsStatus, s.toegankelijkheid, s.bezienswaardigheid, s.info, s.archtitectueleWaarde,
        //                              g.oppervlakte, g.biodiversiteit, g.aantalWandelpaden, g.inToeristischeWandelroute, g.beoordeling,
        //                              i.aantalWooneenheden, i.rondleiding, i.showwoning, i.architectuurInnovatieScore, i.samenwerkingErfgoedOfToerisme
        //                       FROM Project p 
        //                       JOIN Locatie l ON p.locatieId = l.id
        //                       LEFT JOIN StadsOntwikkeling s ON p.id = s.projectId
        //                       LEFT JOIN GroeneRuimte g ON p.id = g.projectId
        //                       LEFT JOIN InnovatieWonen i ON p.id = i.projectId
        //                       WHERE p.id = @id";

        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        cmd.Parameters.AddWithValue("@id", id);
        //        conn.Open();

        //        using (SqlDataReader r = cmd.ExecuteReader())
        //        {
        //            if (!r.Read()) return null;

        //            Locatie loc = new Locatie(
        //                (int)r["locatieId"],
        //                r["wijk"].ToString(),
        //                r["straat"].ToString(),
        //                r["gemeente"].ToString(),
        //                (int)r["postcode"],
        //                r["huisnummer"].ToString()
        //            );


        //            ProjectStatus status = (ProjectStatus)Enum.Parse(typeof(ProjectStatus), r["status"].ToString());


        //            if (r["verguningsStatus"] != DBNull.Value)
        //            {
        //                return new Stadsontwikkeling(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, null,
        //                    (VergunningStatus)Enum.Parse(typeof(VergunningStatus), r["verguningsStatus"].ToString()),
        //                    (Toegankelijkheid)Enum.Parse(typeof(Toegankelijkheid), r["toegankelijkheid"].ToString()),
        //                    (bool)r["bezienswaardigheid"], (bool)r["info"], (bool)r["archtitectueleWaarde"])
        //                { Id = id };
        //            }
        //            else if (r["oppervlakte"] != DBNull.Value)
        //            {
        //                return new GroeneRuimte(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (double)r["oppervlakte"], (double)r["biodiversiteit"], (int)r["aantalWandelpaden"], null, (bool)r["inToeristischeWandelroute"], (double)r["beoordeling"]) { Id = id };
        //            }
        //            else
        //            {
        //                return new InnovatieWonen(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (int)r["aantalWooneenheden"], (bool)r["rondleiding"], (bool)r["showwoning"], (double)r["architectuurInnovatieScore"], (bool)r["samenwerkingErfgoedOfToerisme"]) { Id = id };
        //            }
        //        }
        //    }
        //}


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
                string sql = @"SELECT DISTINCT p.id FROM Project p 
                               LEFT JOIN Locatie l ON p.locatieId = l.id
                               LEFT JOIN ProjectPartner pp ON p.id = pp.projectId
                               LEFT JOIN Partner pr ON pp.partnerId = pr.id
                               LEFT JOIN StadsOntwikkeling s ON p.id = s.projectId
                               LEFT JOIN GroeneRuimte g ON p.id = g.projectId
                               LEFT JOIN InnovatieWonen i ON p.id = i.projectId WHERE 1=1";

                SqlCommand cmd = new SqlCommand { Connection = conn };

                if (!string.IsNullOrEmpty(type))
                {
                    if (type.ToLower().Contains("stads")) sql += " AND s.id IS NOT NULL";
                    else if (type.ToLower().Contains("groen")) sql += " AND g.id IS NOT NULL";
                    else if (type.ToLower().Contains("woon")) sql += " AND i.id IS NOT NULL";
                }
                if (!string.IsNullOrEmpty(wijk)) { sql += " AND l.wijk LIKE @wijk"; cmd.Parameters.AddWithValue("@wijk", "%" + wijk + "%"); }
                if (status.HasValue) { sql += " AND p.status = @status"; cmd.Parameters.AddWithValue("@status", status.Value.ToString()); }
                if (start.HasValue) { sql += " AND p.startDatum >= @start"; cmd.Parameters.AddWithValue("@start", start.Value); }
                if (eind.HasValue) { sql += " AND p.startDatum <= @eind"; cmd.Parameters.AddWithValue("@eind", eind.Value); }
                if (!string.IsNullOrEmpty(partner)) { sql += " AND pr.naam LIKE @p"; cmd.Parameters.AddWithValue("@p", "%" + partner + "%"); }

                cmd.CommandText = sql;
                conn.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    List<int> ids = new List<int>();
                    while (r.Read()) ids.Add((int)r["id"]);
                    r.Close();
                    foreach (int id in ids) gefilterdeLijst.Add(GeefProject(id));
                }
            }
            return gefilterdeLijst;
        }
    }


    






    //public List<ProjectCombinatie> GeefProjectenMetFilters(ProjectStatus? status, string? wijk, Project? project, DateTime? voorsteDatum, DateTime? laatsteDatum, string? partnerNaam) {
    //    if (status != null) {

    //    }

    //    string query = "SELECT * FROM Project, ";

    //    List<ProjectCombinatie> projectjes = new List<ProjectCombinatie>();
    //    return projectjes;
    //}
}





