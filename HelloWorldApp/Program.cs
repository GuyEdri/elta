var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Make the app listen on 0.0.0.0:3000 inside the container
app.Urls.Add("http://0.0.0.0:3000");

app.MapGet("/", () => "Hello World!");

app.Run();
 
