using Microsoft.JSInterop;

namespace MusicHFE2.Services
{
    public interface IRazorPayService
    {
        Task InitializeAsync(string key);
        Task CreatePaymentAsync(string orderId, decimal amount, string currency,
                              string name, string description, string prefillEmail,
                              string prefillContact, string themeColor,string razorPaykey);
        Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature);
        event EventHandler<PaymentSuccessEventArgs> OnPaymentSuccess;
        event EventHandler<PaymentFailedEventArgs> OnPaymentFailed;
    }

    public class RazorPayOptions
    {
        public string Key { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "INR";
        public string Name { get; set; } = "LocalGigster";
        public string Description { get; set; } = "Music Services";
        public string OrderId { get; set; } = string.Empty;
        public string PrefillEmail { get; set; } = string.Empty;
        public string PrefillContact { get; set; } = string.Empty;
        public string ThemeColor { get; set; } = "#F37254";
    }
}
