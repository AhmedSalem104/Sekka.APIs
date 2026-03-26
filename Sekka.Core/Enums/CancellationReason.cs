namespace Sekka.Core.Enums;

public enum CancellationReason
{
    CustomerRequest = 0,
    DriverRequest = 1,
    NoResponse = 2,
    DuplicateOrder = 3,
    Other = 4
}
