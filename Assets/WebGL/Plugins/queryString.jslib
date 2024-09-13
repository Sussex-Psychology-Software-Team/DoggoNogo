mergeInto(LibraryManager.library, {
    queryString: function (variable) {
        const urlParams = new URLSearchParams(window.location.search);
        let queryValue = urlParams.get(UTF8ToString(variable));
        if(queryValue===null) queryValue = "QUERY VAR NOT FOUND";
        const bufferSize = lengthBytesUTF8(queryValue) + 1;
        const buffer = _malloc(bufferSize);
        stringToUTF8(queryValue, buffer, bufferSize);
        return buffer;
    }
});