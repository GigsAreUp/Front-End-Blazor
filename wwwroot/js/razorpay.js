let razorPayKey = '';
let razorPayInstance = null;

export function getRazorPayKey() {
    return razorPayKey;
}

export function setRazorPayKey(key) {
    razorPayKey = key;
    console.log('RazorPay key set:', key.substring(0, 10) + '...');
}

export async function loadRazorPayScript() {
    return new Promise((resolve, reject) => {
        if (typeof Razorpay !== 'undefined') {
            console.log('RazorPay script already loaded');
            resolve();
            return;
        }

        if (document.getElementById('razorpay-script')) {
            document.getElementById('razorpay-script').onload = () => resolve();
            document.getElementById('razorpay-script').onerror = () => reject(new Error('Failed to load RazorPay script'));
            return;
        }

        const script = document.createElement('script');
        script.id = 'razorpay-script';
        script.src = 'https://checkout.razorpay.com/v1/checkout.js';
        script.onload = () => {
            console.log('RazorPay script loaded successfully');
            resolve();
        };
        script.onerror = () => {
            console.error('Failed to load RazorPay script');
            reject(new Error('Failed to load RazorPay script'));
        };
        document.body.appendChild(script);
    });
}

export function openRazorPay(options) {
    console.log('Opening RazorPay with options:', {
        ...options,
        key: options.key ? options.key.substring(0, 10) + '...' : 'MISSING_KEY',
        handler: 'function' // Indicate that handler is present
    });

    if (!options.key) {
        console.error('RazorPay key is missing in options');
        throw new Error('Authentication key was missing during initialization');
    }

    if (typeof Razorpay === 'undefined') {
        console.error('RazorPay script not loaded');
        throw new Error('RazorPay script not loaded');
    }

    // Create RazorPay instance with handler
    razorPayInstance = new Razorpay({
        key: options.key,
        amount: options.amount,
        currency: options.currency,
        name: options.name,
        description: options.description,
        order_id: options.order_id,
        handler: function (response) {
            console.log('Payment success:', response);
            if (options.handler && options.handler.invokeMethodAsync) {
                options.handler.invokeMethodAsync('OnPaymentSuccess',
                    response.razorpay_payment_id,
                    response.razorpay_order_id,
                    response.razorpay_signature);
            }
        },
        prefill: options.prefill,
        theme: options.theme,
        notes: options.notes
    });

    // Also set up failed payment handler via event listener
    razorPayInstance.on('payment.failed', function (response) {
        console.error('Payment failed:', response);
        if (options.handler && options.handler.invokeMethodAsync) {
            options.handler.invokeMethodAsync('OnPaymentFailed',
                response.error.metadata.payment_id,
                response.error.description);
        }
    });

    // Open payment dialog
    razorPayInstance.open();

    // Optional: Add modal close event
    razorPayInstance.on('modal.close', function () {
        console.log('RazorPay modal closed');
    });
}