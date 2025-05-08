window.downloadFileFromByteArray = (contentType, fileName, byteArray) => {
    // Create a Blob from the byte array and the content type
    var blob = new Blob([byteArray], { type: contentType });

    // Create an object URL for the Blob
    var url = URL.createObjectURL(blob);

    // Create an anchor element dynamically
    var a = document.createElement('a');
    a.href = url;
    a.download = fileName;

    // Trigger a click event to start the download
    a.click();

    // Clean up the object URL after download
    URL.revokeObjectURL(url);
};
