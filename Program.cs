using CDM.Database;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// SQLite connection (fichier app.db dans le dossier de l'app)
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=app.db"));


builder.Services.AddScoped<CDM.Service.ChargeCalculator>();
builder.Services.AddScoped<CDM.Service.AuthService>();
builder.Services.AddSession();


builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(typeof(RequireLoginAttribute));
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var auth = scope.ServiceProvider.GetRequiredService<CDM.Service.AuthService>();

    // S'il n'y a aucun admin → création automatique
    if (!db.Coproprietaires.Any(c => c.Role == "Admin"))
    {
        Console.WriteLine("⚠ Aucun admin détecté → création d'un compte admin par défaut.");

        auth.CreatePassword("Adding@2016", out var hash, out var salt);

        var admin = new CDM.Database.Models.Coproprietaire
        {
            Nom = "FERRER",
            Email = "plf74@msn.com",
            Role = "Admin",
            IsActive = true,
            PasswordHash = hash,
            PasswordSalt = salt,
        };

        db.Coproprietaires.Add(admin);
        db.SaveChanges();

        Console.WriteLine("✔ Administrateur par défaut créé !");
    }
}



app.UseSession();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // applique toutes les migrations existantes
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.Run();
