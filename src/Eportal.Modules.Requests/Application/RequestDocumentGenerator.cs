using Eportal.Modules.Requests.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;

namespace Eportal.Modules.Requests.Application;

public static class RequestDocumentGenerator
{
    public static byte[] Generate(
    StudentRequest request,
    string studentFullName,
    string indexNumber,
    string studyProgram)
    {
        var title = GetTitle(request.Type);

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("UNIVERZITET")
                    .FontSize(18).Bold().AlignCenter();

                page.Content()
                    .PaddingVertical(30)
                    .Column(col =>
                    {
                        col.Item().Text(title).FontSize(16).Bold().AlignCenter();
                        col.Item().PaddingTop(30);

                        switch (request.Type)
                        {
                            case RequestType.PotvrdaOStudiranju:
                            case RequestType.PotvrdaZaStipendiju:
                            case RequestType.Uverenje:
                                col.Item().Text($"Ovim se potvrđuje da je student {studentFullName}, broj indeksa {indexNumber}, upisan na studijski program {studyProgram}.");
                                break;

                            case RequestType.Molba:
                                col.Item().Text($"Student {studentFullName}, broj indeksa {indexNumber}, studijski program {studyProgram}, podneo/la je sledeću molbu:");
                                col.Item().PaddingTop(15).Text(string.IsNullOrWhiteSpace(request.Note) ? "(bez dodatnog teksta)" : request.Note);
                                col.Item().PaddingTop(20).Text("Studentska služba je molbu odobrila.");
                                break;

                            case RequestType.PromenaLicnihPodataka:
                                col.Item().Text($"Student {studentFullName}, broj indeksa {indexNumber}, zatražio/la je promenu ličnih podataka:");
                                col.Item().PaddingTop(15).Text(string.IsNullOrWhiteSpace(request.Note) ? "(nije naveden detalj)" : request.Note);
                                col.Item().PaddingTop(20).Text("Zahtev je odobren i podaci će biti ažurirani.");
                                break;
                        }

                        col.Item().PaddingTop(40);
                        col.Item().Text($"Datum izdavanja: {request.ResolvedAt:dd.MM.yyyy.}");
                    });

                page.Footer()
                    .AlignRight()
                    .Text("Eportal — automatski generisan dokument");
            });
        });

        return document.GeneratePdf();
    }

    private static string GetTitle(RequestType type) => type switch
    {
        RequestType.PotvrdaOStudiranju => "Potvrda o studiranju",
        RequestType.PotvrdaZaStipendiju => "Potvrda za stipendiju",
        RequestType.Uverenje => "Uverenje",
        RequestType.Molba => "Molba",
        RequestType.PromenaLicnihPodataka => "Potvrda o promeni ličnih podataka",
        _ => "Potvrda"
    };
}