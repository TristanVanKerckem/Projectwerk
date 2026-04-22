using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerBL.Exeptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProjectbeheerDL.Repository {
    public class AdminRepository {

        string connectionString;

        public AdminRepository(string connectionString) {
            this.connectionString = connectionString;
        }

        public void VoegGebruikerToe(Gebruiker g) {
            string query = "INSERT INTO Gebruiker (naam,email,isAdmin) VALUES(@naam,@email,@isAdmin)";
            using (SqlConnection conn = new SqlConnection(connectionString))
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


        public void VerwijderProject(Project project) { // we zijn voor een volledige delete uit de database gegaan ipv de data op non-actief te zetten
            // Door CASCADE te gebruiken in de database voor de foreign keys die een verwijzing naar Project hebben, moeten er veel minder queries opgesteld worden om gerelateerde gegevens te verwwijderen
            string queryProject = "DELETE Project WHERE projectId=@projectId";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = con.CreateCommand()) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int));
                    cmd.CommandText = queryProject;
                    cmd.Parameters["@projectId"].Value = project.Id;
                    con.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {
                    throw new Exception();
                    //throw new Exception("Fout bij database operatie: " + ex.Message);
                }
            }
        }

        public void UpdateInformatieProject(Project project,ProjectCombinatie projecten) {
            string queryProject = "UPDATE Project SET titel=@titel, startDatum=@startDatum, beschrijving=@beschrijving, status=@status FROM Project WHERE id=@id";
            string queryProjectPartner = "UPDATE ProjectPartner SET rol=@rol FROM ProjectPartner WHERE projectId=@projectId";
            string queryInnoWonen = "UPDATE InnovatieWonen SET aantalWooneenheden=@aantalWooneenheden, rondleiding=@rondleiding, showwoning=@showwoning, architectuurInnovatieScore=@architectuurInnovatieScore, samenwerkingErfgoedOfToerisme=@samenwerkingErfgoedOfToerisme FROM InnovatieWonen WHERE projectId=@projectId";
            string queryGroenRuimte = "UPDATE GroeneRuimte SET oppervlakte=@oppervlakte, biodiversiteit=@biodiversiteit, aantalWandelpaden=@aantalWandelpaden, inToeristischeWandelroute=@inToeristischeWandelroute, beoordeling=@beoordeling FROM GroeneRuimte WHERE projectId=@projectId";
            string queryStadOntw = "UPDATE StadsOntwikkeling SET vergunningsStatus=@vergunningsStatus, architectueleWaarde=@architectuelewaarde, toegankelijkheid=@toegankelijkheid, bezienswaardigheid=@bezienswaardigheid, info=@info FROM StadsOntwikkeling WHERE projectId=@projectId";
            string queryLocatie = "UPDATE Locatie SET wijk=@wijk, straat=@straat, gemeente=@gemeente, postcode=@postcode, huisnummer=@huisnummer FROM Locatie INNER JOIN Project ON Locatie.id=Project.locatieId WHERE Project.id=@projectId";

            using(SqlConnection con = new SqlConnection(connectionString))
            using(SqlCommand cmd1 = con.CreateCommand())
            using(SqlCommand cmd2 = con.CreateCommand())
            using(SqlCommand cmd3 = con.CreateCommand())
            using(SqlCommand cmd4 = con.CreateCommand())
            using(SqlCommand cmd5 = con.CreateCommand())
            using(SqlCommand cmd6 = con.CreateCommand()) {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();
                cmd1.Transaction = transaction;
                cmd2.Transaction = transaction;
                cmd3.Transaction = transaction;
                cmd4.Transaction = transaction;
                cmd5.Transaction = transaction;
                cmd6.Transaction = transaction;

                
                cmd2.CommandText = queryProjectPartner;
                cmd3.CommandText = queryInnoWonen;
                cmd4.CommandText = queryGroenRuimte;
                cmd5.CommandText = queryStadOntw;
                cmd6.CommandText = queryLocatie;

                try {
                    // Update Project
                    cmd1.CommandText = queryProject;
                    cmd1.Parameters.AddWithValue("@titel", project.Titel);
                    cmd1.Parameters.AddWithValue("@startDatum", project.StartDatum);
                    cmd1.Parameters.AddWithValue("@beschrijving", project.Beschrijving);
                    cmd1.Parameters.AddWithValue("@status", project.Status);
                    cmd1.Parameters.AddWithValue("@id", project.Id);

                    //Update ProjectPartners

                   //cmd2.Parameters.AddWithValue("@rol", pp.Rollen);
                   // cmd2.Parameters.AddWithValue("@projectId", project.Id);
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


    }
}
