mergeInto(LibraryManager.library, {
  fullpage: function () {
    function resizeCanvas(){
        //This container is only featured if the build settings are 'default' and not 'minimal'
        //const container = document.getElementById('#unity-container')
        //container.style.width = window.innerWidth+'px' //or 100%
        //container.style.height = window.innerHeight+'px' //or 100%
        const canvas = document.getElementById('#unity-canvas')
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }
    window.onresize = resizeCanvas;
    resizeCanvas()
  },
  
});