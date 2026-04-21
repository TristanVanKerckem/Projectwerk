using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
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

                }
            }
        }


        public void VerwijderProject(Project project) { // we zijn voor een volledige delete uit de database gegaan ipv de data op non-actief te zetten
            // Door CASCADE te gebruiken in de database voor de foreign keys die een verwijzing naar Project hebben, moeten er veel minder queries opgesteld worden om gerelateerde gegevens te verwwijderen
            string queryProject = "DELETE FROM Project WHERE projectId=@projectId";
            using (SqlConnection con = new SqlConnection(connectionString))
            using (SqlCommand cmd = con.CreateCommand()) {
                try {
                    cmd.Parameters.Add(new SqlParameter("@projectId", SqlDbType.Int));
                    cmd.CommandText = queryProject;
                    cmd.Parameters["@projectId"].Value = project.Id;
                    con.Open();
                    cmd.ExecuteNonQuery();
                } catch (Exception ex) {

                }
            }
        }

        public void UpdateInformatieProject(Project project) {
            string queryProject = "UPDATE FROM Project SET titel=@titel, startDatum=@startDatum, beschrijving=@beschrijving, status=@status WHERE id=@id";
            string queryProjectPartner = "UPDATE FROM ProjectPartner SET rol=@rol WHERE projectId=@projectId";
            string queryInnoWonen = "UPDATE FROM InnovatieWonen SET aantalWooneenheden=@aantalWooneenheden, rondleiding=@rondleiding, showwoning=@showwoning, architectuurInnovatieScore=@architectuurInnovatieScore, samenwerkingErfgoedOfToerisme=@samenwerkingErfgoedOfToerisme WHERE projectId=@projectId";
            string queryGroenRuimte = "UPDATE FROM ";
            string queryStadOntw = "";
            string queryLocatie = "";
        }


    }
}
