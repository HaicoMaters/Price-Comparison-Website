function recieveScraperUpdateLogs()
{
    const connection = new singalR.HubConnectionBuilder()
        .withUrl("/scraperHub")
        .withAutomaticReconnect()
        .build();

    connection.on("RecieveScraperUpdateLogs", function (message) {
        const listingLogs = document.getElementById("listingLogs");
        const li = document.createElement("li");
        li.className = "list-group-item";
        li.textContet = message;
        listingLogs.insertBefore(li, listingLogs.firstChild);

        // Keep Last 30 logs
        while (listingLogs.children.length > 30){
            listingLogs.removeChild(listingLogs.lastChild);
        }});

        connection.Start()
            .then(() => console.log("SignalR Connected"))
            .catch(e => console.error(e.toString("SignalR Connection Error: " + e)))
}