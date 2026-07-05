
using Eportal.Modules.Academic.Infrastructure;
using Eportal.Modules.Exams.Infrastructure;
using Eportal.Modules.Identity.Application;
using Eportal.Modules.Identity.Domain;
using Eportal.Modules.Identity.Infrastructure;
using Eportal.Modules.Requests.Infrastructure;
using Eportal.Shared;
using Eportal.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<IdentityDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddDbContext<AcademicDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddDbContext<ExamsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddDbContext<RequestsDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<IdentityDbContext>();

builder.Services.AddScoped<IUserLookupService, UserLookupService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/access-denied";
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await Eportal.Modules.Identity.Infrastructure.RoleSeeder.SeedAsync(scope.ServiceProvider);
    await Eportal.Modules.Identity.Infrastructure.AdminSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapPost("/logout", async (SignInManager<AppUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/");
});

app.MapGet("/requests/{id:int}/document", async (
    int id,
    Eportal.Modules.Requests.Infrastructure.RequestsDbContext requestsDb,
    Eportal.Modules.Academic.Infrastructure.AcademicDbContext academicDb,
    Eportal.Shared.IUserLookupService userLookup) =>
{
    var request = await requestsDb.StudentRequests.FindAsync(id);

    if (request is null || request.Status != Eportal.Modules.Requests.Domain.RequestStatus.Approved)
    {
        return Results.NotFound();
    }

    var student = await academicDb.Students
        .Include(s => s.StudyProgram)
        .FirstOrDefaultAsync(s => s.Id == request.StudentId);

    if (student is null)
    {
        return Results.NotFound();
    }

    var user = await userLookup.FindByUserIdAsync(student.UserId);
    var fullName = user is not null ? $"{user.FirstName} {user.LastName}" : "Nepoznat student";

    var pdfBytes = Eportal.Modules.Requests.Application.RequestDocumentGenerator.Generate(
        request, fullName, student.IndexNumber, student.StudyProgram?.Name ?? "");

    return Results.File(pdfBytes, "application/pdf", $"potvrda_{id}.pdf");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
