using ProjectbeheerBL.Interfaces;
using ProjectbeheerDL.Repository;
using ProjectbeheerBL.Domein;

string connectionstring = @"Data Source=alejandro\sqlexpress;Initial Catalog=ProjectBeheerStadGent;Integrated Security=True;Trust Server Certificate=True";

ProjectRepo repo = new ProjectRepo(connectionstring);
Locatie loc = new Locatie("Hoboken", "tripie", "Antwerpen", 2660, "25E");
InnovatieWonen innov = new InnovatieWonen("Speciaal", new DateTime(2000, 1, 1), "Testestetstt", ProjectbeheerBL.Domein.Enums.ProjectStatus.Planning, loc, 22, true, true, 10.0, true);

repo.VoegProjectToe(innov);