using ProjectbeheerBL.Interfaces;
using ProjectbeheerDL.Repository;
using ProjectbeheerBL.Domein;


string connectionstring = @"Data Source=alejandro\sqlexpress;Initial Catalog=ProjectBeheerStadGent;Integrated Security=True;Trust Server Certificate=True";

ProjectRepo repo = new ProjectRepo(connectionstring);
Locatie loc1 = new Locatie(1, "Hoboken", "Meetjeslandstraat", "Antwerpen", 2660, "25E");
Locatie loc2 = new Locatie(2, "station", "Eedverbondkaai", "Gent", 9000, "7");
Locatie loc3 = new Locatie(3, "rabot", "Prinses Clementinalaan", "Gent", 9000, "277");

InnovatieWonen innov = new InnovatieWonen("Stramien", new DateTime(2010, 10, 11), "Hoe goed werkt het", ProjectbeheerBL.Domein.Enums.ProjectStatus.Uitvoering, loc1, 200, true, false, 8.88, true);

List<Bouwfirma> firmas = new List<Bouwfirma>();
Bouwfirma firma = new Bouwfirma("bouwen","ttt@hotmail.com","047498745102");
firmas.Add(firma);
Stadsontwikkeling So = new Stadsontwikkeling("Stram", new DateTime(2010, 10, 11), "Hoe goed werkt het", ProjectbeheerBL.Domein.Enums.ProjectStatus.Uitvoering, loc2, firmas,ProjectbeheerBL.Domein.Enums.VergunningStatus.Goedgekeurd, ProjectbeheerBL.Domein.Enums.Toegankelijkheid.VolledigOpenbaar,true,true,true);

List<BeschikbareFaciliteiten> faciliteiten = new List<BeschikbareFaciliteiten>();
BeschikbareFaciliteiten BF = new BeschikbareFaciliteiten("Speeltuin", true);
faciliteiten.Add(BF);    
GroeneRuimte GR = new GroeneRuimte("Stramf", new DateTime(2010, 10, 11), "Hoe goed werkt het", ProjectbeheerBL.Domein.Enums.ProjectStatus.Uitvoering, loc3,10.0,8.5,30,faciliteiten,true,7.5);

repo.VoegProjectToe(innov);
repo.VoegProjectToe(So);
repo.VoegProjectToe(GR);