namespace Domain;

public class CatalogObject
{
    public CatalogObject() 
    {
        Id = Guid.NewGuid();
    }

    public CatalogObject(
        string messierNumber,
        string newGeneralCalatog,
        string objectType,
        string constellation,
        string rightAscension,
        string declination,
        decimal magnitude,
        double distanceLightYears) : this()
    {
        MessierNumber = messierNumber;
        NewGeneralCalatog = newGeneralCalatog;
        ObjectType = objectType;
        Constellation = constellation;
        RightAscension = rightAscension;
        Declination = declination;
        Magnitude = magnitude;
        DistanceLightYears = distanceLightYears;
    }

    public Guid Id { get; set; }
    public virtual Catalog Catalog { get; set; }
    public string MessierNumber { get; set; }
    public string NewGeneralCalatog { get; set; }
    public string ObjectType { get; set; }
    public string Constellation { get; set; }
    public string RightAscension { get; set; }
    public string Declination { get; set; }
    public decimal Magnitude { get; set; }
    public double DistanceLightYears { get; set; }
}
