
using Newtonsoft.Json;
using System.Diagnostics;



var builder = WebApplication.CreateBuilder(args);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyMethod();
                          policy.AllowAnyHeader();
                          policy.AllowAnyOrigin();
                      });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/gardes", () =>
{
    Gardes Garde = new Gardes();
    using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + "/db.json"))
    {
        string json;
        try { json = r.ReadToEnd(); } catch (Exception e) { throw new Exception("Can't read json file"); }

        try { Garde = JsonConvert.DeserializeObject<Gardes>(json); } catch (Exception e) { throw new Exception($"Could Not parse Json file, service-config.json format is wrong : {e}"); }
        if (Garde.Gardesdata.Count == 0)
            Console.WriteLine("config file is empty");
        return Garde;
    }
    return Garde;
});

app.MapPost("/gardes", (Grade grade) =>
{
    Gardes Gardes = new Gardes();
    string json;
    using (StreamReader r = new StreamReader(Directory.GetCurrentDirectory() + "/db.json"))
    {
 
        try { json = r.ReadToEnd(); } catch (Exception e) { throw new Exception("Can't read json file"); }

        try { Gardes = JsonConvert.DeserializeObject<Gardes>(json); } catch (Exception e) { throw new Exception($"Could Not parse Json file, service-config.json format is wrong : {e}"); }

        Gardes.Gardesdata.Add(grade);
       

    }
     json = JsonConvert.SerializeObject(Gardes, Formatting.Indented);
    File.WriteAllText(Directory.GetCurrentDirectory() + "/db.json", json);

});
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
app.UseAuthorization();

app.MapControllers();

app.Run();


public class Grade
{
    public DateTime DateDebut { get; set; }

    public DateTime DateFind { get; set; }
    public string Nom { get; set; }
    public string Garde { get; set; }
}

public class Gardes
{
    public List<Grade> Gardesdata { get; set; }
}

