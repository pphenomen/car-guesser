var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("PrologApi", client =>
{
    var prologApiUrl = builder.Configuration.GetValue<string>("PrologApi:BaseUrl");
    client.BaseAddress = new Uri("https://localhost:5001/api/Akinator/");
});

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();