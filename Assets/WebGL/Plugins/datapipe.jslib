mergeInto(LibraryManager.library, {
    dataPipe: function (data, id) {
        console.log("data js: ", data);
        fetch("https://pipe.jspsych.org/api/data/", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Accept: "*/*",
            },
            body: JSON.stringify({
                experimentID: "VSyXogVR8oTS",
                filename: id + ".json", // Construct using participant ID here
                data: JSON.parse(data), // Add JSON object here
            }),
        });
    }
});
