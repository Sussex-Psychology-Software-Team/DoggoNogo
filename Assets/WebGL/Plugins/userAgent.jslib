mergeInto(LibraryManager.library, {
    userAgent: function(){
        const ua = window.navigator.userAgent;
        const bufferSize = lengthBytesUTF8(ua) + 1;
        const buffer = _malloc(bufferSize);
        stringToUTF8(ua, buffer, bufferSize);
        return buffer;
    }
});