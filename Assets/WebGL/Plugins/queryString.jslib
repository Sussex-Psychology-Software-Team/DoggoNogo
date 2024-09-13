mergeInto(LibraryManager.library, {
    queryString: function (variable) {
        const urlParams = new URLSearchParams(window.location.search);
        const queryValue = urlParams.get(variable);
        if(queryValue===null) console.log('no expID found');
        const bufferSize = lengthBytesUTF8(queryValue) + 1;
        const buffer = _malloc(bufferSize);
        stringToUTF8(queryValue, buffer, bufferSize);
        return buffer;
    }
});