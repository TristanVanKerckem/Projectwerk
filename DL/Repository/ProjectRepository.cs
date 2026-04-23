using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Domein.Enums;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.NetworkInformation;

namespace ProjectbeheerDL.Repository
{
    public class ProjectRepo : IProjectRepository
    {
        private readonly string _connectionString;

        public ProjectRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        // returned int om onze ProjectId te kunnen ophalen in de GebruikerVoegtProjectToe methode in de AdminRepo
        // gebruik maken van IDBConnection en IDBTransaction ipv van sqlConn & Trans --> Businesslaag moeten we gescheiden blijven houden
        // wordt ook gebruikt bij de kindklassen voor consistent te blijven
        // Ook interessante keuze voor uitbreiding/verandering --> repository is makkelijker herbruikbaar indien verandering van gebruikte server a.d.h.v. onze factories
        public int VoegProjectToe(Project project, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection interfaceConn, IDbTransaction interfaceTrans) 
        {
            int databaseId;
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            if (project is Stadsontwikkeling s) databaseId = VoegStadsOntwikkelingToe(s, project, s.Locatie, s.Bouwfirmas, partner, locPartner, rollen, conn, trans);
            else if (project is GroeneRuimte g) databaseId = VoegGroeneRuimteToe(g, project, g.Locatie, g.BeschikbareFaciliteiten, partner, locPartner, rollen, conn, trans);
            else if (project is InnovatieWonen i) databaseId = VoegInnovatieWonenToe(i, project, i.Locatie, i.WoonvormTypes, partner, locPartner, rollen, conn, trans);
            else throw new Exception("Onbekend projecttype.");

            return databaseId;
        }

        // Belangrijk om apart toe te voegen --> Je kan geen connectie openen als er al 1 geopend is, anders kunnen we geen check doen of er al algemene ProjectInfo in de Database ingevuld staat door een ander kindproject
        private int VoegProjectInfoToe(Project p, Locatie l, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection interfaceConn, IDbTransaction interfaceTrans) {
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryProject = "INSERT INTO Project (titel, startDatum, beschrijving, status, locatieId) VALUES (@titel, @startDatum, @beschrijving, @status, @locatieId); SELECT SCOPE_IDENTITY();";
            // We gaan voor nu de Locatie al hier aanmaken, als het software project uitgewerkt wordt en we voegen Locatie toe voor een Partner best in aparte Repository
            // Op dit moment ook wordt er telkens een nieuwe Locatie aangemaakt, er wordt niet gecheckt of deze al in de database staat
            string queryLocatie = "INSERT INTO Locatie (wijk, straat, gemeente, postcode, huisnummer) VALUES (@wijk, @straat, @gemeente, @postcode, @huisnummer); SELECT SCOPE_IDENTITY();";
            string queryPartner = "INSERT INTO Partner (naam, email, locatieId) VALUES (@naam, @email, @locatieId); SELECT SCOPE_IDENTITY();";
            string queryProjectPartner = "INSERT INTO ProjectPartner (rol, projectId, partnerId) VALUES (@rol, @projectId, @partnerId)";
            using (SqlCommand cmd1 = new SqlCommand(queryProject, conn, trans))
            using (SqlCommand cmd2 = new SqlCommand(queryLocatie, conn, trans))
            using (SqlCommand cmd3 = new SqlCommand(queryPartner, conn, trans))
            using (SqlCommand cmd4 = new SqlCommand(queryProjectPartner, conn, trans)) {
                try {
                    int databaseLocatieId_Project;
                    int databaseProjectId;
                    int databaseLocatieId_Partner;
                    int databasePartnerId;

                    // Voeg Locatie van Project Toe
                    cmd2.Parameters.Clear();
                    cmd2.Parameters.AddWithValue("@wijk", l.Wijk);
                    cmd2.Parameters.AddWithValue("@straat", l.Straat);
                    cmd2.Parameters.AddWithValue("@gemeente", l.Gemeente);
                    cmd2.Parameters.AddWithValue("@postcode", l.Postcode);
                    cmd2.Parameters.AddWithValue("@huisnummer", l.HuisNummer);

                    databaseLocatieId_Project = Convert.ToInt32(cmd2.ExecuteScalar());
                    // Voeg Project Toe
                    cmd1.Parameters.Clear();
                    cmd1.Parameters.AddWithValue("@titel", p.Titel);
                    cmd1.Parameters.AddWithValue("@startDatum", p.StartDatum);
                    cmd1.Parameters.AddWithValue("@beschrijving", p.Beschrijving);
                    cmd1.Parameters.AddWithValue("@status", p.Status);
                    cmd1.Parameters.AddWithValue("@locatieId", databaseLocatieId_Project);
                    databaseProjectId = Convert.ToInt32(cmd1.ExecuteScalar());

                    // Voeg Partner,Vestiging Partner & Rol(len) Toe
                    if (partner != null && locPartner != null) {
                        // Vestiging
                        cmd2.Parameters.Clear();
                        cmd2.Parameters.AddWithValue("@wijk", locPartner.Wijk);
                        cmd2.Parameters.AddWithValue("@straat", locPartner.Straat);
                        cmd2.Parameters.AddWithValue("@gemeente", locPartner.Gemeente);
                        cmd2.Parameters.AddWithValue("@postcode", locPartner.Postcode);
                        cmd2.Parameters.AddWithValue("@huisnummer", locPartner.HuisNummer);
                        databaseLocatieId_Partner = Convert.ToInt32(cmd2.ExecuteScalar());

                        // Partner
                        cmd3.Parameters.Clear();
                        cmd3.Parameters.AddWithValue("@naam", partner.Naam);
                        cmd3.Parameters.AddWithValue("@email", partner.Email);
                        cmd3.Parameters.AddWithValue("@locatieId", databaseLocatieId_Partner);
                        databasePartnerId = Convert.ToInt32(cmd3.ExecuteScalar());

                        // ProjectPartner met rol
                        foreach (string rol in rollen) {
                            cmd4.Parameters.Clear();
                            cmd4.Parameters.AddWithValue("@rol", rol);
                            cmd4.Parameters.AddWithValue("@projectId", databaseProjectId);
                            cmd4.Parameters.AddWithValue("@partnerId", databasePartnerId);
                            cmd4.ExecuteNonQuery();
                        }
                    }
                    return databaseProjectId;
                } catch (Exception ex) {

                    throw new Exception();
                }
            }
        }

        // Nodig voor te checken of ProjectInfo al is aangemaakt door een kindklasse van hetzelfde Project
        private bool ProjectInfoBestaat(int projectId, IDbConnection interfaceConn, IDbTransaction interfaceTrans)
        { //conn en trans meegeven, anders error door de connectie als je nieuwe start
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string query = "SELECT COUNT(1) FROM Project WHERE id = @projectId";
            using (SqlCommand cmd = new SqlCommand(query, conn, trans))
            {
                try
                {
                    cmd.Parameters.AddWithValue("@projectId", projectId);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }
        }

        public bool BestaatBouwfirma(string naam, IDbConnection conn, IDbTransaction trans) {
            string query = "SELECT COUNT(*) FROM Bouwfirma WHERE naam = @naam";
            using (SqlCommand cmd = new SqlCommand(query, (SqlConnection)conn, (SqlTransaction)trans)) {
                cmd.Parameters.AddWithValue("@naam", naam);
                int aantal = (int)cmd.ExecuteScalar();
                return aantal > 0;
            }
        }

        // ophalen om te checken of Bouwfirma al in de Databank staat, en ook indien deze er al in staat dat we deze gebruiken
        public int? HaalBouwfirmaIdOp(string naam, IDbConnection con, IDbTransaction transaction) {
            string query = "SELECT id FROM BouwFirma WHERE naam = @naam";

            using (SqlCommand cmd = new SqlCommand(query, (SqlConnection)con, (SqlTransaction)transaction)) {
                cmd.Parameters.AddWithValue("@naam", naam);
                object result = cmd.ExecuteScalar();

                // Als result null is, bestaat de firma niet
                if (result != null && result != DBNull.Value) {
                    return Convert.ToInt32(result);
                }
                return null;
            }
        }

        private int VoegStadsOntwikkelingToe(Stadsontwikkeling s, Project p, Locatie l, List<Bouwfirma> bouwfirmas, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection interfaceConn, IDbTransaction interfaceTrans) {
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            try
            {
                int databaseProjectId;

                // check of Project al bestaat
                bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                // Toevoegen indien deze nog niet bestaat
                if (!projectBestaatInDataBank) {
                    databaseProjectId = VoegProjectInfoToe(p, l, partner, locPartner, rollen, conn, trans); // return de id + vult Project verder aan
                } else {
                    databaseProjectId = p.Id;
                }
                //int id = InvoegenBasisProject(s, conn, trans);
                string queryStadsOntwikkeling = @"INSERT INTO StadsOntwikkeling (verguningsStatus, archtitectueleWaarde, toegankelijkheid, bezienswaardigheid, info, projectId) 
                                                          VALUES (@verg,@arch, @toeg, @beziens, @info, @projectId); SELECT SCOPE_IDENTITY();";
                int stadsOntwikkelingId;

                string queryBouwfirma = "INSERT INTO BouwFirma (naam, email, telefoon) VALUES (@naam, @email, @telefoon); SELECT SCOPE_IDENTITY();";
                int bouwfirmaId;

                string queryKoppeltabelStadBouw = "INSERT INTO Bouwfirma_Stadsontwikkeling (bouwfirmaid, stadsontwikkelingsid) VALUES (@bouwfirmaId, @stadsontwikkelingsId)";

                using (SqlCommand cmd1 = new SqlCommand(queryStadsOntwikkeling, conn, trans))
                using (SqlCommand cmd2 = new SqlCommand(queryBouwfirma, conn, trans))
                using (SqlCommand cmd3 = new SqlCommand(queryKoppeltabelStadBouw, conn, trans))
                {
                    //cmd.Parameters.AddWithValue("@id", id);
                    // Aanvullen Stadsontwikkeling
                    cmd1.Parameters.AddWithValue("@verg", s.VergunningStatus);
                    cmd1.Parameters.AddWithValue("@toeg", s.Toegankelijkheid);
                    cmd1.Parameters.AddWithValue("@beziens", s.IsBezienswaardig);
                    cmd1.Parameters.AddWithValue("@info", s.HeeftInfo);
                    cmd1.Parameters.AddWithValue("@arch", s.HeeftArchitecturaleWaarde);
                    cmd1.Parameters.AddWithValue("@projectId", databaseProjectId);
                    stadsOntwikkelingId = Convert.ToInt32(cmd1.ExecuteScalar());

                    // Aanvullen Bouwfirma
                    foreach (Bouwfirma bouwfirma in bouwfirmas)
                    {
                        if (!BestaatBouwfirma(bouwfirma.Naam, conn, trans)) {
                            cmd2.Parameters.Clear(); // Belangrijk voor te kunnen loopen
                            cmd2.Parameters.AddWithValue("@naam", bouwfirma.Naam);
                            cmd2.Parameters.AddWithValue("@email", bouwfirma.Email);
                            cmd2.Parameters.AddWithValue("@telefoon", bouwfirma.TelefoonNummer);
                            bouwfirmaId = Convert.ToInt32(cmd2.ExecuteScalar());
                        } else {
                            bouwfirmaId = (int)HaalBouwfirmaIdOp(bouwfirma.Naam, conn, trans);
                        }

                        

                        // Koppeltabel aanvullen tussen BF & GR
                        cmd3.Parameters.Clear();
                        cmd3.Parameters.AddWithValue("@bouwfirmaId", bouwfirmaId);
                        cmd3.Parameters.AddWithValue("@stadsontwikkelingsId", stadsOntwikkelingId);
                        cmd3.ExecuteNonQuery();
                    }
                }

                return databaseProjectId;
            }
            catch { throw; }

        }


        private int VoegGroeneRuimteToe(GroeneRuimte g, Project p, Locatie l, List<BeschikbareFaciliteiten> faciliteiten, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection interfaceConn, IDbTransaction interfaceTrans) {
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            try
            {
                int databaseProjectId;

                // check of Project al bestaat
                bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                // Toevoegen indien deze nog niet bestaat
                if (!projectBestaatInDataBank) {
                    databaseProjectId = VoegProjectInfoToe(p, l, partner, locPartner, rollen, conn, trans); // return de id + vult Project verder aan
                } else {
                    databaseProjectId = p.Id;
                }
                //int id = InvoegenBasisProject(g, conn, trans);
                string queryGroeneRuimte = @"INSERT INTO GroeneRuimte (oppervlakte, biodiversiteit, aantalWandelpaden, inToeristischeWandelroute, beoordeling, projectId) 
                                                     VALUES (@opp, @bio, @wandel, @route, @beoordeling, @projectId); SELECT SCOPE_IDENTITY();";
                int groeneRuimteId;

                string queryBeschikbareFaciliteit = "INSERT INTO BeschikbareFaciliteit (type, isGeverifieerd) VALUES (@type, @isGeverifieerd); SELECT SCOPE_IDENTITY();";
                int faciliteitId;

                string queryKoppeltabelGroenFac = "INSERT INTO GroeneRuimte_Faciliteit (groeneRuimteid, faciliteitId) VALUES (@groeneRuimteId, @faciliteitId)";

                using (SqlCommand cmd1 = new SqlCommand(queryGroeneRuimte, conn, trans))
                //using (SqlCommand cmd2 = new SqlCommand(queryBeschikbareFaciliteit, conn, trans))     --> al ingevuld op dit moment
                using (SqlCommand cmd3 = new SqlCommand(queryKoppeltabelGroenFac, conn, trans))
                {
                    //cmd.Parameters.AddWithValue("@id", id);
                    // GroeneRuimte aanvullen
                    cmd1.Parameters.AddWithValue("@opp", g.Oppervlakte);
                    cmd1.Parameters.AddWithValue("@bio", g.Biodiversiteit);
                    cmd1.Parameters.AddWithValue("@wandel", g.AantalWandelpaden);
                    cmd1.Parameters.AddWithValue("@route", g.IsInToeristWandelroute);
                    cmd1.Parameters.AddWithValue("@beoordeling", g.Beoordeling);
                    cmd1.Parameters.AddWithValue("@projectId", databaseProjectId);
                    groeneRuimteId = Convert.ToInt32(cmd1.ExecuteScalar()); // Zo vragen we de actuele id op

                    // BeschikbareFaciliteiten aanvullen
                    foreach (BeschikbareFaciliteiten faciliteit in faciliteiten)
                    { // IN COMMENT GEZET WANT VOOR NU ZETTEN WE EEN AANTAL VASTE WAARDEN IN DATABANK DIE NIET VERANDEREN
                        //cmd2.Parameters.Clear();
                        //cmd2.Parameters.AddWithValue("@type", faciliteit.Naam);
                        //cmd2.Parameters.AddWithValue("@isGeverifieerd", faciliteit.IsGeverifieerd);
                        //faciliteitId = Convert.ToInt32(cmd2.ExecuteScalar());

                        // Koppeltabel aanvullen tussen BF & GR
                        cmd3.Parameters.Clear();
                        cmd3.Parameters.AddWithValue("@groeneRuimteId", groeneRuimteId);
                        cmd3.Parameters.AddWithValue("@faciliteitId", faciliteit.Id); // Indien we toch waarden toevoegen in een latere versie --> faciliteitId hier laten
                        cmd3.ExecuteNonQuery();
                    }

                }

                return databaseProjectId;
            }
            catch { throw; }


        }

        private int VoegInnovatieWonenToe(InnovatieWonen i, Project p, Locatie l, List<WoonvormType> woonvormTypes, Partner? partner, Locatie? locPartner, List<string> rollen, IDbConnection interfaceConn, IDbTransaction interfaceTrans) {
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            try
            {
                int databaseProjectId;
                // check of Project al bestaat
                bool projectBestaatInDataBank = ProjectInfoBestaat(p.Id, conn, trans);

                // Toevoegen indien deze nog niet bestaat
                if (!projectBestaatInDataBank) {
                    databaseProjectId = VoegProjectInfoToe(p, l, partner, locPartner, rollen, conn, trans); // return de id + vult Project verder aan
                } else {
                    databaseProjectId = p.Id;
                }

                //int id = InvoegenBasisProject(i, conn, trans);
                string queryInnovatieWonen = @"INSERT INTO InnovatieWonen (aantalWooneenheden, rondleiding, showwoning, architectuurInnovatieScore, samenwerkingErfgoedOfToerisme, projectId) 
                                                       VALUES (@aantal, @rond, @show, @score, @samen, @projectId); SELECT SCOPE_IDENTITY();";
                int innovatieWonenId;

                string queryWoonvormType = "INSERT INTO WoonvormType (naam, isGeverifieerd) VALUES (@naam, @isGeverifieerd); SELECT SCOPE_IDENTITY();";
                int woonvormTypeId;

                string queryKoppeltabelInnoWoon = "INSERT INTO WoonvormType_InnovatieWonen (woonvormTypeId, InnovatieWonenId) VALUES (@woonvormTypeId, @innovatieWonenId)";

                using (SqlCommand cmd1 = new SqlCommand(queryInnovatieWonen, conn, trans))
                //using (SqlCommand cmd2 = new SqlCommand(queryWoonvormType, conn, trans))
                using (SqlCommand cmd3 = new SqlCommand(queryKoppeltabelInnoWoon, conn, trans))
                {

                    //cmd.Parameters.AddWithValue("@id", id);
                    // Innovatie Woning aanvullen
                    cmd1.Parameters.AddWithValue("@aantal", i.AantalWooneenheden);
                    cmd1.Parameters.AddWithValue("@rond", i.HeeftRondleiding);
                    cmd1.Parameters.AddWithValue("@show", i.HeeftShowcase);
                    cmd1.Parameters.AddWithValue("@score", i.ArchitectuurScore);
                    cmd1.Parameters.AddWithValue("@samen", i.HeeftSamenwerkingErfgoedOfToerisme);
                    cmd1.Parameters.AddWithValue("@projectId", databaseProjectId);
                    innovatieWonenId = Convert.ToInt32(cmd1.ExecuteScalar());

                    foreach (WoonvormType woonvormType in woonvormTypes)
                    { // idem als bij groene woning
                        //cmd2.Parameters.Clear();
                        //cmd2.Parameters.AddWithValue("@naam", woonvormType.Naam);
                        //cmd2.Parameters.AddWithValue("@isGeverifieerd", woonvormType.IsGeverifieerd);
                        //woonvormTypeId = Convert.ToInt32(cmd2.ExecuteScalar());

                        // Koppeltabel aanvullen tussen IW & WT
                        cmd3.Parameters.Clear();
                        cmd3.Parameters.AddWithValue("@innovatieWonenId", innovatieWonenId);
                        cmd3.Parameters.AddWithValue("@woonvormTypeId", woonvormType.Id); // idem als bij groene woning
                        cmd3.ExecuteNonQuery();
                    }
                }

                return databaseProjectId;
            }
            catch { throw; }


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

        public List<string> GeefBeschikbareFaciliteiten()
        {
            List<string> faciliteiten = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT type FROM BeschikbareFaciliteit";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        faciliteiten.Add(reader["type"].ToString());
                    }
                }
            }
            return faciliteiten;
        }

        public List<string> GeefBouwfirmas()
        {
            List<string> firmas = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT naam FROM Bouwfirma";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        firmas.Add(reader["naam"].ToString());
                    }
                }
            }
            return firmas;
        }

        public List<string> GeefWoonvormTypes()
        {
            List<string> types = new List<string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT naam FROM WoonvormType ORDER BY naam ASC";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        types.Add(reader["naam"].ToString());
                    }
                }
            }
            return types;
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





        //public List<ProjectCombinatie> GeefAlleProjecten()
        //{
        //    List<ProjectCombinatie> resultaat = new List<ProjectCombinatie>();
        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        string sql = "SELECT id FROM Project";
        //        SqlCommand cmd = new SqlCommand(sql, conn);
        //        conn.Open();
        //        using (SqlDataReader r = cmd.ExecuteReader())
        //        {
        //            while (r.Read())
        //            {
        //                // We maken voor elk project een virtuele combinatie aan voor de UI
        //                var project = GeefProject((int)r["id"]);
        //                var combo = new ProjectCombinatie { Id = project.Id };
        //                combo.ProjectComboLijst.Add(project);
        //                resultaat.Add(combo);
        //            }
        //        }
        //    }
        //    return resultaat;
        //}
        public List<ProjectCombinatie> GeefAlleProjecten()
        {
            List<ProjectCombinatie> resultaat = new List<ProjectCombinatie>();

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
                       LEFT JOIN Partner pr ON pp.partnerId = pr.id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();

                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
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
                        if (r["verguningsStatus"] != DBNull.Value)
                        {
                            Stadsontwikkeling projectResult = new Stadsontwikkeling(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, null,
                                (VergunningStatus)Enum.Parse(typeof(VergunningStatus), r["verguningsStatus"].ToString()),
                                (Toegankelijkheid)Enum.Parse(typeof(Toegankelijkheid), r["toegankelijkheid"].ToString()),
                                (bool)r["bezienswaardigheid"], (bool)r["info"], (bool)r["archtitectueleWaarde"]);
                            ProjectCombinatie stad = new ProjectCombinatie(projectResult);
                            resultaat.Add(stad);
                        }
                        if (r["oppervlakte"] != DBNull.Value)
                        {
                            GroeneRuimte projectResult = new GroeneRuimte(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (double)r["oppervlakte"], (double)r["biodiversiteit"], (int)r["aantalWandelpaden"], null, (bool)r["inToeristischeWandelroute"], (double)r["beoordeling"]);
                            ProjectCombinatie groen = new ProjectCombinatie(projectResult);
                            resultaat.Add(groen);
                        }
                        if (r["aantalWooneenheden"] != DBNull.Value)
                        {
                            InnovatieWonen projectResult = new InnovatieWonen(r["titel"].ToString(), (DateTime)r["startDatum"], r["beschrijving"].ToString(), status, loc, (int)r["aantalWooneenheden"], (bool)r["rondleiding"], (bool)r["showwoning"], (double)r["architectuurInnovatieScore"], (bool)r["samenwerkingErfgoedOfToerisme"]);
                            ProjectCombinatie innovatie = new ProjectCombinatie(projectResult);
                            resultaat.Add(innovatie);
                        }
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





