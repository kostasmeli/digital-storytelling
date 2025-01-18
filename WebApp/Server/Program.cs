global using BlazorApp.Shared;
global using Microsoft.EntityFrameworkCore;
global using BlazorApp.Server.Data;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MyApp",
            ValidAudience = "MyApp",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value))
        };
    });
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
});
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<DataContext>();
        await dbContext.Database.MigrateAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseResponseCompression();

var provider = new FileExtensionContentTypeProvider();
//provider.Mappings.Add(".wasm", "application/octet-stream");
provider.Mappings.Add(".unity", "application/octet-stream");
provider.Mappings.Add(".bank","text/html");
provider.Mappings.Add(".yarn", "text/plain");

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
});


app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");


app.Run();
