using System;
using System.Collections.Generic;
using System.Text;
using ProjectbeheerBL.Domein;
using Microsoft.Data.SqlClient;

namespace ProjectbeheerDL.Repository {
    public class StadsontwikkelingRepository : ProjectRepository {
        public StadsontwikkelingRepository(string connectionstring) : base(connectionstring) {}

        public void VoegProjectToe(Project project) {
            // Specifieke code voor stadsontwikkeling projecten
            base.VoegProjectToe(project);
            StadOntwikkeling s= (StadOntwikkeling)project;


            using(sqlConnection conn = new sqlConnection(connectionstring)) {
                string sql = "INSERT INTO StadsOntwikkeling (id, vergunningsStatus, architectueeleWaarde, toegankelijkheid, bezienswaardigheid, uitlegbord, infowandeling) " +
                              "VALUES (@id, @verg, @arch, @toeg, @beziens, @bord, @info)";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", s.Id);
                cmd.Parameters.AddWithValue("@verg", (int)s.VergunningStatus);
                [cite_start]// Feedback Tommy: HeeftArchitecturaleWaarde [cite: 4]
                cmd.Parameters.AddWithValue("@arch", s.HeeftArchitecturaleWaarde);
                cmd.Parameters.AddWithValue("@toeg", (int)s.Toegankelijkheid);
                cmd.Parameters.AddWithValue("@beziens", s.IsBezienswaardig);
                cmd.Parameters.AddWithValue("@bord", s.HeeftInfo);
                cmd.Parameters.AddWithValue("@info", s.HeeftArchitecturaleWaarde);

                conn.Open();
                cmd.ExecuteNonQuery();

                [cite_start]// Koppeling met Bouwfirmas (Veel-op-veel tabel) 
                foreach (var firma in s.Bouwfirmas)
                {
                    string sqlLink = "INSERT INTO Bouwfirma_Stadsontwikkeling (bouwfirmaId, stadsontwikkelingid) VALUES (@fId, @sId)";
                    SqlCommand cmdLink = new SqlCommand(sqlLink, conn);
                    cmdLink.Parameters.AddWithValue("@fId", firma.Id);
                    cmdLink.Parameters.AddWithValue("@sId", s.Id);
                    cmdLink.ExecuteNonQuery();
                }
            }
        }
    }
}
