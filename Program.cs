using DataCloudDemo.Common;
using DataCloudDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddDbContext<DCContext>(options =>
options.UseNpgsql(Utilities.GetPGConnectionString(Environment.GetEnvironmentVariable("DATABASE_URL"))));
//options.UseNpgsql(builder.Configuration.GetConnectionString("DCContext")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1800);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddCors(policyBuilder =>
    policyBuilder.AddDefaultPolicy(policy =>
        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
);

var app = builder.Build();

//Revert to the legacy timestamp behavior
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next(context);
    });

    var culture = CultureInfo.CreateSpecificCulture("en-US");
    var dateformat = new DateTimeFormatInfo
    {
        ShortDatePattern = "dd-MM-yyyy",
        LongDatePattern = "MM-dd-yyyy hh:mm:ss tt"
    };
    culture.DateTimeFormat = dateformat;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
    CultureInfo.DefaultThreadCurrentCulture = culture;

    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.SetIsOriginAllowed((host) => true)
.AllowCredentials());
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
                name: "defaultApi",
                pattern: "{controller=webhook}/{action=index}/{id?}");
});
app.MapRazorPages();

app.Run();
