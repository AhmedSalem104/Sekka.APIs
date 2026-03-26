namespace Sekka.Core.DTOs.Customer;

public class CreateRatingDto
{
    public Guid? OrderId { get; set; }
    public int RatingValue { get; set; }
    public bool QuickResponse { get; set; }
    public bool ClearAddress { get; set; }
    public bool RespectfulBehavior { get; set; }
    public bool EasyPayment { get; set; }
    public bool WrongAddress { get; set; }
    public bool NoAnswer { get; set; }
    public bool DelayedPickup { get; set; }
    public bool PaymentIssue { get; set; }
    public string? FeedbackText { get; set; }
}

public class RatingDto
{
    public Guid Id { get; set; }
    public int RatingValue { get; set; }
    public List<string> PositiveTags { get; set; } = new();
    public List<string> NegativeTags { get; set; } = new();
    public string? FeedbackText { get; set; }
    public DateTime CreatedAt { get; set; }
}
