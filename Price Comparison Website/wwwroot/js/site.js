// Notification handling---------------------------------------------------------------
document.addEventListener('DOMContentLoaded', function () {
    const notificationDropdown = document.getElementById('notificationDropdown');
    if (notificationDropdown) {
        // Load notifications immediately
        fetchNotifications();

        // Initialize bootstrap dropdown
        const dropdown = new bootstrap.Dropdown(notificationDropdown, {
            autoClose: false
        });
        
        notificationDropdown.addEventListener('click', async function (e) {
            e.preventDefault();
            dropdown.toggle(); // Explicitly toggle the dropdown
            try {
                await markNotificationsAsRead();
            } catch (error) {
                console.error('Error handling notifications:', error);
            }
        });

        // Close dropdown when clicking outside
        document.addEventListener('click', function (e) {
            if (!notificationDropdown.contains(e.target)) {
                dropdown.hide();
            }
        });

        // Refresh notifications every minute
        setInterval(fetchNotifications, 60000);
    }
});

async function fetchNotifications() {
    try {
        const response = await fetch('/User/GetNotifications', {
            method: 'GET',
            headers: {
                'Accept': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        updateNotificationUI(data);
    } catch (error) {
        console.error('Error fetching notifications:', error);
    }
}

function updateNotificationUI(data) {
    const notificationList = document.getElementById('notificationList');
    const notificationCount = document.getElementById('notificationCount');
    
    if (!notificationList || !notificationCount) return;

    notificationList.innerHTML = '';
    
    if (data && data.notifications && data.notifications.length > 0) {
        // Update unread count
        const unreadCount = data.notifications.filter(n => !n.isRead).length;
        notificationCount.textContent = unreadCount || '0';

        // Add notifications header
        const headerItem = document.createElement('li');
        headerItem.innerHTML = `<h6 class="dropdown-header">Notifications</h6>`;
        notificationList.appendChild(headerItem);

        data.notifications.forEach(notification => {
            const listItem = document.createElement('li');
            const itemClass = notification.isRead ? '' : 'notification-unread';
            const textClass = notification.isRead ? 'text-muted' : '';
            listItem.innerHTML = `
                <div class="notification-item ${itemClass}">
                    <div class="notification-text ${textClass}">
                        ${notification.message}
                        <small class="notification-timestamp">${formatTimestamp(notification.timestamp)}</small>
                    </div>
                </div>`;
            notificationList.appendChild(listItem);
        });
    } else {
        notificationCount.textContent = '0';
        const listItem = document.createElement('li');
        listItem.innerHTML = '<span class="dropdown-item-text">No notifications</span>';
        notificationList.appendChild(listItem);
    }
}

function formatTimestamp(timestamp) {
    const date = new Date(timestamp);
    const now = new Date();
    const diffInMinutes = Math.floor((now - date) / (1000 * 60));

    if (diffInMinutes < 1) return 'Just now';
    if (diffInMinutes < 60) return `${diffInMinutes}m ago`;
    if (diffInMinutes < 1440) return `${Math.floor(diffInMinutes / 60)}h ago`;
    if (diffInMinutes < 10080) return `${Math.floor(diffInMinutes / 1440)}d ago`;
    
    return date.toLocaleDateString();
}

async function markNotificationsAsRead() {
    try {
        const response = await fetch('/User/MarkNotificationsAsRead', {
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
        
        // Refresh notifications but keep dropdown open
        await fetchNotifications();
    } catch (error) {
        console.error('Error marking notifications as read:', error);
    }
}

//---------------------------------------------------------------------------------------