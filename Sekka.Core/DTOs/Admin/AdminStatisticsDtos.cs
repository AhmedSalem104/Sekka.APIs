using Sekka.Core.Enums;

namespace Sekka.Core.DTOs.Admin;

public class AdminDashboardDto
{
    public int TotalDrivers { get; set; }
    public int OnlineDrivers { get; set; }
    public int TotalOrdersToday { get; set; }
    public decimal TotalRevenueToday { get; set; }
    public int PendingOrders { get; set; }
    public int ActiveDeliveries { get; set; }
}

public class QuickStatsDto
{
    public int OnlineDrivers { get; set; }
    public int ActiveOrders { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TodayOrders { get; set; }
}

public class KPIDashboardDto
{
    public decimal SuccessRate { get; set; }
    public double AvgDeliveryTime { get; set; }
    public decimal CustomerSatisfaction { get; set; }
    public decimal DriverUtilization { get; set; }
    public decimal RevenuePerOrder { get; set; }
    public decimal CostPerDelivery { get; set; }
}

public class PeriodComparisonDto
{
    public string Period1Label { get; set; } = null!;
    public string Period2Label { get; set; } = null!;
    public int Period1Orders { get; set; }
    public int Period2Orders { get; set; }
    public decimal OrdersGrowth { get; set; }
    public decimal Period1Revenue { get; set; }
    public decimal Period2Revenue { get; set; }
    public decimal RevenueGrowth { get; set; }
}

public class RevenueReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetProfit { get; set; }
    public List<RevenueSourceDto> Sources { get; set; } = new();
    public List<DriverRevenueEntryDto> TopDrivers { get; set; } = new();
}

public class RevenueSourceDto
{
    public string SourceName { get; set; } = null!;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
}

public class DriverRevenueEntryDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public decimal Revenue { get; set; }
    public int OrderCount { get; set; }
    public decimal Percentage { get; set; }
}

public class DriversPerformanceReportDto
{
    public int TotalDrivers { get; set; }
    public int ActiveDrivers { get; set; }
    public List<DriverPerformanceEntryDto> TopPerformers { get; set; } = new();
    public List<DriverPerformanceEntryDto> LowPerformers { get; set; } = new();
    public List<VehicleTypePerformanceDto> VehicleTypeBreakdown { get; set; } = new();
}

public class DriverPerformanceEntryDto
{
    public Guid DriverId { get; set; }
    public string DriverName { get; set; } = null!;
    public int TotalOrders { get; set; }
    public decimal SuccessRate { get; set; }
    public double AvgDeliveryTime { get; set; }
    public decimal Revenue { get; set; }
    public decimal Rating { get; set; }
}

public class VehicleTypePerformanceDto
{
    public VehicleType VehicleType { get; set; }
    public int DriverCount { get; set; }
    public int OrderCount { get; set; }
    public decimal AvgDeliveryTime { get; set; }
    public decimal SuccessRate { get; set; }
}

public class RegionsReportDto
{
    public List<RegionReportEntryDto> Regions { get; set; } = new();
    public string BestRegion { get; set; } = null!;
    public string WorstRegion { get; set; } = null!;
}

public class RegionReportEntryDto
{
    public string RegionName { get; set; } = null!;
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal SuccessRate { get; set; }
    public double AvgDeliveryTime { get; set; }
    public int ActiveDrivers { get; set; }
}

public class RealtimeDashboardDto
{
    public int OnlineDrivers { get; set; }
    public int ActiveOrders { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TodayOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ActiveDeliveries { get; set; }
    public int DelayedOrders { get; set; }
    public decimal AvgDeliveryTimeToday { get; set; }
}
