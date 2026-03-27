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

    // ── Orders ──
    public static string OrderCreated => "تم إنشاء الطلب بنجاح";
    public static string OrderUpdated => "تم تحديث الطلب بنجاح";
    public static string OrderDeleted => "تم حذف الطلب بنجاح";
    public static string OrderStatusUpdated => "تم تحديث حالة الطلب بنجاح";
    public static string OrderDelivered => "تم تسليم الطلب بنجاح";
    public static string OrderFailed => "تم تسجيل محاولة التسليم الفاشلة";
    public static string OrderCancelled => "تم إلغاء الطلب بنجاح";
    public static string OrderTransferred => "تم تحويل الطلب بنجاح";
    public static string OrderPartialDelivery => "تم تسجيل التسليم الجزئي بنجاح";
    public static string BulkImportCompleted => "تم استيراد الطلبات بنجاح";
    public static string PhotoUploaded => "تم رفع صورة التوثيق بنجاح";
    public static string AddressSwapped => "تم تغيير العنوان بنجاح";
    public static string WaitingTimerStarted => "تم بدء مؤقت الانتظار";
    public static string WaitingTimerStopped => "تم إيقاف مؤقت الانتظار";
    public static string RouteOptimized => "تم تحسين المسار بنجاح";
    public static string RouteReordered => "تم إعادة ترتيب المسار بنجاح";
    public static string OrderAssigned => "تم تعيين الطلب للسائق بنجاح";
    public static string TimeSlotBooked => "تم حجز الفترة الزمنية بنجاح";
    public static string TimeSlotCreated => "تم إنشاء الفترة الزمنية بنجاح";
    public static string TimeSlotUpdated => "تم تحديث الفترة الزمنية بنجاح";
    public static string TimeSlotDeleted => "تم حذف الفترة الزمنية بنجاح";
    public static string SyncCompleted => "تم المزامنة بنجاح";

    // ── Customers & Partners ──
    public static string CustomerUpdated => "تم تحديث بيانات العميل بنجاح";
    public static string CustomerRated => "تم تقييم العميل بنجاح";
    public static string CustomerBlocked => "تم حظر العميل بنجاح";
    public static string CustomerUnblocked => "تم إلغاء حظر العميل بنجاح";
    public static string CallerIdSaved => "تم حفظ هوية المتصل بنجاح";
    public static string CallerIdUpdated => "تم تحديث هوية المتصل بنجاح";
    public static string CallerIdDeleted => "تم حذف هوية المتصل بنجاح";
    public static string AddressSaved => "تم حفظ العنوان بنجاح";
    public static string AddressUpdated => "تم تحديث العنوان بنجاح";
    public static string AddressDeleted => "تم حذف العنوان بنجاح";
    public static string PartnerCreated => "تم إضافة الشريك بنجاح";
    public static string PartnerUpdated => "تم تحديث الشريك بنجاح";
    public static string PartnerDeleted => "تم حذف الشريك بنجاح";
    public static string PartnerVerificationSubmitted => "تم رفع مستند التحقق بنجاح";
    public static string PartnerVerified => "تم التحقق من الشريك بنجاح";
    public static string PickupPointCreated => "تم إضافة نقطة الاستلام بنجاح";
    public static string PickupPointUpdated => "تم تحديث نقطة الاستلام بنجاح";
    public static string PickupPointDeleted => "تم حذف نقطة الاستلام بنجاح";
    public static string PickupPointRated => "تم تقييم نقطة الاستلام بنجاح";
    public static string BlacklistVerified => "تم تأكيد البلاغ بنجاح";
    public static string BlacklistRemoved => "تم إزالة الرقم من القائمة السوداء بنجاح";

    // ── Financial ──
    public static string SettlementCreated => "تم إنشاء التسوية بنجاح";
    public static string SettlementApproved => "تم الموافقة على التسوية بنجاح";
    public static string SettlementRejected => "تم رفض التسوية";
    public static string ReceiptUploaded => "تم رفع الإيصال بنجاح";
    public static string PaymentRequestCreated => "تم إنشاء طلب الدفع بنجاح";
    public static string PaymentRequestApproved => "تم الموافقة على طلب الدفع بنجاح";
    public static string PaymentRequestRejected => "تم رفض طلب الدفع";
    public static string PaymentRequestCancelled => "تم إلغاء طلب الدفع بنجاح";
    public static string ProofUploaded => "تم رفع إثبات التحويل بنجاح";
    public static string ExpenseCreated => "تم تسجيل المصروف بنجاح";
    public static string DisputeCreated => "تم فتح النزاع بنجاح";
    public static string DisputeResolved => "تم حل النزاع بنجاح";
    public static string DisputeRejected => "تم رفض النزاع";
    public static string DisputeEscalated => "تم تصعيد النزاع";
    public static string InvoiceGenerated => "تم إنشاء الفاتورة بنجاح";
    public static string InvoiceStatusUpdated => "تم تحديث حالة الفاتورة بنجاح";
    public static string RefundRequested => "تم تقديم طلب الاسترداد بنجاح";
    public static string RefundApproved => "تم الموافقة على الاسترداد بنجاح";
    public static string RefundRejected => "تم رفض الاسترداد";
    public static string WalletAdjusted => "تم تعديل الرصيد بنجاح";
    public static string WalletFrozenSuccess => "تم تجميد المحفظة بنجاح";
    public static string WalletUnfrozen => "تم إلغاء تجميد المحفظة بنجاح";
    public static string BulkAdjustmentCompleted => "تم التعديل الجماعي بنجاح";

    // ── Communication ──
    public static string NotificationRead => "تم تعليم الإشعار كمقروء";
    public static string AllNotificationsRead => "تم تعليم كل الإشعارات كمقروءة";
    public static string SOSActivated => "تم تفعيل حالة الطوارئ";
    public static string SOSDismissed => "تم إلغاء حالة الطوارئ";
    public static string SOSResolved => "تم حل حالة الطوارئ";
    public static string SOSAcknowledged => "تم تأكيد استلام البلاغ";
    public static string SOSEscalated => "تم تصعيد البلاغ";
    public static string TemplateCreated => "تم إنشاء القالب بنجاح";
    public static string TemplateUpdated => "تم تحديث القالب بنجاح";
    public static string TemplateDeleted => "تم حذف القالب بنجاح";
    public static string ConversationCreated => "تم إنشاء المحادثة بنجاح";
    public static string ConversationClosedSuccess => "تم إغلاق المحادثة بنجاح";
    public static string MessageSent => "تم إرسال الرسالة بنجاح";
    public static string MessageReadSuccess => "تم تعليم الرسالة كمقروءة";
    public static string BroadcastSent => "تم إرسال الإشعار الجماعي بنجاح";

    // ── Intelligence ──
    public static string SegmentCreated => "تم إنشاء الشريحة بنجاح";
    public static string SegmentUpdated => "تم تحديث الشريحة بنجاح";
    public static string SegmentDeleted => "تم حذف الشريحة بنجاح";
    public static string SegmentRefreshed => "تم تحديث أعضاء الشريحة بنجاح";
    public static string CampaignCreated => "تم إنشاء الحملة بنجاح";
    public static string CampaignUpdated => "تم تحديث الحملة بنجاح";
    public static string CampaignDeleted => "تم حذف الحملة بنجاح";
    public static string CampaignLaunched => "تم إطلاق الحملة بنجاح";
    public static string CampaignPaused => "تم إيقاف الحملة مؤقتاً";
    public static string CampaignResumed => "تم استئناف الحملة بنجاح";
    public static string RecommendationRead => "تم تعليم التوصية كمقروءة";
    public static string RecommendationDismissed => "تم تجاهل التوصية";
    public static string RecommendationActedUpon => "تم تنفيذ التوصية";

    // ── Location & Vehicles ──
    public static string VehicleCreated => "تم إضافة المركبة بنجاح";
    public static string VehicleUpdated => "تم تحديث المركبة بنجاح";
    public static string VehicleDeleted => "تم حذف المركبة بنجاح";
    public static string VehicleApproved => "تم الموافقة على المركبة";
    public static string VehicleRejected => "تم رفض المركبة";
    public static string MaintenanceRecorded => "تم تسجيل الصيانة بنجاح";
    public static string ParkingSpotSaved => "تم حفظ مكان الركنة بنجاح";
    public static string ParkingSpotDeleted => "تم حذف مكان الركنة";
    public static string BreakStarted => "تم بدء الاستراحة";
    public static string BreakEnded => "تم إنهاء الاستراحة";
}
