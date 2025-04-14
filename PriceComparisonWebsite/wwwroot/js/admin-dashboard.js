function receiveScraperLogs() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/scraperHub")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceiveScraperLog", function (message) {  
        const listingLogs = document.getElementById("listingLogs");
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.textContent = message;
        listingLogs.insertBefore(li, listingLogs.firstChild);

        // Keep Last 30 logs
        while (listingLogs.children.length > 30) {
            listingLogs.removeChild(listingLogs.lastChild);
        }
    });

    connection.start() 
        .then(() => console.log("SignalR Connected"))
        .catch(err => console.error("SignalR Connection Error: " + err));
}

let formInitialized = false;

document.addEventListener('DOMContentLoaded', function() {
    // Initialize SignalR
    receiveScraperLogs();

    // Handle form submission - only attach once
    const form = document.getElementById('updateListingsForm');
    if (form && !formInitialized) {
        form.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const button = document.getElementById('updateListingsButton');
            button.disabled = true;
            button.textContent = 'Updating...';

            try {
                const response = await fetch(form.action, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });

                const result = await response.json();
                
                // Show success/error message
                const messageDiv = document.createElement('div');
                messageDiv.className = `alert alert-${result.success ? 'success' : 'danger'} alert-dismissible fade show`;
                messageDiv.innerHTML = `
                    ${result.message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                `;
                
                // Insert message at the top of the listingUpdates tab
                const tabContent = document.getElementById('listingUpdates');
                tabContent.insertBefore(messageDiv, tabContent.firstChild);

            } catch (error) {
                console.error('Error:', error);
            } finally {
                button.disabled = false;
                button.textContent = 'Update All Listings';
            }
        });
        formInitialized = true;
    }
});