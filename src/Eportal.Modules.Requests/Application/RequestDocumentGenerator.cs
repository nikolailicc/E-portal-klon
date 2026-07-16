using Eportal.Modules.Requests.Domain;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Eportal.Modules.Requests.Application;

// Model za jedan položen ispit — prilagodi imenu/tipovima iz tvog domena
public sealed record ExamResult(string SubjectName, int Espb, int Grade, DateTime PassedAt);

public static class RequestDocumentGenerator
{
    public static byte[] Generate(
        StudentRequest request,
        string studentFullName,
        string indexNumber,
        string studyProgram,
        string logoPath,
        IReadOnlyList<ExamResult>? examResults = null)
    {
        var title = GetTitle(request.Type);
        var showGradesTable = request.Type is RequestType.PotvrdaZaStipendiju or RequestType.PotvrdaPolozenihIspita;

        var document = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(12).LineHeight(1.3f));

                page.Header().Row(row =>
                {
                    row.ConstantItem(70).Height(70).Image(logoPath).FitArea();
                    row.RelativeItem().AlignMiddle().Text("Prirodno-matematički fakultet")
                        .FontSize(18).Bold().AlignCenter();
                    row.ConstantItem(70); 
                });

                page.Content()
                    .PaddingVertical(30)
                    .Column(col =>
                    {
                        col.Item().Text(title).FontSize(16).Bold().AlignCenter();
                        col.Item().PaddingTop(30);

                        switch (request.Type)
                        {
                            case RequestType.PotvrdaOStudiranju:
                            case RequestType.Uverenje:
                                col.Item().Text($"Ovim se potvrđuje da je student {studentFullName}, broj indeksa {indexNumber}, upisan na studijski program {studyProgram}.");
                                break;

                            case RequestType.PotvrdaZaStipendiju:
                            case RequestType.PotvrdaPolozenihIspita:
                                col.Item().Text($"Ovim se potvrđuje da je student {studentFullName}, broj indeksa {indexNumber}, upisan na studijski program {studyProgram}, i da je položio ispite navedene u nastavku:");
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

                        if (showGradesTable && examResults is { Count: > 0 })
                        {
                            col.Item().PaddingTop(25).Text("Pregled položenih ispita:").Bold();
                            col.Item().PaddingTop(10).Element(c => BuildGradesTable(c, examResults));
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

    private static void BuildGradesTable(IContainer container, IReadOnlyList<ExamResult> exams)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);   // R. br.
                columns.RelativeColumn(3);    // Naziv predmeta
                columns.ConstantColumn(50);   // ESPB
                columns.ConstantColumn(50);   // Ocena
                columns.ConstantColumn(100);  // Datum polaganja
            });

            table.Header(header =>
            {
                header.Cell().Element(HeaderCell).AlignCenter().Text("R. br.");
                header.Cell().Element(HeaderCell).Text("Naziv predmeta");
                header.Cell().Element(HeaderCell).AlignCenter().Text("ESPB");
                header.Cell().Element(HeaderCell).AlignCenter().Text("Ocena");
                header.Cell().Element(HeaderCell).AlignCenter().Text("Datum polaganja");
            });

            for (int i = 0; i < exams.Count; i++)
            {
                var exam = exams[i];
                table.Cell().Element(BodyCell).AlignCenter().Text((i + 1).ToString());
                table.Cell().Element(BodyCell).Text(exam.SubjectName);
                table.Cell().Element(BodyCell).AlignCenter().Text(exam.Espb.ToString());
                table.Cell().Element(BodyCell).AlignCenter().Text(exam.Grade.ToString());
                table.Cell().Element(BodyCell).AlignCenter().Text(exam.PassedAt.ToString("dd.MM.yyyy."));
            }

            var totalEspb = exams.Sum(e => e.Espb);
            var averageGrade = exams.Average(e => e.Grade);

            table.Footer(footer =>
            {
                footer.Cell().ColumnSpan(2).Element(FooterCell).Text("Ukupno / Prosečna ocena:").Bold();
                footer.Cell().Element(FooterCell).AlignCenter().Text(totalEspb.ToString()).Bold();
                footer.Cell().Element(FooterCell).AlignCenter().Text(averageGrade.ToString("0.00")).Bold();
                footer.Cell().Element(FooterCell).Text("");
            });
        });
    }

    private static IContainer HeaderCell(IContainer container) =>
        container.Background(Colors.Grey.Lighten2)
                  .Border(1).BorderColor(Colors.Grey.Medium)
                  .Padding(5)
                  .DefaultTextStyle(x => x.FontSize(10).Bold());

    private static IContainer BodyCell(IContainer container) =>
        container.Border(1).BorderColor(Colors.Grey.Lighten1)
                  .Padding(5)
                  .DefaultTextStyle(x => x.FontSize(10));

    private static IContainer FooterCell(IContainer container) =>
        container.Background(Colors.Grey.Lighten3)
                  .Border(1).BorderColor(Colors.Grey.Medium)
                  .Padding(5)
                  .DefaultTextStyle(x => x.FontSize(10));

    private static string GetTitle(RequestType type) => type switch
    {
        RequestType.PotvrdaOStudiranju => "Potvrda o studiranju",
        RequestType.PotvrdaZaStipendiju => "Potvrda za stipendiju",
        RequestType.Uverenje => "Uverenje",
        RequestType.Molba => "Molba",
        RequestType.PromenaLicnihPodataka => "Potvrda o promeni ličnih podataka",
        RequestType.PotvrdaPolozenihIspita => "Potvrda položenih ispita",
        _ => "Potvrda"
    };
}