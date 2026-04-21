using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;
using Microsoft.Data.SqlClient;

namespace ProjectbeheerDL.Repository {
    public class GroeneRuimteRepository : ProjectRepository {
        public GroeneRuimteRepository(string connectionstring) : base(connectionstring) {}

        public void VoegProjectToe(Project project) 
        {
            // Specifieke code voor groene ruimte projecten
            base.VoegProjectToe(project);
            GroeneRuimte g = (GroeneRuimte)project;


            using(sqlConnection conn = new sqlConnection(connectionstring)) {
                string sql = "INSERT INTO GroeneRuimte (id, oppervlakte, biodiversiteit, aantalWandelpaden, inToeristischeWandelroute, beoordeling) " +
                              "VALUES (@id, @opp, @bio, @paden, @route, @beoordeling)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", g.Id);
                cmd.Parameters.AddWithValue("@opp", g.Oppervlakte);
                cmd.Parameters.AddWithValue("@bio", g.Biodiversiteit);
                cmd.Parameters.AddWithValue("@paden", g.AantalWandelpaden);
                cmd.Parameters.AddWithValue("@route", g.IsInToeristWandelroute);
                cmd.Parameters.AddWithValue("@beoordeling", g.Beoordeling);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<GroeneRuimte> GeefGroeneRuimtes()
        {
            List<GroeneRuimte> lijst = new List<GroeneRuimte>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string sql = @"SELECT p.*, g.* FROM GroeneRuimte g 
                       JOIN Project p ON g.Id = p.Id";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var g = new GroeneRuimte
                        {
                            Id = (int)reader["Id"],
                            Titel = reader["Titel"].ToString(),
                            Oppervlakte = (double)reader["Oppervlakte"],
                            IsInToeristWandelroute = (bool)reader["IsInToeristWandelroute"]
                        };
                        lijst.Add(g);
                    }
                }
            }
            return lijst;
        }
    }
}
