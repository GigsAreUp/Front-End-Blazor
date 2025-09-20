window.qrScanner = {
    video: null,
    stream: null,
    requestId: null,

    start: async function (canvasId, dotNetRef) {
        // Always stop any previous session before starting new
        this.stop();

        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            throw new Error("Canvas element with id '" + canvasId + "' not found.");
        }
        const ctx = canvas.getContext('2d');
        this.video = document.createElement('video');

        try {
            this.stream = await navigator.mediaDevices.getUserMedia({ video: { facingMode: 'environment' } });
            this.video.srcObject = this.stream;
            await this.video.play();

            canvas.width = 300;
            canvas.height = 300;

            const self = this;
            function scan() {
                if (self.video && self.video.readyState === self.video.HAVE_ENOUGH_DATA) {
                    ctx.drawImage(self.video, 0, 0, canvas.width, canvas.height);
                    const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                    const code = jsQR(imageData.data, imageData.width, imageData.height);

                    if (code) {
                        dotNetRef.invokeMethodAsync('OnQRCodeScanned', code.data);
                        self.stop(); // stop automatically after success
                        return;
                    }
                }
                self.requestId = requestAnimationFrame(scan);
            }

            this.requestId = requestAnimationFrame(scan);
        } catch (err) {
            throw new Error('Failed to access camera: ' + err.message);
        }
    },

    stop: function () {
        if (this.requestId) {
            cancelAnimationFrame(this.requestId);
            this.requestId = null;
        }
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
            this.stream = null;
        }
        if (this.video) {
            this.video.pause();
            this.video.srcObject = null;
            this.video = null;
        }
    }
};
