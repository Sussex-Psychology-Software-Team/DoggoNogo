mergeInto(LibraryManager.library, {
    queryString: function (variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (decodeURIComponent(pair[0]) == variable) {
                var queryValue = decodeURIComponent(pair[1]);
                var bufferSize = lengthBytesUTF8(queryValue) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(queryValue, buffer, bufferSize);
                return buffer;
            }
        }
        // If none found
        var notFound = "NONE FOUND JS";
        var bufferSize = lengthBytesUTF8(notFound) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(notFound, buffer, bufferSize);
        return buffer;
    }
});