using Sekka.Persistence.Entities.Base;

namespace Sekka.Persistence.Entities;

public class Rating : BaseEntity<Guid>
{
    public Guid DriverId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? OrderId { get; set; }
    public int RatingValue { get; set; }

    // Positive tags
    public bool QuickResponse { get; set; }
    public bool ClearAddress { get; set; }
    public bool RespectfulBehavior { get; set; }
    public bool EasyPayment { get; set; }

    // Negative tags
    public bool WrongAddress { get; set; }
    public bool NoAnswer { get; set; }
    public bool DelayedPickup { get; set; }
    public bool PaymentIssue { get; set; }

    public string? FeedbackText { get; set; }

    // Navigation
    public Driver Driver { get; set; } = null!;
    public Customer Customer { get; set; } = null!;
    public Order? Order { get; set; }
}
