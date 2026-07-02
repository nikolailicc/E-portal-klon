# Eportal

Digitalni portal za upravljanje fakultetom — samostalni projekat za završni rad.

Eportal digitalizuje ključne administrativne i akademske procese na fakultetu: evidenciju studenata i predmeta, prijavu i praćenje ispita, elektronsku studentsku službu i sistem obaveštenja, kroz jedinstven portal sa ulogama za studenta, profesora, studentsku službu i administratora.

## Tehnologije

- **Backend:** ASP.NET Core 10, C#
- **Frontend:** Blazor Server
- **Baza podataka:** MySQL + Entity Framework Core 9
- **Autentifikacija:** ASP.NET Core Identity + role-based autorizacija

## Arhitektura

Projekat je organizovan kao **modularni monolit** — jedna aplikacija sa jasno odvojenim modulima, umesto pune mikroservisne arhitekture, radi jednostavnijeg razvoja i održavanja u okviru samostalnog rada.

```
Eportal.sln
├── src/
│   ├── Eportal.Web                    → Blazor Server frontend
│   ├── Eportal.Shared                 → zajednički DTO modeli i kontrakti
│   ├── Eportal.Modules.Identity       → autentifikacija, autorizacija, korisnici i uloge
│   ├── Eportal.Modules.Academic       → profil studenta, predmeti, upis        (u planu)
│   ├── Eportal.Modules.Exams          → ispitni rokovi, prijava, ocene         (u planu)
│   └── Eportal.Modules.Requests       → digitalna studentska služba, dokumenta (u planu)
```

Svaki modul je organizovan po slojevima: **Domain** (entiteti), **Application** (poslovna logika) i **Infrastructure** (baza, eksterni servisi).

## Trenutni status

- [x] Solution struktura i moduli povezani referencama
- [x] `AppUser` entitet (nasleđuje `IdentityUser`) + `UserRole` enum
- [x] EF Core `IdentityDbContext` + MySQL konekcija, migracije primenjene
- [x] Registracija, login i logout — end-to-end
- [x] Seed 4 osnovne uloge (Student, Profesor, StudentskaSluzba, Administrator)
- [x] Zaštita ruta po ulogama (`[Authorize(Roles = "...")]`)
- [ ] Academic modul
- [ ] Exams modul
- [ ] Requests & Documents modul

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

3. Primeni EF Core migracije:

   ```
   dotnet ef database update --project src/Eportal.Modules.Identity/Eportal.Modules.Identity.csproj --startup-project src/Eportal.Web/Eportal.Web.csproj
   ```

4. Pokreni aplikaciju:
   ```
   dotnet run --project src/Eportal.Web/Eportal.Web.csproj
   ```

Pri prvom pokretanju aplikacija automatski seed-uje osnovne uloge u bazu.

## Struktura po ulogama

| Uloga             | Mogućnosti                                                  |
| ----------------- | ----------------------------------------------------------- |
| Student           | Profil, predmeti, prijava ispita, ocene, zahtevi, dokumenti |
| Profesor          | Svoji predmeti, prijave, unos ocena                         |
| Studentska služba | Upravljanje studentima, obrada zahteva, ispitni rokovi      |
| Administrator     | Upravljanje korisnicima i ulogama                           |
