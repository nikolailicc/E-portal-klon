# Eportal

Digitalni portal za upravljanje fakultetom — samostalni projekat za završni rad.

Eportal digitalizuje ključne administrativne i akademske procese na fakultetu: evidenciju studenata i predmeta, prijavu i praćenje ispita, elektronsku studentsku službu i sistem obaveštenja, kroz jedinstven portal sa ulogama za studenta, profesora, studentsku službu i administratora.

## Tehnologije

- **Backend:** ASP.NET Core 10, C# (bez zasebnog Web API sloja — logika živi unutar Blazor Server aplikacije i modula)
- **Frontend:** Blazor Server
- **Baza podataka:** MySQL + Entity Framework Core 9 (Pomelo provajder)
- **Autentifikacija:** ASP.NET Core Identity (cookie-based) + role-based autorizacija

## Arhitektura

Projekat je organizovan kao **modularni monolit** — jedna aplikacija sa jasno odvojenim modulima, umesto pune mikroservisne arhitekture, radi jednostavnijeg razvoja i održavanja u okviru samostalnog rada.

```
Eportal.sln
├── src/
│   ├── Eportal.Web                    → Blazor Server frontend
│   │   └── Components/Pages/
│   │       ├── Identity/               → Login, Register, Users, Profile, AccessDenied
│   │       ├── Academic/               → AddCourse, CourseList, Enroll, StudyPrograms
│   │       └── Exams/                  → ExamPeriods, Exams, GradeExam, RegisterExam
│   ├── Eportal.Shared                 → zajednički DTO modeli i kontrakti (npr. UserSummaryDto, IUserLookupService)
│   ├── Eportal.Modules.Identity       → autentifikacija, autorizacija, korisnici i uloge
│   ├── Eportal.Modules.Academic       → studenti, predmeti, studijski programi, upisi
│   ├── Eportal.Modules.Exams          → ispitni rokovi, ispiti, prijave, ocene
│   └── Eportal.Modules.Requests       → digitalna studentska služba, dokumenta (u planu)
```

Svaki modul je organizovan po slojevima: **Domain** (entiteti), **Application** (poslovna logika, po potrebi) i **Infrastructure** (baza, eksterni servisi). Kad jednom modulu treba podatak iz drugog (npr. Academic/Exams modulu ime profesora ili studenta iz Identity modula), koristi se labava veza preko `Eportal.Shared` (zajednički DTO + interfejs), a ne direktna referenca između modula — svaki modul ostaje nezavisno razvojna celina.

## Trenutni status

- [x] Solution struktura i moduli povezani referencama
- [x] **Identity modul — gotov**: registracija (zaključana za Administratora/Studentsku službu), login/logout, 4 uloge (Student, Profesor, StudentskaSluzba, Administrator), zaštita ruta po ulogama, upravljanje korisnicima (promena uloge, aktivacija/deaktivacija/brisanje naloga), lični profil sa promenom lozinke
- [x] **Academic modul — gotov**: studijski programi (CRUD), predmeti (dodavanje/izmena/brisanje, vezani za više studijskih programa), upis studenata na predmete, lista predmeta filtrirana po ulozi
- [x] **Exams modul — gotov**: ispitni rokovi (sesije sa periodom), zakazivanje konkretnih ispita po predmetu unutar roka, prijava/odjava ispita (student), unos ocena (profesor, ograničeno na sopstvene predmete), automatski prosek i ESPB na profilu studenta (računa se od položenih ispita, ocena 5 se ne računa u prosek, uzima se najbolja ocena po predmetu)
- [ ] Requests & Documents modul — digitalna studentska služba, generisanje PDF potvrda

## Pokretanje projekta lokalno

### Preduslovi

- [.NET SDK](https://dotnet.microsoft.com/download) (10.x)
- MySQL server (lokalno ili u kontejneru)

### Podešavanje

1. Kloniraj repozitorijum:

   ```
   git clone https://github.com/nikolailicc/E-portal-klon
   cd Eportal
   ```

2. Kreiraj `src/Eportal.Web/appsettings.Development.json` (nije uključen u repozitorijum) sa svojim connection stringom:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Port=PORT_MYSQL;Database=IME_BAZE;User=IME_USERA;Password=TVOJA_LOZINKA;"
     }
   }
   ```

3. Primeni EF Core migracije — projekat ima **tri odvojena `DbContext`-a** nad istom bazom (`IdentityDbContext`, `AcademicDbContext`, `ExamsDbContext`), pa je potrebno primeniti migracije za svaki, uz eksplicitno naveden `--context`:

   ```
   dotnet ef database update --context IdentityDbContext --project src/Eportal.Modules.Identity/Eportal.Modules.Identity.csproj --startup-project src/Eportal.Web/Eportal.Web.csproj

   dotnet ef database update --context AcademicDbContext --project src/Eportal.Modules.Academic/Eportal.Modules.Academic.csproj --startup-project src/Eportal.Web/Eportal.Web.csproj

   dotnet ef database update --context ExamsDbContext --project src/Eportal.Modules.Exams/Eportal.Modules.Exams.csproj --startup-project src/Eportal.Web/Eportal.Web.csproj
   ```

4. Pokreni aplikaciju:
   ```
   dotnet run --project src/Eportal.Web/Eportal.Web.csproj
   ```

Pri prvom pokretanju aplikacija automatski seed-uje 4 osnovne uloge i administratorski nalog:

- **Email:** `admin@eportal.local`
- **Lozinka:** `Admin123!`

## Struktura po ulogama

| Uloga             | Mogućnosti                                                                                                           |
| ----------------- | -------------------------------------------------------------------------------------------------------------------- |
| Student           | Lični i akademski profil (prosek, ESPB, upisani/položeni predmeti), prijava/odjava ispita                            |
| Profesor          | Pregled i izmena svojih predmeta, unos ocena za svoje predmete                                                       |
| Studentska služba | Dodavanje naloga (Student/Profesor), predmeti, upis studenata na predmete, studijski programi, ispitni rokovi/ispiti |
| Administrator     | Sve navedeno + upravljanje korisnicima (uloge, aktivacija/deaktivacija/brisanje naloga)                              |
