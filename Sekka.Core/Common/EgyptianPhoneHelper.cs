using System.Text.RegularExpressions;

namespace Sekka.Core.Common;

public static class EgyptianPhoneHelper
{
    private static readonly Regex MobileRegex =
        new(@"^\+201[0125]\d{8}$", RegexOptions.Compiled);

    /// <summary>
    /// Normalizes any Egyptian phone format to the canonical +201XXXXXXXXX (13 chars).
    /// </summary>
    public static string Normalize(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return phone;

        phone = phone.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");

        if (phone.StartsWith("0020"))
            phone = "+" + phone[2..];
        else if (phone.StartsWith("0") && phone.Length >= 10)
            phone = "+20" + phone[1..];
        else if (phone.StartsWith("20") && phone.Length > 10 && !phone.StartsWith("+"))
            phone = "+" + phone;
        else if (!phone.StartsWith("+") && phone.StartsWith("1") && phone.Length == 10)
            phone = "+20" + phone;

        return phone;
    }

    /// <summary>
    /// Validates if the phone is a valid Egyptian mobile number.
    /// </summary>
    public static bool IsValid(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;
        return MobileRegex.IsMatch(Normalize(phone));
    }

    /// <summary>
    /// Checks if the phone is an Egyptian mobile (010/011/012/015).
    /// </summary>
    public static bool IsMobile(string phone) => IsValid(phone);

    /// <summary>
    /// Returns the international format without '+' — for SMS APIs.
    /// Example: +201012345678 → 201012345678
    /// </summary>
    public static string ToInternationalFormat(string phone)
    {
        var normalized = Normalize(phone);
        return normalized.StartsWith("+") ? normalized[1..] : normalized;
    }

    /// <summary>
    /// Formats for display: +20 101 234 5678
    /// </summary>
    public static string ToDisplayFormat(string phone)
    {
        var normalized = Normalize(phone);
        if (normalized.StartsWith("+20") && normalized.Length == 13)
            return $"+20 {normalized[3..6]} {normalized[6..10]} {normalized[10..]}";
        return normalized;
    }

    /// <summary>
    /// Identifies the carrier from the phone number.
    /// </summary>
    public static string GetCarrierName(string phone)
    {
        var normalized = Normalize(phone);
        if (!MobileRegex.IsMatch(normalized)) return "Unknown";
        return normalized[4] switch
        {
            '0' => "Vodafone",
            '1' => "Etisalat",
            '2' => "Orange",
            '5' => "WE",
            _ => "Unknown"
        };
    }
}
