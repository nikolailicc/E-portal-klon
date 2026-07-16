namespace Eportal.Modules.Requests.Domain;

public class StudentRequest
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public RequestType Type { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Submitted;

    public string? Note { get; set; }
    public string? RejectionReason { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}

public enum RequestType
{
    PotvrdaOStudiranju = 0,
    PotvrdaZaStipendiju = 1,
    PromenaLicnihPodataka = 2,
    Molba = 3,
    Uverenje = 4,
    PotvrdaPolozenihIspita = 5
}

public enum RequestStatus
{
    Submitted = 0,
    InReview = 1,
    Approved = 2,
    Rejected = 3
}