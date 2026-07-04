# Eportal

Digitalni portal za upravljanje fakultetom — samostalni projekat za završni rad.

Eportal digitalizuje ključne administrativne i akademske procese na fakultetu: evidenciju studenata i predmeta, prijavu i praćenje ispita, elektronsku studentsku službu i sistem obaveštenja kroz jedinstven portal sa ulogama za studenta, profesora, studentsku službu i administratora.

---

# Tehnologije

- **Backend:** ASP.NET Core 10
- **Frontend:** Blazor Server
- **Jezik:** C#
- **Baza podataka:** MySQL
- **ORM:** Entity Framework Core 9.0.11 + Pomelo
- **Autentifikacija:** ASP.NET Core Identity (cookie-based)
- **Arhitektura:** Modularni monolit

---

# Arhitektura

```
Eportal.sln
└── src/
    ├── Eportal.Web
    ├── Eportal.Shared
    ├── Eportal.Modules.Identity
    ├── Eportal.Modules.Academic
    ├── Eportal.Modules.Exams
    └── Eportal.Modules.Requests (planirano)
```

Svaki modul je organizovan po slojevima:

- Domain
- Application (po potrebi)
- Infrastructure

Moduli međusobno ne zavise direktno. Za razmenu podataka koriste se zajednički DTO modeli i interfejsi iz `Eportal.Shared`.

---

# Implementirani moduli

## Identity

- Registracija korisnika (Administrator / Studentska služba)
- Login / Logout
- Cookie autentifikacija
- Upravljanje korisnicima
- Promena uloge
- Aktivacija / deaktivacija naloga
- Brisanje naloga
- Profil korisnika
- Promena lozinke

---

## Academic

- CRUD studijskih programa
- CRUD predmeta
- Veza predmet ↔ studijski program (many-to-many)
- Dodela profesora predmetu
- Upis studenata na predmete
- Prikaz predmeta po ulozi
- Akademski podaci studenta

---

## Exams

Implementirano:

- CRUD ispitnih rokova
- Zakazivanje ispita unutar ispitnog roka
- Validacija datuma ispita
- Sprečavanje duplog zakazivanja
- Prijava ispita
- Odjava ispita
- Profesor ocenjuje samo svoje studente
- Unos ocena (5–10)

Preostalo:

- računanje proseka
- automatsko ažuriranje ESPB bodova na profilu studenta

---

## Requests & Documents

Planirano:

- elektronski zahtevi
- PDF potvrde
- digitalna studentska služba

---

# Trenutni status

- ✅ Identity modul
- ✅ Academic modul
- 🟡 Exams modul (99% završen)
- ⏳ Requests & Documents modul

---

# Pokretanje projekta

## Preduslovi

- .NET SDK 10
- MySQL

## appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=EportalDb;User=...;Password=...;"
  }
}
```

---

## Migracije

Identity

```bash
dotnet ef database update \
--context IdentityDbContext \
--project src/Eportal.Modules.Identity \
--startup-project src/Eportal.Web
```

Academic

```bash
dotnet ef database update \
--context AcademicDbContext \
--project src/Eportal.Modules.Academic \
--startup-project src/Eportal.Web
```

Exams

```bash
dotnet ef database update \
--context ExamsDbContext \
--project src/Eportal.Modules.Exams \
--startup-project src/Eportal.Web
```

---

## Pokretanje

```bash
dotnet run --project src/Eportal.Web
```

Prilikom prvog pokretanja automatski se kreiraju:

- četiri osnovne uloge
- administratorski nalog

```
Email: admin@eportal.local
Password: Admin123!
```

---

# Uloge

| Uloga | Funkcionalnosti |
|-------|-----------------|
| Student | Profil, akademski podaci, prijava i odjava ispita, pregled prijavljenih ispita |
| Profesor | Pregled svojih predmeta, unos ocena |
| Studentska služba | Upravljanje studijskim programima, predmetima, upisima, ispitnim rokovima i ispitima |
| Administrator | Sve funkcionalnosti + upravljanje korisnicima |

---

# Sledeći koraci

- Izračunavanje prosečne ocene
- Automatsko računanje ESPB bodova
- Requests & Documents modul
- PDF generisanje potvrda