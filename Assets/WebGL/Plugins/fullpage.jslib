mergeInto(LibraryManager.library, {
    fullpage: function () {
        const canvas = document.getElementById('#unity-canvas')
        //canvas.removeAttribute("style")
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }
});