using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Exeptions;
using ProjectbeheerBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public void UpdateInformatieProject(Project project, Partner? partner, Locatie? partnerLocatie, List<string> rollen, List <BeschikbareFaciliteiten> facil, List<Bouwfirma> bouwfirmas, List<WoonvormType> woontypes, ProjectCombinatie projecten) {
            string queryProject = "UPDATE Project SET titel=@titel, startDatum=@startDatum, beschrijving=@beschrijving, status=@status WHERE id=@id";
            string queryProjectPartner = "INSERT INTO ProjectPartner (rol, projectId, partnerId) VALUES (@rol, @projectId, @partnerId)";
            string queryInnoWonen = "UPDATE InnovatieWonen SET aantalWooneenheden=@aantalWooneenheden, rondleiding=@rondleiding, showwoning=@showwoning, architectuurInnovatieScore=@architectuurInnovatieScore, samenwerkingErfgoedOfToerisme=@samenwerkingErfgoedOfToerisme WHERE projectId=@projectId";
            string queryGroenRuimte = "UPDATE GroeneRuimte SET oppervlakte=@oppervlakte, biodiversiteit=@biodiversiteit, aantalWandelpaden=@aantalWandelpaden, inToeristischeWandelroute=@inToeristischeWandelroute, beoordeling=@beoordeling WHERE projectId=@projectId;";
            string queryStadOntw = "UPDATE StadsOntwikkeling SET verguningsStatus=@vergunningsStatus, archtitectueleWaarde=@architectuelewaarde, toegankelijkheid=@toegankelijkheid, bezienswaardigheid=@bezienswaardigheid, info=@info WHERE projectId=@projectId";
            string queryLocatie = "UPDATE Locatie SET wijk=@wijk, straat=@straat, gemeente=@gemeente, postcode=@postcode, huisnummer=@huisnummer WHERE id = (SELECT locatieId FROM Project WHERE id=@projectId)";
            string queryPartner = "INSERT INTO Partner (naam, email, locatieId) VALUES (@naam, @email, @locatieId); SELECT SCOPE_IDENTITY();";
            string queryPartnerLoc = "INSERT INTO Locatie (wijk, straat, gemeente, postcode, huisnummer) VALUES (@wijk, @straat, @gemeente, @postcode, @huisnummer); SELECT SCOPE_IDENTITY();";
            string queryGroenFacil = "INSERT INTO GroeneRuimte_Faciliteit (groeneRuimteid, faciliteitId) VALUES (@groeneRuimteId, @faciliteitId)";
            string queryBouwfirma = "INSERT INTO BouwFirma (naam, email, telefoon) VALUES (@naam, @email, @telefoon); SELECT SCOPE_IDENTITY();";
            string queryBouwfirmaKoppel = "INSERT INTO Bouwfirma_Stadsontwikkeling (bouwfirmaid, stadsontwikkelingsid) VALUES (@bouwfirmaId, @stadsontwikkelingsId)";
            string queryGroeneRuimteKoppel = "INSERT INTO GroeneRuimte_Faciliteit (groeneRuimteid, faciliteitId) VALUES (@groeneRuimteId, @faciliteitId)";
            string queryInnovatieKoppel = "INSERT INTO WoonvormType_InnovatieWonen (woonvormTypeId, InnovatieWonenId) VALUES (@woonvormTypeId, @innovatieWonenId)";

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd1 = con.CreateCommand())
            using (SqlCommand cmd2 = con.CreateCommand())
            using (SqlCommand cmd3 = con.CreateCommand())
            using (SqlCommand cmd4 = con.CreateCommand())
            using (SqlCommand cmd5 = con.CreateCommand())
            using (SqlCommand cmd6 = con.CreateCommand())
            using (SqlCommand cmd7 = con.CreateCommand())
            using (SqlCommand cmd8 = con.CreateCommand())
            using (SqlCommand cmd9 = con.CreateCommand())
            using (SqlCommand cmd10 = con.CreateCommand())
            using (SqlCommand cmd11 = con.CreateCommand())
            using (SqlCommand cmd12 = con.CreateCommand())
            using (SqlCommand cmd13 = con.CreateCommand()) {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmd1.Transaction = transaction;
                cmd2.Transaction = transaction;
                cmd3.Transaction = transaction;
                cmd4.Transaction = transaction;
                cmd5.Transaction = transaction;
                cmd6.Transaction = transaction;
                cmd7.Transaction = transaction;
                cmd8.Transaction = transaction;
                cmd9.Transaction = transaction;
                cmd10.Transaction = transaction;
                cmd11.Transaction = transaction;
                cmd12.Transaction = transaction;
                cmd13.Transaction = transaction;



                cmd1.CommandText = queryProject;
                cmd2.CommandText = queryProjectPartner;
                cmd3.CommandText = queryInnoWonen;
                cmd4.CommandText = queryGroenRuimte;
                cmd5.CommandText = queryStadOntw;
                cmd6.CommandText = queryLocatie;
                cmd7.CommandText = queryPartner;
                cmd8.CommandText = queryPartnerLoc;
                cmd9.CommandText = queryGroenFacil;
                cmd10.CommandText = queryBouwfirma;
                cmd11.CommandText = queryBouwfirmaKoppel;
                cmd12.CommandText = queryGroeneRuimteKoppel;
                cmd13.CommandText = queryInnovatieKoppel;



                try {
                    // Update Project
                    cmd1.Parameters.Clear();
                    cmd1.Parameters.AddWithValue("@titel", project.Titel);
                    cmd1.Parameters.AddWithValue("@startDatum", project.StartDatum);
                    cmd1.Parameters.AddWithValue("@beschrijving", project.Beschrijving);
                    cmd1.Parameters.AddWithValue("@status", project.Status);
                    cmd1.Parameters.AddWithValue("@id", project.Id);
                    cmd1.ExecuteNonQuery();

                    if (partner != null && partnerLocatie != null) {
                        int vestigingPartnerId;
                        // Update Vestiging partner --> Wordt enkel toegevoegd, oude data blijft bestaan
                        cmd8.Parameters.Clear();
                        cmd8.Parameters.AddWithValue("@wijk", partnerLocatie.Wijk);
                        cmd8.Parameters.AddWithValue("@straat", partnerLocatie.Straat);
                        cmd8.Parameters.AddWithValue("@gemeente", partnerLocatie.Gemeente);
                        cmd8.Parameters.AddWithValue("@postcode", partnerLocatie.Postcode);
                        cmd8.Parameters.AddWithValue("@huisnummer", partnerLocatie.HuisNummer);
                        vestigingPartnerId = Convert.ToInt32(cmd8.ExecuteScalar());

                        int partnerId;
                        // Update Partner --> Oude data blijft bestaan
                        cmd7.Parameters.Clear();
                        cmd7.Parameters.AddWithValue("@naam", partner.Naam);
                        cmd7.Parameters.AddWithValue("@email", partner.Email);
                        cmd7.Parameters.AddWithValue("@locatieId", vestigingPartnerId);
                        partnerId = Convert.ToInt32(cmd7.ExecuteScalar());

                        // Update ProjectPartners
                        VerwijderPartnersVanProject(project.Id, con, transaction);
                        foreach (string rol in rollen) {
                                cmd2.Parameters.Clear();
                                cmd2.Parameters.AddWithValue("@partnerId", partnerId);
                                cmd2.Parameters.AddWithValue("@projectId", project.Id);
                                cmd2.Parameters.AddWithValue("@rol", rol);
                                cmd2.ExecuteNonQuery();
                        }
                    }

                    // Update Locatie Project
                    cmd6.Parameters.Clear();
                    cmd6.Parameters.AddWithValue("@wijk", project.Locatie.Wijk);
                    cmd6.Parameters.AddWithValue("@straat", project.Locatie.Straat);
                    cmd6.Parameters.AddWithValue("@gemeente", project.Locatie.Gemeente);
                    cmd6.Parameters.AddWithValue("@postcode", project.Locatie.Postcode);
                    cmd6.Parameters.AddWithValue("@huisnummer", project.Locatie.HuisNummer);
                    cmd6.Parameters.AddWithValue("@projectId", project.Id);
                    cmd6.ExecuteNonQuery();

                    //Update KindProjecten
                    foreach (Project kind in projecten.ProjectComboLijst) {
                        if (kind is InnovatieWonen) {
                            //Update InnoWonen
                            InnovatieWonen inno = (InnovatieWonen)kind;
                            cmd3.Parameters.Clear();
                            cmd3.Parameters.AddWithValue("@aantalWooneenheden", inno.AantalWooneenheden);
                            cmd3.Parameters.AddWithValue("@rondleiding", inno.HeeftRondleiding);
                            cmd3.Parameters.AddWithValue("@showwoning", inno.HeeftShowcase);
                            cmd3.Parameters.AddWithValue("@architectuurInnovatieScore", inno.ArchitectuurScore);
                            cmd3.Parameters.AddWithValue("@samenwerkingErfgoedOfToerisme", inno.HeeftSamenwerkingErfgoedOfToerisme);
                            cmd3.Parameters.AddWithValue("@projectId", project.Id);
                            cmd3.ExecuteNonQuery();

                            // Update WoonvormTypeKoppeltabel
                            verwijderWoonvormTypesVanProject(inno.Id, con, transaction);
                            foreach (WoonvormType type in woontypes) {
                                cmd13.Parameters.Clear();
                                cmd13.Parameters.AddWithValue("@woonvormTypeId", type.Id);
                                cmd13.Parameters.AddWithValue("@innovatieWonenId", inno.Id);
                                cmd13.ExecuteNonQuery();
                            }

                        } else if (kind is GroeneRuimte) {
                            // Update GroeneRuimte
                            GroeneRuimte groen = (GroeneRuimte)kind;
                            cmd4.Parameters.Clear();
                            cmd4.Parameters.AddWithValue("@oppervlakte", groen.Oppervlakte);
                            cmd4.Parameters.AddWithValue("@biodiversiteit", groen.Biodiversiteit);
                            cmd4.Parameters.AddWithValue("@aantalWandelpaden", groen.AantalWandelpaden);
                            cmd4.Parameters.AddWithValue("@inToeristischeWandelroute", groen.IsInToeristWandelroute);
                            cmd4.Parameters.AddWithValue("@beoordeling", groen.Beoordeling);
                            cmd4.Parameters.AddWithValue("@projectId", project.Id);
                            cmd4.ExecuteNonQuery();

                            // Update Koppeltabel GroeneRuimte_faciliteiten
                            verwijderFaciliteitenVanProject(groen.Id, con, transaction);

                            foreach (BeschikbareFaciliteiten faciliteit in facil) {
                                cmd12.Parameters.Clear();
                                cmd12.Parameters.AddWithValue("@groeneRuimteId", groen.Id);
                                cmd12.Parameters.AddWithValue("@faciliteitId", faciliteit.Id);
                                cmd12.ExecuteNonQuery();
                            }
                            

                        } else {
                            // Update StadsOntw
                            Stadsontwikkeling stad = (Stadsontwikkeling)kind;
                            cmd5.Parameters.Clear();
                            cmd5.Parameters.AddWithValue("@vergunningsStatus", stad.VergunningStatus);
                            cmd5.Parameters.AddWithValue("@architectueleWaarde", stad.HeeftArchitecturaleWaarde);
                            cmd5.Parameters.AddWithValue("@toegankelijkheid", stad.Toegankelijkheid);
                            cmd5.Parameters.AddWithValue("@bezienswaardigheid", stad.IsBezienswaardig);
                            cmd5.Parameters.AddWithValue("@info", stad.HeeftInfo);
                            cmd5.Parameters.AddWithValue("@projectId", project.Id);
                            cmd5.ExecuteNonQuery();

                            // Update BouwFirmas
                            verwijderBouwfirmasVanProject(stad.Id, con, transaction);
                            int databaseBouwfirmaId;
                            foreach (Bouwfirma bouwfirma in bouwfirmas) {
                                if (!_projectRepository.BestaatBouwfirma(bouwfirma.Naam, con, transaction)) {
                                    cmd10.Parameters.Clear(); // Belangrijk voor te kunnen loopen
                                    cmd10.Parameters.AddWithValue("@naam", bouwfirma.Naam);
                                    cmd10.Parameters.AddWithValue("@email", bouwfirma.Email);
                                    cmd10.Parameters.AddWithValue("@telefoon", bouwfirma.TelefoonNummer);
                                    databaseBouwfirmaId = Convert.ToInt32(cmd10.ExecuteScalar());
                                } else {
                                    databaseBouwfirmaId = (int)_projectRepository.HaalBouwfirmaIdOp(bouwfirma.Naam, con, transaction);
                                }

                                // Koppeltabel aanvullen tussen BF & SO
                                cmd11.Parameters.Clear();
                                cmd11.Parameters.AddWithValue("@bouwfirmaId", databaseBouwfirmaId);
                                cmd11.Parameters.AddWithValue("@stadsontwikkelingsId", stad.Id);
                                cmd11.ExecuteNonQuery();
                            }
                        }

                        
                    }
                    transaction.Commit();
                } catch (Exception ex) {
                    transaction.Rollback();
                    throw;
                }
            }

        }

        

        private void VerwijderPartnersVanProject(int projectId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel, niet de partner uit de Partner tabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryProjectPartner = "DELETE FROM ProjectPartner WHERE projectId=@projectId";

            using (SqlCommand cmd =  new SqlCommand(queryProjectPartner, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int));
                    cmd.Parameters["@projectId"].Value = projectId;  
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw;
                }
            }
        }

        private void verwijderFaciliteitenVanProject(int groeneRuimteId,  IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelGroenFac = "DELETE FROM GroeneRuimte_Faciliteit WHERE groeneruimteId=@groeneRuimteId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelGroenFac, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@groeneRuimteId", SqlDbType.Int));
                    cmd.Parameters["@groeneRuimteId"].Value = groeneRuimteId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw;
                }
            }
        }

        private void verwijderBouwfirmasVanProject(int stadsontwikkelingsProjectId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelBouwStad = "DELETE FROM Bouwfirma_stadsontwikkeling WHERE stadsontwikkelingsId=@stadsontwikkelingsId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelBouwStad, conn, trans)) {
                try {                 
                    cmd.Parameters.Add(new SqlParameter("@stadsontwikkelingsId", SqlDbType.Int));
                    cmd.Parameters["@stadsontwikkelingsId"].Value = stadsontwikkelingsProjectId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw;
                }
            }
        }

        private void verwijderWoonvormTypesVanProject(int innovatieWonenId, IDbConnection interfaceConn, IDbTransaction interfaceTrans) { // verwijderd enkel de koppeltabel
            SqlConnection conn = (SqlConnection)interfaceConn;
            SqlTransaction trans = (SqlTransaction)interfaceTrans;

            string queryKoppeltabelInnotype = "DELETE FROM WoonvormType_InnovatieWonen WHERE innovatieWonenId=@innovatieWonenId";
            using (SqlCommand cmd = new SqlCommand(queryKoppeltabelInnotype, conn, trans)) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@innovatieWonenId", SqlDbType.Int));
                    cmd.Parameters["@innovatieWonenId"].Value = innovatieWonenId;
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw;
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
