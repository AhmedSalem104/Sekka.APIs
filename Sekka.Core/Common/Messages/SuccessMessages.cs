namespace Sekka.Core.Common.Messages;

public static class SuccessMessages
{
    // ── Auth ──
    public static string OtpSent => "تم إرسال كود التحقق بنجاح";
    public static string Registered => "تم إنشاء الحساب بنجاح";
    public static string LoggedIn => "تم تسجيل الدخول بنجاح";
    public static string LoggedOut => "تم تسجيل الخروج بنجاح";
    public static string PasswordChanged => "تم تغيير كلمة المرور بنجاح";
    public static string PasswordReset => "تم إعادة تعيين كلمة المرور بنجاح";
    public static string TokenRefreshed => "تم تجديد التوكن بنجاح";
    public static string DeviceRegistered => "تم تسجيل الجهاز بنجاح";

    // ── Profile ──
    public static string ProfileUpdated => "تم تحديث الملف الشخصي بنجاح";
    public static string ImageUploaded => "تم رفع الصورة بنجاح";

    // ── Settings ──
    public static string SettingsUpdated => "تم تحديث الإعدادات بنجاح";

    // ── Account Management ──
    public static string DeletionRequested => "تم تقديم طلب حذف الحساب بنجاح";
    public static string DeletionConfirmed => "تم تأكيد حذف الحساب بنجاح";
    public static string SessionTerminated => "تم إنهاء الجلسة بنجاح";
    public static string AllSessionsTerminated => "تم تسجيل الخروج من جميع الأجهزة بنجاح";

    // ── Privacy ──
    public static string ConsentUpdated => "تم تحديث الموافقة بنجاح";
    public static string DataExportRequested => "تم تقديم طلب تصدير البيانات بنجاح";
    public static string DataDeletionRequested => "تم تقديم طلب حذف البيانات بنجاح";

    // ── Admin ──
    public static string RoleCreated => "تم إنشاء الدور بنجاح";
    public static string RoleUpdated => "تم تحديث الدور بنجاح";
    public static string RoleDeleted => "تم حذف الدور بنجاح";
    public static string RoleAssigned => "تم تعيين الدور بنجاح";
    public static string RoleRemoved => "تم إزالة الدور بنجاح";
    public static string DriverActivated => "تم تفعيل السائق بنجاح";
    public static string DriverDeactivated => "تم تعطيل السائق بنجاح";

    // ── Profile Completion Steps ──
    public static string StepName => "الاسم";
    public static string StepVehicleType => "نوع المركبة";
    public static string StepLicenseImage => "صورة الرخصة";
    public static string StepProfilePhoto => "صورة البروفايل";
    public static string StepEmail => "البريد الإلكتروني";
    public static string StepDefaultRegion => "المنطقة الافتراضية";
}
