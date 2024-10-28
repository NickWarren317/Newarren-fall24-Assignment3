using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newarren_fall24_Assignment3.Data;
using Newarren_fall24_Assignment3.Services;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
/*
 Server=tcp:fall24newarrenassignment3.database.windows.net,1433;Initial Catalog=Newarren-HW3-f24;Persist Security Info=False;User ID=nwarren;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
 
 */
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("AzureConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


//get openai secret
string keyVaultUrl = "https://newarren-assignment3.vault.azure.net/";
string secretName = "openai-key";

// Create a secret client
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

// Retrieve the secret
KeyVaultSecret key = client.GetSecret(secretName);
string openAIKey = key.Value;

// Define your OpenAI endpoint
string openAIEndpoint = "https://openaiconn.openai.azure.com/";

// Create the OpenAIService instance with the key and endpoint


builder.Services.AddScoped<OpenAIService>(provider => new OpenAIService(openAIKey, openAIEndpoint));

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
