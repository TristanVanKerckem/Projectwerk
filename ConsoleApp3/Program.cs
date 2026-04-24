using Microsoft.Data.SqlClient;
using ProjectbeheerBL.Domein;
using ProjectbeheerDL.Repository;
public class Program
{
    private static void Main(string[] args)
    {
        string connectionString = @"Data Source=Laptop_Tristan\SQLEXPRESS;Initial Catalog=Projectbeheer;Integrated Security=True;Encrypt=True;TrustServerCertificate=True";
        using SqlConnection conn = new SqlConnection(connectionString);
        conn.Open();
        ProjectRepo repo = new ProjectRepo(connectionString);
        List<ProjectCombinatie> projectenGekregen = repo.GeefAlleProjecten();
        string sql = "SELECT COUNT(*) FROM Project";

        using SqlCommand cmd = new SqlCommand(sql, conn);
        int count = (int)cmd.ExecuteScalar();

        Console.WriteLine("Aantal projecte: " + count);
    }
}