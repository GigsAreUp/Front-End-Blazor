using Microsoft.JSInterop;
using static MusicHFE2.Pages.Buyer.RazorPay;

namespace MusicHFE2.Services
{
    public class RazorPayService : IRazorPayService, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _razorPayModule;
        private bool _isInitialized = false;
        private bool _isScriptLoaded = false;

        public event EventHandler<PaymentSuccessEventArgs>? OnPaymentSuccess;
        public event EventHandler<PaymentFailedEventArgs>? OnPaymentFailed;

        public RazorPayService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task InitializeAsync(string key)
        {
            if (!_isInitialized)
            {
                try
                {
                    // Load the interop module
                    _razorPayModule = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                        "import", "./js/razorpay.js");

                    // Load the RazorPay script
                    await _razorPayModule.InvokeVoidAsync("loadRazorPayScript");
                    _isScriptLoaded = true;

                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to initialize RazorPay service", ex);
                }
            }
        }

        public async Task CreatePaymentAsync(string orderId, decimal amount, string currency,
                                           string name, string description, string prefillEmail,
                                           string prefillContact, string themeColor, string razorPayKey)
        {
            if (!_isInitialized || _razorPayModule == null)
                throw new InvalidOperationException("RazorPay service not initialized. Call InitializeAsync first.");

            if (!_isScriptLoaded)
                throw new InvalidOperationException("RazorPay script not loaded");

            if (string.IsNullOrEmpty(razorPayKey))
                throw new ArgumentException("RazorPay key is required");

            try
            {
                // Create handler instance for this specific payment
                var handler = new RazorPayHandler(this);
                var finalAmount = CalculateGrossAmount(amount, 3, 0);
                var options = new
                {
                    key = razorPayKey,
                    amount = (int)(finalAmount * 100), // Convert to paise (ensure integer)
                    currency = currency,
                    name = name,
                    description = description,
                    order_id = orderId,
                    handler = DotNetObjectReference.Create(handler),
                    prefill = new
                    {
                        email = prefillEmail,
                        contact = prefillContact
                    },
                    theme = new
                    {
                        color = themeColor
                    },
                    notes = new
                    {
                        order_id = orderId
                    }
                };

                await _razorPayModule.InvokeVoidAsync("openRazorPay", options);
            }
            catch (JSException jsEx)
            {
                throw new InvalidOperationException($"RazorPay JS error: {jsEx.Message}", jsEx);
            }
        }

        public async Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature)
        {
            // This should call your backend API to verify the signature
            // For security reasons, signature verification should be done server-side
            try
            {
                // Replace with actual API call to your backend
                // Example: return await _httpClient.PostJsonAsync<bool>("/api/verify-payment", new { orderId, paymentId, signature });
                return await Task.FromResult(true);
            }
            catch
            {
                return false;
            }
        }

        internal void TriggerPaymentSuccess(PaymentSuccessEventArgs args)
        {
            OnPaymentSuccess?.Invoke(this, args);
        }

        internal void TriggerPaymentFailed(PaymentFailedEventArgs args)
        {
            OnPaymentFailed?.Invoke(this, args);
        }

        public async ValueTask DisposeAsync()
        {
            if (_razorPayModule != null)
            {
                await _razorPayModule.DisposeAsync();
            }
        }
        public decimal CalculateGrossAmount(decimal netAmount, decimal percentFee, decimal fixedFee)
        {
            return (netAmount + fixedFee) / (1 - percentFee);
        }
    }

    // Handler for JS callbacks
    public class RazorPayHandler
    {
        private readonly RazorPayService _razorPayService;

        public RazorPayHandler(RazorPayService razorPayService)
        {
            _razorPayService = razorPayService;
        }

        [JSInvokable]
        public async Task OnPaymentSuccess(string paymentId, string orderId, string signature)
        {
            try
            {
                // Verify signature first
                var isValid = await _razorPayService.VerifyPaymentSignatureAsync(orderId, paymentId, signature);

                if (isValid)
                {
                    _razorPayService.TriggerPaymentSuccess(new PaymentSuccessEventArgs
                    {
                        PaymentId = paymentId,
                        OrderId = orderId,
                        Signature = signature
                    });
                }
                else
                {
                    _razorPayService.TriggerPaymentFailed(new PaymentFailedEventArgs
                    {
                        PaymentId = paymentId,
                        Error = "Payment signature verification failed"
                    });
                }
            }
            catch (Exception ex)
            {
                _razorPayService.TriggerPaymentFailed(new PaymentFailedEventArgs
                {
                    PaymentId = paymentId,
                    Error = $"Error processing payment: {ex.Message}"
                });
            }
        }

        [JSInvokable]
        public void OnPaymentFailed(string paymentId, string error)
        {
            _razorPayService.TriggerPaymentFailed(new PaymentFailedEventArgs
            {
                PaymentId = paymentId,
                Error = error
            });
        }
    }

    // Event args classes
    public class PaymentSuccessEventArgs : EventArgs
    {
        public string PaymentId { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string Signature { get; set; } = string.Empty;
    }

    public class PaymentFailedEventArgs : EventArgs
    {
        public string PaymentId { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
    }
}
