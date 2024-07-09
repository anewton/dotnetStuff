using Domain;
using Microsoft.VisualBasic.FileIO;
using System.Reflection;

namespace Data;

public class CatalogDataImportService
{
    public static IEnumerable<CatalogObject> GetCatalogObjects(string fileName)
    {
        List<CatalogObject> catalogObjects = [];
        ArgumentNullException.ThrowIfNull(fileName);

        var assembly = Assembly.GetExecutingAssembly();
        var catalogResourceName = "Data.Resources." + fileName;

        using Stream catalogStream = assembly.GetManifestResourceStream(catalogResourceName);
        if (catalogStream != null)
        {
            using TextFieldParser csvParser = new(catalogStream);
            csvParser.SetDelimiters([","]);
            csvParser.HasFieldsEnclosedInQuotes = true;
            csvParser.ReadLine();
            while (!csvParser.EndOfData)
            {
                string[] row = csvParser.ReadFields();
                if (row != null)
                {
                    var catalogObject = new CatalogObject
                    {
                        MessierNumber = row[0].ToString(),
                        NewGeneralCalatog = row[1].ToString(),
                        ObjectType = row[2].ToString(),
                        Constellation = row[3].ToString(),
                        RightAscension = row[4].ToString(),
                        Declination = row[5].ToString(),
                        Magnitude = decimal.Parse(row[6].ToString()),
                        DistanceLightYears = double.Parse(row[7].ToString())
                    };
                    catalogObjects.Add(catalogObject);
                }
            }
        }
        return catalogObjects;
    }

}