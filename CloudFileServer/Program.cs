using CloudFileServer.Applibs;
using CloudFileServer.Persistent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddMemoryCache();
builder.Services.AddPersistentServices(ConfigHelper.ConnectionStrings.MSSQL);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
