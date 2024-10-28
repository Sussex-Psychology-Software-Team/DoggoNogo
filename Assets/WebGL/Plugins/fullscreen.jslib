mergeInto(LibraryManager.library, {
    GoFullscreen: function() {
        try {
            if (!document.fullscreenElement) {
                var canvas = document.getElementById("unityContainer"); // Update the ID if needed
                if (canvas.requestFullscreen) {
                    canvas.requestFullscreen();
                } else if (canvas.mozRequestFullScreen) { // Firefox
                    canvas.mozRequestFullScreen();
                } else if (canvas.webkitRequestFullscreen) { // Chrome, Safari, and Opera
                    canvas.webkitRequestFullscreen();
                } else if (canvas.msRequestFullscreen) { // IE/Edge
                    canvas.msRequestFullscreen();
                } else {
                    console.warn("Fullscreen API is not supported in this browser.");
                }
            } else {
                if (document.exitFullscreen) {
                    document.exitFullscreen();
                } else {
                    console.warn("Exiting fullscreen is not supported in this browser.");
                }
            }
        } catch (error) {
            console.error("Failed to toggle fullscreen:", error);
        }
    }
});
