// Dropdown menu for notifications
document.addEventListener('DOMContentLoaded', function () { 
    const notificationDropdown = document.getElementById('notificationDropdown');
    if (notificationDropdown) {
        fetchNotifications(); // Fetch notifications on page load
        const dropdown = new bootstrap.Dropdown(notificationDropdown, {
            autoClose: false
        });
        
        notificationDropdown.addEventListener('click', async function (e) { // Toggle dropdown and mark notifications as read
            e.preventDefault();
            dropdown.toggle();
            try {
                await markNotificationsAsRead();
            } catch (error) {
                console.error('Error handling notifications:', error);
            }
        });

        document.addEventListener('click', function (e) { // Close dropdown when clicking outside
            if (!notificationDropdown.contains(e.target)) {
                dropdown.hide();
            }
        });

        setInterval(fetchNotifications, 60000); // Fetch notifications every minute
    }
});

// Fetches notifications from the server using a GET request to user controller
async function fetchNotifications() {
    try {
        const response = await fetch('/User/GetNotifications', { // Fetch notifications
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json(); // Parse JSON response
        updateNotificationUI(data); // Update notification dropdown UI
    } catch (error) {
        console.error('Error fetching notifications:', error);
    }
}

// Updates the notification dropdown UI with the provided data
function updateNotificationUI(data) {
    const notificationList = document.getElementById('notificationList');
    const notificationCount = document.getElementById('notificationCount');
    
    if (!notificationList || !notificationCount) return;

    notificationList.innerHTML = '';
    
    if (data && data.notifications && data.notifications.length > 0) { // Check for notifications and display them and their count
        const unreadCount = data.notifications.filter(n => !n.isRead).length;
        notificationCount.textContent = unreadCount || '0'; // Display unread count

        const headerItem = document.createElement('li'); 
        headerItem.innerHTML = `<h6 class="dropdown-header">Notifications</h6>`; 
        notificationList.appendChild(headerItem);

        const wrapper = document.createElement('div');
        wrapper.className = 'notification-wrapper';
        
        data.notifications.forEach(notification => { // Display notifications
            const itemClass = notification.isRead ? '' : 'notification-unread';
            const textClass = notification.isRead ? 'text-muted' : '';
            
            const notificationDiv = document.createElement('div');
            notificationDiv.className = `notification-item ${itemClass}`;
            notificationDiv.innerHTML = `
                <div class="d-flex align-items-center justify-content-between">
                    <div class="notification-text ${textClass}">
                        ${notification.message}
                        <small class="notification-timestamp">${formatTimestamp(notification.timestamp)}</small>
                    </div>
                    <button class="btn btn-link text-danger dismiss-notification" 
                            data-notification-id="${notification.id}"
                            data-bs-toggle="tooltip"
                            data-bs-placement="left"
                            title="Dismiss notification">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>`;
            wrapper.appendChild(notificationDiv);

            // Initialize Bootstrap tooltip for dismiss button
            const tooltip = notificationDiv.querySelector('[data-bs-toggle="tooltip"]');
            const bsTooltip = new bootstrap.Tooltip(tooltip);
            tooltip.addEventListener('mouseleave', () => {
                bsTooltip.hide();
            });
            
            setupDismissButton(notificationDiv.querySelector('.dismiss-notification')); // Setup dismiss button
        });

        const wrapperItem = document.createElement('li');
        wrapperItem.appendChild(wrapper);
        notificationList.appendChild(wrapperItem);
    } 
    else { // Display no notifications
        notificationCount.textContent = '0';
        const listItem = document.createElement('li');
        listItem.innerHTML = '<span class="dropdown-item-text">No notifications</span>';
        notificationList.appendChild(listItem);
    }
}

// Formats a timestamp string into a human-readable format
function formatTimestamp(timestamp) {
    const [datePart, timePart] = timestamp.split(' '); 
    const [day, month, year] = datePart.split('/'); // Parse date parts from timestamp
    const date = new Date(`${year}-${month}-${day} ${timePart}`);
    
    if (isNaN(date.getTime())) {
        console.error('Invalid timestamp:', timestamp);
        return 'Invalid date';
    }

    const now = new Date();
    const diffInMinutes = Math.floor((now - date) / (1000 * 60));

    // Return formatted timestamp based on time difference
    if (diffInMinutes < 1) return 'Just now';
    if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
    if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`;
    if (diffInMinutes < 10080) return `${Math.floor(diffInMinutes / 1440)}d ago`;
    
    return date.toLocaleDateString('en-GB', { // Return formatted date and time string for older notifications
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    });
}

// Marks all notifications as read by sending a POST request to user controller
async function markNotificationsAsRead() {
    try {
        const response = await fetch('/User/MarkNotificationsAsRead', { // Mark notifications as read
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            }
        });

        if (!response.ok) {
            throw new Error('Error marking notifications as read');
        }
        
        await fetchNotifications(); // Fetch notifications after marking them as read
    } catch (error) {
        console.error('Error marking notifications as read:', error);
    }
}
 // Dismisses a notification by sending a POST request to user controller
function setupDismissButton(button) {
    button.addEventListener('click', async (e) => {
        e.preventDefault();
        e.stopPropagation();
        const notificationId = e.currentTarget.dataset.notificationId;
        
        try { // Dismiss notification
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const response = await fetch(`/User/DismissNotification/${notificationId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                }
            });

            if (response.ok) { // Fetch notifications after dismissing
                const result = await response.json();
                if (result.success) {
                    await fetchNotifications();
                }
            }
        } catch (error) {
            console.error('Error dismissing notification:', error);
        }
    });
}
