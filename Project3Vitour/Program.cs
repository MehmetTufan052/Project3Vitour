using Microsoft.Extensions.Options;
using Project3Vitour.Services.CatregoryService;
using Project3Vitour.Services.GalleryService;
using Project3Vitour.Services.HuggingFaceService;
using Project3Vitour.Services.ReservationService;
using Project3Vitour.Services.ReviewService;
using Project3Vitour.Services.TourPlanService;
using Project3Vitour.Services.TourServices.ITourService;
using Project3Vitour.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Optional local secrets file (kept out of git)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReservationService, ReservationService>();
builder.Services.AddScoped<IGalleryService, GalleryService>();
builder.Services.AddScoped<ITourPlanService, TourPlanService>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddHttpClient<HuggingFaceService>();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettingsKey"));
builder.Services.Configure<ReviewSettings>(builder.Configuration.GetSection("ReviewSettings"));

builder.Services.AddScoped<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value; 
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

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


