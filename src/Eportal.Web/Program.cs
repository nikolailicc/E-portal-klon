
using Eportal.Modules.Academic.Infrastructure;
using Eportal.Modules.Exams.Infrastructure;
using Eportal.Modules.Identity.Application;
using Eportal.Modules.Identity.Domain;
using Eportal.Modules.Identity.Infrastructure;
using Eportal.Modules.Requests.Application;
using Eportal.Modules.Requests.Infrastructure;
using Eportal.Shared;
using Eportal.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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
    Eportal.Modules.Exams.Infrastructure.ExamsDbContext examsDb,
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
    var logoPath = Path.Combine(app.Environment.WebRootPath, "images", "logo.png");

    var courses = academicDb.Courses.ToList();

    var passedExams = examsDb.ExamRegistrations
        .Include(r => r.Exam)
        .Where(r => r.StudentId == student.Id && r.Grade != null && r.Grade >= 6) // prilagodi prag prolaznosti ako je drugačiji
        .OrderBy(r => r.Exam!.ExamDate)
        .ToList();

    var examResults = passedExams
        .Where(r => r.Exam is not null)
        .Select(r =>
        {
            var course = courses.FirstOrDefault(c => c.Id == r.Exam!.CourseId);
            return new ExamResult(
                SubjectName: course?.Name ?? "Nepoznat predmet",
                Espb: course?.Espb ?? 0,
                Grade: r.Grade!.Value,
                PassedAt: r.Exam!.ExamDate
            );
        })
        .ToList();

    var pdfBytes = RequestDocumentGenerator.Generate(
        request: request,
        studentFullName: fullName,
        indexNumber: student.IndexNumber,
        studyProgram: student.StudyProgram?.Name ?? "",
        logoPath: logoPath,
        examResults: examResults
    );

    return Results.File(pdfBytes, "application/pdf", $"potvrda_{id}.pdf");
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
