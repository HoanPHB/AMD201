document.addEventListener("DOMContentLoaded", function () {
    // Function to adjust the timestamp to the user's local time zone
    function adjustToUserTimeZone(timestamp) {
        // Create a Date object from the ISO formatted timestamp
        const date = new Date(timestamp);

        // Check if the date is valid
        if (isNaN(date.getTime())) {
            return 'Invalid Date';
        }

        // Get the user's time zone offset in minutes
        const userTimeZoneOffset = date.getTimezoneOffset(); // in minutes

        // Adjust the date by the user's time zone offset
        date.setMinutes(date.getMinutes() - userTimeZoneOffset);

        // Convert to local time string
        return date.toLocaleString();
    }

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

            fetch(`http://localhost:5003/url/${shortCode}?returnUrl=true`) // Add returnUrl=true here
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

    // Event listener for fetching analytics
    document.getElementById("analyticsBtn").addEventListener("click", function () {
        const analyticsInput = document.getElementById("analyticsInput").value.trim();
        const analyticsContainer = document.getElementById("analyticsContainer");

        if (analyticsInput) {
            analyticsContainer.innerHTML = "Fetching analytics... Please wait.";

            const shortCode = analyticsInput.split("/").pop(); // Extract short code from URL

            fetch(`http://localhost:5003/url/analytics/${shortCode}`)
            .then((response) => {
                if (!response.ok) {
                    throw new Error("Analytics not found.");
                }
                return response.json();
            })
            .then((data) => {
                if (data) {
                    // Adjust timestamps to the user's local time
                    const lastClickedAtLocal = data.lastClickedAt
                        ? adjustToUserTimeZone(data.lastClickedAt)
                        : 'Never';

                    const createdAtLocal = adjustToUserTimeZone(data.createdAt);

                    const expiresAtLocal = data.expiresAt
                        ? adjustToUserTimeZone(data.expiresAt)
                        : 'Never';

                    analyticsContainer.innerHTML = `
                        <div class="analytics-box">
                            <p><strong>Click Count:</strong> ${data.clickCount}</p>
                        </div>
                        <div class="analytics-box">
                            <p><strong>Last Clicked At:</strong> ${lastClickedAtLocal}</p>
                        </div>
                        <div class="analytics-box">
                            <p><strong>Created At:</strong> ${createdAtLocal}</p>
                        </div>
                        <div class="analytics-box">
                            <p><strong>Expires At:</strong> ${expiresAtLocal}</p>
                        </div>
                    `;
                } else {
                    analyticsContainer.innerHTML = "Error: No analytics found for this short code.";
                }
            })
            .catch((err) => {
                analyticsContainer.innerHTML = "An error occurred. Please try again.";
                console.error(err);
            });
        } else {
            alert("Please enter a valid shortened URL!");
        }
    });
});
