mergeInto(LibraryManager.library, {
    queryStringWhole: function (variable) {
        const query = window.location.search.substring(1);
        return query
    }
});
