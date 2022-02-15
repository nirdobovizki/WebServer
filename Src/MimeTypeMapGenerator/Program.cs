
var destPath = "../../../../NirDobovizki.WebServer/Generated/MimeTypes.cs";
if(!File.Exists(destPath))
{
    Console.WriteLine("MimeTypes.cs not at the expected location");
    return;
}

var http = new HttpClient();
var result = await http.GetAsync("https://svn.apache.org/repos/asf/httpd/httpd/trunk/docs/conf/mime.types");
if(!result.IsSuccessStatusCode)
{
    Console.WriteLine("can't get mime.types file from apache");
    return;
}

List<(string, string)> types = new List<(string, string)>();

var usedExt = new HashSet<string>();
using (var typesReader = new StreamReader(await result.Content.ReadAsStreamAsync()))
{
    string? tLine;
    while((tLine = typesReader.ReadLine())!=null)
    {
        tLine = tLine.Trim();
        if (tLine.Length == 0 || tLine[0] == '#') continue;
        var parts = tLine.Split(' ','\t').Where(s=>!string.IsNullOrWhiteSpace(s)).ToArray();
        foreach (var ext in parts.Skip(1))
        {
            if (usedExt.Contains(ext))
            {
                Console.WriteLine($"duplicate {ext}");
                continue;
            }
            usedExt.Add(ext);
            types.Add((parts[0], ext));
        }
    }
}


using var template = new StreamReader("MimeTypes.template");
using var dest = new StreamWriter(destPath);

string? line;
while((line=template.ReadLine())!=null)
{
    if (line.Trim() == "%CONTENT%")
    {
        int spaceCount = line.Length-line.TrimStart().Length;
        foreach (var (mtype, ext) in types)
        {
            dest.WriteLine($"{new string(' ', spaceCount)}{{\"{ext}\",\"{mtype}\"}},");
        }
    }
    else
    {
        dest.WriteLine(line);
    }
}