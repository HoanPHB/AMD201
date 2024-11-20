document.addEventListener("DOMContentLoaded", function () {
    // Event listener for shortening URLs
    document.getElementById("shortenBtn").addEventListener("click", function () {
        const urlInput = document.getElementById("urlInput").value.trim();
        const shortenedLinkField = document.getElementById("shortenedLink");
        const copyBtn = document.getElementById("copyBtn");

        if (urlInput) {
            shortenedLinkField.value = "Shortening... Please wait.";

            // Make API request to shorten URL
            fetch("http://localhost:5003/url/shorten", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ originalUrl: urlInput }),
            })
                .then((response) => response.json())
                .then((data) => {
                    if (data.shortUrl) {
                        shortenedLinkField.value = data.shortUrl;
                        copyBtn.disabled = false;
                    } else {
                        shortenedLinkField.value = "Error: " + data.message;
                        copyBtn.disabled = true;
                    }
                })
                .catch((err) => {
                    shortenedLinkField.value = "An error occurred. Please try again.";
                    console.error(err);
                });
        } else {
            alert("Please enter a valid URL!");
        }
    });

    // Event listener for COPY button in the shorten URL section
    document.getElementById("copyBtn").addEventListener("click", function () {
        const shortenedLinkField = document.getElementById("shortenedLink");
        const copyBtn = this;
        navigator.clipboard.writeText(shortenedLinkField.value).then(() => {
            const originalText = copyBtn.innerHTML;
            copyBtn.innerHTML = "<b>Copied</b>";
            setTimeout(() => {
                copyBtn.innerHTML = originalText;
            }, 1000);
        }).catch(err => {
            console.error("Failed to copy text: ", err);
        });
    });

    // Event listener for retrieving original URLs
    document.getElementById("retrieveBtn").addEventListener("click", function () {
        const retrieveInput = document.getElementById("retrieveInput").value.trim();
        const originalLinkField = document.getElementById("originalLink");
        const copyRetrieveBtn = document.getElementById("copyRetrieveBtn");

        if (retrieveInput) {
            originalLinkField.value = "Retrieving... Please wait.";

            // Make API request to retrieve original URL
            const shortCode = retrieveInput.split("/").pop(); // Extract short code from URL
            fetch(`http://localhost:5003/url/${shortCode}`)
                .then((response) => {
                    if (!response.ok) {
                        throw new Error("Shortened URL not found.");
                    }
                    return response.json();
                })
                .then((data) => {
                    if (data.originalUrl) {
                        originalLinkField.value = data.originalUrl;
                        copyRetrieveBtn.disabled = false;
                    } else {
                        originalLinkField.value = "Error: No URL found for this short code.";
                        copyRetrieveBtn.disabled = true;
                    }
                })
                .catch((err) => {
                    originalLinkField.value = "An error occurred. Please try again.";
                    console.error(err);
                });
        } else {
            alert("Please enter a valid shortened URL!");
        }
    });

    // Event listener for COPY button in the retrieve URL section
    document.getElementById("copyRetrieveBtn").addEventListener("click", function () {
        const originalLinkField = document.getElementById("originalLink");
        const copyRetrieveBtn = this;
        navigator.clipboard.writeText(originalLinkField.value).then(() => {
            const originalText = copyRetrieveBtn.innerHTML;
            copyRetrieveBtn.innerHTML = "<b>Copied</b>";
            setTimeout(() => {
                copyRetrieveBtn.innerHTML = originalText;
            }, 1000);
        }).catch(err => {
            console.error("Failed to copy text: ", err);
        });
    });
});
