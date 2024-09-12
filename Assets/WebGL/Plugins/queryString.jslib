mergeInto(LibraryManager.library, {
    queryString: function (variable) {
        const query = window.location.search.substring(1);
        const vars = query.split("&");
        for(let i=0; i<vars.length; i++) {
                let pair = vars[i].split("=");
                if(pair[0] == variable){ 
                    const queryValue = pair[1];
                    const bufferSize = lengthBytesUTF8(queryValue) + 1;
                    const buffer = _malloc(bufferSize);
                    stringToUTF8(queryValue, buffer, bufferSize);
                    return buffer;
                }
        }
        return('');
    }
});
