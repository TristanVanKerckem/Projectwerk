using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;
using Microsoft.Data.SqlClient;


namespace ProjectbeheerDL.Repository {
    public class InnovatieWonenRepository : ProjectRepository
    {
        public InnovatieWonenRepository(string connectionstring) : base(connectionstring) { }

        public void VoegProjectToe(Project project)
        {
            // Specifieke code voor innovatie wonen projecten
            base.VoegProjectToe(project);
            InnovatieWonen i = (InnovatieWonen)project;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = "INSERT INTO InnovatieWonen (id, aantalWooneenheden, rondleiding, showwoning, architectuurInnovatieScore, samenwerkingErfgoed, samenwerkingToerisme) " +
                             "VALUES (@id, @eenheden, @rond, @show, @score, @erfgoed, @toerisme)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", i.Id);
                cmd.Parameters.AddWithValue("@eenheden", i.AantalWooneenheden);
                cmd.Parameters.AddWithValue("@rond", i.HeeftRondleiding);
                cmd.Parameters.AddWithValue("@show", i.HeeftShowcase);
                cmd.Parameters.AddWithValue("@score", i.ArchitectuurScore);
                cmd.Parameters.AddWithValue("@erfgoed", i.HeeftSamenwerkingErfgoedOfToerisme);
                cmd.Parameters.AddWithValue("@toerisme", i.HeeftSamenwerkingErfgoedOfToerisme);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
