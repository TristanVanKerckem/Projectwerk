using ProjectbeheerBL.Domein;

public class InnovatieWonen : Project
{
    public int AantalWooneenheden { get; set; }
    public List<WoonvormType> WoonvormTypes { get; set; } = new List<WoonvormType>();
    public bool HeeftRondleiding { get; set; }
    public bool HeeftShowcase { get; set; }
    public double ArchitectuurScore { get; set; }
    public bool HeeftSamenwerkingErfgoedOfToerisme { get; set; }

    public InnovatieWonen(string titel, DateTime startDatum, string beschrijving, ProjectFase fase, Locatie locatie, int aantalWooneenheden, bool heeftRondleiding, bool heeftShowcase, double architectuurScore, bool heeftSamenwerkingErfgoedOfToerisme)
        : base(titel, startDatum, beschrijving, fase, locatie)
    {
<<<<<<< HEAD
       
=======
        AantalWooneenheden = aantalWooneenheden;
        HeeftRondleiding = heeftRondleiding;
        HeeftShowcase = heeftShowcase;
        ArchitectuurScore = architectuurScore;
        HeeftSamenwerkingErfgoedOfToerisme = heeftSamenwerkingErfgoedOfToerisme;
>>>>>>> 8065b7090dd2c901a78e0df9569cb95bcd6d4475
    }

    public InnovatieWonen() { }
}
