mergeInto(LibraryManager.library, {
    queryString: function (variable) {
        const urlParams = new URLSearchParams(window.location.search);
        const queryValue = urlParams.get('variable');
        if(myParam===null) console.log('no expID found');
        var bufferSize = lengthBytesUTF8(myParam) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(queryValue, buffer, bufferSize);
        return buffer;
    }
});