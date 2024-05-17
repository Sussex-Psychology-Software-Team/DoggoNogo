mergeInto(LibraryManager.library, {
    dataPipe: function (data,id) {
        fetch("https://pipe.jspsych.org/api/data/", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "*/*",
            },
            body: JSON.stringify({
                experimentID: "VSyXogVR8oTS",
                filename: id + ".csv", // Construct using participant ID here
                data: data, // Add JSON object here
            }),
        });
    }
});
