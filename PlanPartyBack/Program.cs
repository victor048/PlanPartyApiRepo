using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PlanPartyBack.Models;
using PlanPartyBack.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuração do MongoDB
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    Console.WriteLine($"MongoDB Connection String: {settings.ConnectionString}");
    return client;
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// Registro dos serviços
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<MongoDbService>();

// Adicionar o serviço de hashing de senha
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Adicionar o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Ativar o Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlanPartyBack API V1");
    c.RoutePrefix = "swagger";
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
