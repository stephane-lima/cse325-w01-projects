using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal:C}{Environment.NewLine}");

File.WriteAllText(Path.Combine(salesTotalDir, "salesReport.txt"), GenerateSalesReport(salesFiles));

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {      
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

string GenerateSalesReport(IEnumerable<string> salesFiles)
{
    double salesTotal = CalculateSalesTotal(salesFiles);

    string salesReport = "Sales Summary\n";
    salesReport += "-------------------------------------------------\n";
    salesReport += $"Total Sales: {salesTotal:C}\n\n";
    salesReport += "Details:";

    var findFiles = FindFiles("stores");

    foreach (var file in findFiles)
    {
        if (file.EndsWith("sales.json"))
        {
            string salesJson = File.ReadAllText(file);
            SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

            salesReport += $"\n{file}: {data?.Total:C}";
        }
    }

    return salesReport;
}

record SalesData (double Total);
//record SalesReport (double Total);
