using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Exeptions;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProjectbeheerDL.Repository {
    public class AdminRepository {

        private readonly string _connectionString;
        private readonly IProjectRepository _projectRepository;

        public AdminRepository(string connectionString, IProjectRepository projectrepository) {
            _connectionString = connectionString;
            _projectRepository = projectrepository;
        }

        public void VoegGebruikerToe(Gebruiker g) {
            string query = "INSERT INTO Gebruiker (naam,email,isAdmin) VALUES(@naam,@email,@isAdmin)";
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = conn.CreateCommand()) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@naam", SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar));
                    cmd.Parameters.Add(new SqlParameter("@isAdmin", SqlDbType.Bit));
                    cmd.CommandText = query;
                    cmd.Parameters["@naam"].Value = g.Naam;
                    cmd.Parameters["@email"].Value = g.Email;
                    cmd.Parameters["@isAdmin"].Value = g.IsBeheerder;
                    conn.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }


        public void VerwijderProject(int projectId) { 
            // we zijn voor een volledige delete uit de database gegaan ipv de data op non-actief te zetten
            // Door CASCADE te gebruiken in de database voor de foreign keys die een verwijzing naar Project hebben, moeten er veel minder queries opgesteld worden om gerelateerde gegevens te verwwijderen
            string queryProject = "DELETE FROM Project WHERE id=@projectId";
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = con.CreateCommand()) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int));
                    cmd.CommandText = queryProject;
                    cmd.Parameters["@projectId"].Value = projectId;
                    con.Open();
                    cmd.ExecuteNonQuery();
                } catch (SqlException ex) // Specifieke SQL fouten vangen
        {
                    // HIER zie je nu de echte reden (bijv. Constraint violation)
                    throw new Exception($"Database fout bij verwijderen project {projectId}: {ex.Message}", ex);
                } catch (Exception ex) {
                    throw new Exception();
                    //throw new Exception("Fout bij database operatie: " + ex.Message);
                }
            }
        }

        public void UpdateInformatieProject(Project project, ProjectCombinatie projecten) {
            string queryProject = "UPDATE Project SET titel=@titel, startDatum=@startDatum, beschrijving=@beschrijving, status=@status FROM Project WHERE id=@id";
            string queryProjectPartner = "UPDATE ProjectPartner SET rol=@rol FROM ProjectPartner WHERE projectId=@projectId";
            string queryInnoWonen = "UPDATE InnovatieWonen SET aantalWooneenheden=@aantalWooneenheden, rondleiding=@rondleiding, showwoning=@showwoning, architectuurInnovatieScore=@architectuurInnovatieScore, samenwerkingErfgoedOfToerisme=@samenwerkingErfgoedOfToerisme FROM InnovatieWonen WHERE projectId=@projectId";
            string queryGroenRuimte = "UPDATE GroeneRuimte SET oppervlakte=@oppervlakte, biodiversiteit=@biodiversiteit, aantalWandelpaden=@aantalWandelpaden, inToeristischeWandelroute=@inToeristischeWandelroute, beoordeling=@beoordeling FROM GroeneRuimte WHERE projectId=@projectId";
            string queryStadOntw = "UPDATE StadsOntwikkeling SET verguningsStatus=@vergunningsStatus, archtitectueleWaarde=@architectuelewaarde, toegankelijkheid=@toegankelijkheid, bezienswaardigheid=@bezienswaardigheid, info=@info FROM StadsOntwikkeling WHERE projectId=@projectId";
            string queryLocatie = "UPDATE Locatie SET wijk=@wijk, straat=@straat, gemeente=@gemeente, postcode=@postcode, huisnummer=@huisnummer FROM Locatie INNER JOIN Project ON Locatie.id=Project.locatieId WHERE Project.id=@projectId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd1 = con.CreateCommand())
            using (SqlCommand cmd2 = con.CreateCommand())
            using (SqlCommand cmd3 = con.CreateCommand())
            using (SqlCommand cmd4 = con.CreateCommand())
            using (SqlCommand cmd5 = con.CreateCommand())
            using (SqlCommand cmd6 = con.CreateCommand()) {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmd1.Transaction = transaction;
                cmd2.Transaction = transaction;
                cmd3.Transaction = transaction;
                cmd4.Transaction = transaction;
                cmd5.Transaction = transaction;
                cmd6.Transaction = transaction;

                cmd1.CommandText = queryProject;
                cmd2.CommandText = queryProjectPartner;
                cmd3.CommandText = queryInnoWonen;
                cmd4.CommandText = queryGroenRuimte;
                cmd5.CommandText = queryStadOntw;
                cmd6.CommandText = queryLocatie;

                try {
                    // Update Project
                    cmd1.Parameters.AddWithValue("@titel", project.Titel);
                    cmd1.Parameters.AddWithValue("@startDatum", project.StartDatum);
                    cmd1.Parameters.AddWithValue("@beschrijving", project.Beschrijving);
                    cmd1.Parameters.AddWithValue("@status", project.Status);
                    cmd1.Parameters.AddWithValue("@id", project.Id);

                    // Update ProjectPartners
                    cmd2.Parameters.AddWithValue("@projectId", project.Id);
                    foreach (KeyValuePair<Partner, List<string>> partner in project.ProjectPartners) {
                        foreach (string rol in partner.Value) {
                            cmd2.Parameters.Clear();

                            cmd2.Parameters.AddWithValue("@rol", partner.Value);
                        }
                    }

                    
                    
                    //Update KindProjecten
                    foreach (Project kind in projecten.ProjectComboLijst) {
                        if (kind is InnovatieWonen) {
                            //Update InnoWonen
                            InnovatieWonen inno = (InnovatieWonen)kind;
                            cmd3.Parameters.AddWithValue("@aantalWooneenheden", inno.AantalWooneenheden);
                            cmd3.Parameters.AddWithValue("@rondleiding", inno.HeeftRondleiding);
                            cmd3.Parameters.AddWithValue("@showwoning", inno.HeeftShowcase);
                            cmd3.Parameters.AddWithValue("@architectuurInnovatieScore", inno.ArchitectuurScore);
                            cmd3.Parameters.AddWithValue("@samenwerkingErfgoedOfToerisme", inno.HeeftSamenwerkingErfgoedOfToerisme);
                            cmd3.Parameters.AddWithValue("projectId", project.Id);
                        } else if (kind is GroeneRuimte) {
                            // Update GroeneRuimte
                            GroeneRuimte groen = (GroeneRuimte)kind;
                            cmd4.Parameters.AddWithValue("@oppervlakte", groen.Oppervlakte);
                            cmd4.Parameters.AddWithValue("@biodiversiteit", groen.Biodiversiteit);
                            cmd4.Parameters.AddWithValue("@aantalWandelpaden", groen.AantalWandelpaden);
                            cmd4.Parameters.AddWithValue("@inToeristischeWandelroute", groen.IsInToeristWandelroute);
                            cmd4.Parameters.AddWithValue("@beoordeling", groen.Beoordeling);
                            cmd4.Parameters.AddWithValue("@projectId", project.Id);
                        } else {
                            // Update StadsOntw
                            Stadsontwikkeling stad = (Stadsontwikkeling)kind;
                            cmd5.Parameters.AddWithValue("@vergunningsStatus", stad.VergunningStatus);
                            cmd5.Parameters.AddWithValue("@architectueleWaarde", stad.HeeftArchitecturaleWaarde);
                            cmd5.Parameters.AddWithValue("@toegankelijkheid", stad.Toegankelijkheid);
                            cmd5.Parameters.AddWithValue("@bezienswaardigheid", stad.IsBezienswaardig);
                            cmd5.Parameters.AddWithValue("@info", stad.HeeftInfo);
                            cmd4.Parameters.AddWithValue("@projectId", project.Id);
                        }

                        // Update Locatie
                        cmd5.Parameters.AddWithValue("@wijk", project.Locatie.Wijk);
                        cmd5.Parameters.AddWithValue("@straat", project.Locatie.Straat);
                        cmd5.Parameters.AddWithValue("@gemeente", project.Locatie.Gemeente);
                        cmd5.Parameters.AddWithValue("@postcode", project.Locatie.Postcode);
                        cmd5.Parameters.AddWithValue("@huisnummer", project.Locatie.HuisNummer);
                        cmd5.Parameters.AddWithValue("@wijk", project.Locatie.Wijk);
                        cmd5.Parameters.AddWithValue("@projectId", project.Id);

                        transaction.Commit();
                    }
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw new Exception();
                }
            }

        }

        private void VerwijderPartnerVanProject(int projectId, int partnerId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel, niet de partner uit de Partner tabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryProjectPartner = "DELETE FROM ProjectPartner WHERE projectId=@projectId AND partnerId=@partnerId";

            using (SqlCommand cmd =  new SqlCommand(queryProjectPartner, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@partnerId", SqlDbType.Int));

                    cmd.Parameters["@projectId"].Value = projectId;
                    cmd.Parameters["@partnerId"].Value = partnerId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }

        private void verwijderFaciliteitVanProject(int groeneRuimteId, int faciliteitId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelGroenFac = "DELETE FROM GroeneRuimte_Faciliteit WHERE groeneruimteId=@groeneRuimteId AND faciliteitId=@faciliteitId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelGroenFac, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@groeneRuimteId", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@faciliteitId", SqlDbType.Int));

                    cmd.Parameters["@groeneRuimteId"].Value = groeneRuimteId;
                    cmd.Parameters["@faciliteitId"].Value = faciliteitId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }

        private void verwijderBouwfirmaVanProject(int bouwfirmaId, int stadsontwikkelingsProjectId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelBouwStad = "DELETE FROM Bouwfirma_stadsontwikkeling WHERE bouwfirmaId=@bouwfirmaId AND stadsontwikkelingsId=@stadsontwikkelingsId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelBouwStad, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@bouwfirmaId", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@stadsontwikkelingsId", SqlDbType.Int));

                    cmd.Parameters["@bouwfirmaId"].Value = bouwfirmaId;
                    cmd.Parameters["@stadsontwikkelingsId"].Value = stadsontwikkelingsProjectId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }

        private void verwijderWoonvormTypeVanProject(int woonvormTypeId, int innovatieWonenId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelInnotype = "DELETE FROM WoonvormType_InnovatieWonen WHERE woonvormTypeId=@woonvormTypeId AND innovatieWonenId=@innovatieWonenId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelInnotype, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@woonvormTypeId", SqlDbType.Int));
                    cmd.Parameters.Add(new SqlParameter("@innovatieWonenId", SqlDbType.Int));

                    cmd.Parameters["@woonvormTypeId"].Value = woonvormTypeId;
                    cmd.Parameters["@innovatieWonenId"].Value = innovatieWonenId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                }
            }
        }

        // had properder gekund zonder Partner, Locatie & rollen toe te voegen. Maar focus ligt op werkende code voor nu
        public void gebruikerVoegtProjectToe(List<Project> projecten, Gebruiker g, Partner partner, Locatie? locPartner, List<string> rollen) {
            using (IDbConnection conn = new SqlConnection(_connectionString)) {
                conn.Open();
                using (IDbTransaction trans = conn.BeginTransaction()) {
                    try {

                        string queryGebruiker = "INSERT INTO Gebruiker (naam, email, isAdmin) VALUES (@naam, @email, @isAdmin); SELECT SCOPE_IDENTITY();";
                        int gebruikerId;

                        int projectId;

                        string queryProjectCombinatie = "INSERT INTO Project_Gebruiker (projectId, gebruikerId) VALUES (@projectId, @gebruikerId)";

                        using (SqlCommand cmd1 = new SqlCommand(queryGebruiker, (SqlConnection)conn, (SqlTransaction)trans))
                        using (SqlCommand cmd2 = new SqlCommand(queryProjectCombinatie, (SqlConnection)conn, (SqlTransaction)trans)) {
                            cmd1.Parameters.AddWithValue("@naam", g.Naam);
                            cmd1.Parameters.AddWithValue("@email", g.Email);
                            cmd1.Parameters.AddWithValue("@isAdmin", g.IsBeheerder);
                            gebruikerId = Convert.ToInt32(cmd1.ExecuteScalar());

                            foreach (Project p in projecten) {
                                projectId = _projectRepository.VoegProjectToe(p, partner, locPartner, rollen, conn, trans);
                                cmd2.Parameters.Clear();
                                cmd2.Parameters.AddWithValue("@projectId", projectId);
                                cmd2.Parameters.AddWithValue("@gebruikerId", gebruikerId);
                                cmd2.ExecuteNonQuery();
                            }

                        }
                        trans.Commit();
                    } catch (Exception ex) {
                        trans.Rollback();
                        throw;
                    }
                }
            }


        }
    }
}
