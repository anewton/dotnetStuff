namespace Domain;

public class Catalog
{
    public Catalog()
    {
        Id = Guid.NewGuid();
        Objects ??= [];
    }

    public Catalog(
        string name,
        IEnumerable<CatalogObject> objects) : this()
    {
        Name = name;
        Objects = objects;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    public IEnumerable<CatalogObject> Objects { get; set; }
}