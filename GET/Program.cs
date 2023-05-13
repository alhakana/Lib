using GET.Data;
using GET.Entities;
using Microsoft.EntityFrameworkCore;
using GET.Hubs;
using GET.Services;
using GET.ServicesImplementation;
using GET.Repositories;
using GET.RepositoriesImplementation;
using Microsoft.AspNetCore.Identity;
using GET;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext, Identity, and SignalR services
//builder.Services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LibraryDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSignalR();

// Register the ILibraryService and its implementation
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReservationRepository, ReservationRepository>();

var app = builder.Build();

await SeedData(app);

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

app.UseAuthentication(); // Add this line to enable authentication middleware
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<LibraryHub>("/libraryhub"); // Add the SignalR endpoint
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

async Task SeedData(WebApplication app){
    
    using (var scope = app.Services.CreateScope())
    {
        var serviceProvider = scope.ServiceProvider;

        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        
        if (!roleManager.RoleExistsAsync(Role.Librerian.ToString()).Result)
        {
            roleManager.CreateAsync(new IdentityRole(Role.Librerian.ToString())).Wait();
        }

        var librerian = new ApplicationUser()
        {
            UserName = "Alya",
            Email = "aljajana@yahoo.com",
            IsLibrarian = true
        };

        var result = await userManager.CreateAsync(librerian, "Aly@123");
        await userManager.AddToRoleAsync(librerian, "Librarian");
    }
}