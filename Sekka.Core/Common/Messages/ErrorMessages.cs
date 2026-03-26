namespace Sekka.Core.Common.Messages;

public static class ErrorMessages
{
    // ── Auth ──
    public static string InvalidPhoneNumber => "رقم موبايل مصري غير صالح";
    public static string PhoneAlreadyRegistered => "رقم الموبايل مسجل بالفعل";
    public static string PasswordMismatch => "كلمة المرور غير متطابقة";
    public static string AccountCreationFailed(string details) => $"فشل إنشاء الحساب: {details}";
    public static string InvalidCredentials => "رقم الموبايل أو كلمة المرور غير صحيحة";
    public static string AccountSuspended => "الحساب موقوف";
    public static string AccountNotFound => "الحساب غير موجود";
    public static string InvalidToken => "توكن غير صالح";
    public static string SetPasswordFailed(string details) => $"فشل تعيين كلمة المرور: {details}";
    public static string ChangePasswordFailed(string details) => $"فشل تغيير كلمة المرور: {details}";

    // ── OTP / SMS ──
    public static string OtpResendLimitReached => "تم تجاوز الحد الأقصى لإعادة الإرسال. حاول بعد ساعة";
    public static string OtpAttemptsExceeded => "تم تجاوز عدد المحاولات. حاول بعد ساعة";
    public static string OtpExpiredOrNotFound => "كود التحقق منتهي الصلاحية أو غير موجود";
    public static string OtpInvalid => "كود التحقق غير صحيح";
    public static string SmsSendFailed => "فشل إرسال الرسالة. حاول مرة أخرى";

    // ── Driver / Profile ──
    public static string DriverNotFound => "السائق غير موجود";
    public static string ItemNotFound => "العنصر غير موجود";

    // ── Settings ──
    public static string SettingsNotFound => "الإعدادات غير موجودة";
    public static string ChannelSettingsNotFound => "إعداد القناة غير موجود";

    // ── Account Management ──
    public static string NoPendingDeletionRequest => "لا يوجد طلب حذف معلّق";
    public static string InvalidConfirmationCode => "كود التأكيد غير صحيح";
    public static string SessionNotFound => "الجلسة غير موجودة";

    // ── Privacy ──
    public static string NoDeletionRequest => "لا يوجد طلب حذف بيانات";

    // ── Subscriptions ──
    public static string SubscriptionNotFound => "الاشتراك غير موجود";
    public static string NoCurrentSubscription => "لا يوجد اشتراك حالي";
    public static string PlanNotFound => "الباقة غير موجودة";

    // ── Admin ──
    public static string RoleAlreadyExists => "الدور موجود بالفعل";
    public static string RoleNotFound => "الدور غير موجود";
    public static string UserNotFound => "المستخدم غير موجود";

    // ── Features ──
    public static string FeatureUnderDevelopment(string feature) => $"ميزة {feature} قيد التطوير";
    public static string SubscriptionsUnderDev => "ميزة الاشتراكات قيد التطوير";
    public static string PlansUnderDev => "ميزة الباقات قيد التطوير";
    public static string DemoConvertUnderDev => "ميزة التحويل من الديمو قيد التطوير";

    // ── Validation ──
    public static string Required(string field) => $"{field} مطلوب";
    public static string MaxLength(string field, int max) => $"{field} يجب ألا يتجاوز {max} حرف";
    public static string MinLength(string field, int min) => $"{field} يجب أن تكون {min} أحرف على الأقل";
    public static string InvalidFormat(string field) => $"{field} غير صالح";
    public static string InvalidEgyptianPhone => "رقم موبايل مصري غير صالح — يجب أن يبدأ بـ 010, 011, 012, أو 015 (11 رقم)";
    public static string OtpMustBe4Digits => "كود التحقق يجب أن يكون 4 أرقام";

    // ── General ──
    public static string UnexpectedError => "حدث خطأ غير متوقع. يرجى المحاولة لاحقاً";

    // ── SMS ──
    public static string SmsOtpTemplate(string otpCode) => $"كود التحقق الخاص بك في سكّة: {otpCode}";

    // ── Profile Completion ──
    public static string DefaultDriverName => "سائق جديد";

    // ── Orders ──
    public static string OrderNotFound => "الطلب غير موجود";
    public static string InvalidOrderStatus => "حالة الطلب غير صالحة لهذه العملية";
    public static string DuplicateOrder => "طلب مكرر — يوجد طلب مشابه بالفعل";
    public static string OrderAlreadyDelivered => "الطلب تم تسليمه بالفعل";
    public static string OrderAlreadyCancelled => "الطلب ملغي بالفعل";
    public static string CannotCancelDelivered => "لا يمكن إلغاء طلب تم تسليمه";
    public static string NoActiveRoute => "لا يوجد مسار نشط";
    public static string TrackingLinkExpired => "رابط التتبع منتهي الصلاحية";
    public static string TrackingLinkNotFound => "رابط التتبع غير موجود";
    public static string TimeSlotNotFound => "الفترة الزمنية غير موجودة";
    public static string TimeSlotFull => "الفترة الزمنية ممتلئة";
    public static string WaitingTimerNotActive => "مؤقت الانتظار غير نشط";
    public static string TransferNotFound => "طلب التحويل غير موجود";
    public static string BulkImportEmpty => "لم يتم العثور على طلبات في النص";
    public static string InvalidIdempotencyKey => "مفتاح منع التكرار مستخدم بالفعل";
    public static string OcrScanFailed => "فشل مسح الفاتورة";
    public static string SyncConflict => "يوجد تعارض في المزامنة";
    public static string AmountRange => "المبلغ يجب أن يكون بين 0 و 999,999";
    public static string DeliveryAddressRequired => "عنوان التسليم مطلوب";

    // ── Customers & Partners ──
    public static string CustomerNotFound => "العميل غير موجود";
    public static string CustomerAlreadyBlocked => "العميل محظور بالفعل";
    public static string CustomerNotBlocked => "العميل غير محظور";
    public static string PartnerNotFound => "الشريك غير موجود";
    public static string PartnerAlreadyVerified => "الشريك تم التحقق منه بالفعل";
    public static string PartnerVerificationPending => "طلب التحقق قيد المراجعة";
    public static string AddressNotFound => "العنوان غير موجود";
    public static string CallerIdNotFound => "هوية المتصل غير موجودة";
    public static string PickupPointNotFound => "نقطة الاستلام غير موجودة";
    public static string BlacklistEntryNotFound => "الرقم غير موجود في القائمة السوداء";

    // ── Financial ──
    public static string InsufficientBalance => "رصيد غير كافي";
    public static string WalletFrozen => "المحفظة مجمدة";
    public static string WalletNotFrozen => "المحفظة غير مجمدة";
    public static string SettlementNotFound => "التسوية غير موجودة";
    public static string ExpenseNotFound => "المصروف غير موجود";
    public static string PaymentRequestNotFound => "طلب الدفع غير موجود";
    public static string PaymentRequestAlreadyReviewed => "طلب الدفع تمت مراجعته بالفعل";
    public static string PaymentRequestExpired => "طلب الدفع منتهي الصلاحية";
    public static string DisputeNotFound => "النزاع غير موجود";
    public static string DisputeAlreadyResolved => "النزاع تم حله بالفعل";
    public static string InvoiceNotFound => "الفاتورة غير موجودة";
    public static string RefundNotFound => "طلب الاسترداد غير موجود";
    public static string RefundAlreadyProcessed => "طلب الاسترداد تمت معالجته بالفعل";
    public static string SurgePricingRuleNotFound => "قاعدة التسعير غير موجودة";
    public static string DailyStatsNotFound => "إحصائيات اليوم غير موجودة";
}
