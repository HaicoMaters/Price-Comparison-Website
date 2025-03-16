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

        // Create wrapper for horizontal scrolling
        const wrapper = document.createElement('div');
        wrapper.className = 'notification-wrapper';
        
        data.notifications.forEach(notification => {
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

            // Initialize tooltip
            const tooltip = notificationDiv.querySelector('[data-bs-toggle="tooltip"]');
            const bsTooltip = new bootstrap.Tooltip(tooltip);
            tooltip.addEventListener('mouseleave', () => {
                bsTooltip.hide();
            });
            
            // Add click event for dismiss button
            const dismissButton = notificationDiv.querySelector('.dismiss-notification');
            dismissButton.addEventListener('click', async (e) => {
                e.preventDefault();
                e.stopPropagation();
                const notificationId = e.currentTarget.dataset.notificationId;
                console.log('Attempting to dismiss notification:', notificationId);
                
                try {
                    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
                    const response = await fetch(`/User/DismissNotification/${notificationId}`, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': token
                        }
                    });

                    console.log('Dismiss response:', response);

                    if (response.ok) {
                        const result = await response.json();
                        if (result.success) {
                            await fetchNotifications(); // Refresh the notifications list
                        } else {
                            console.error('Failed to dismiss notification');
                        }
                    } else {
                        console.error('Response not OK:', response.status, response.statusText);
                    }
                } catch (error) {
                    console.error('Error dismissing notification:', error);
                }
            });
        });

        // Add wrapper to list
        const wrapperItem = document.createElement('li');
        wrapperItem.appendChild(wrapper);
        notificationList.appendChild(wrapperItem);
    } else {
        notificationCount.textContent = '0';
        const listItem = document.createElement('li');
        listItem.innerHTML = '<span class="dropdown-item-text">No notifications</span>';
        notificationList.appendChild(listItem);
    }
}

function formatTimestamp(timestamp) {
    const [datePart, timePart] = timestamp.split(' ');
    const [day, month, year] = datePart.split('/');
    const date = new Date(`${year}-${month}-${day} ${timePart}`);
    
    // Check if date is valid
    if (isNaN(date.getTime())) {
        console.error('Invalid timestamp:', timestamp);
        return 'Invalid date';
    }

    const now = new Date();
    const diffInMilliseconds = now - date;
    const diffInMinutes = Math.floor(diffInMilliseconds / (1000 * 60));

    // Return relative time based on how long ago it was
    if (diffInMinutes < 1) {
        return 'Just now';
    } 
    else if (diffInMinutes < 60) {
        return `${diffInMinutes}m ago`;
    } 
    else if (diffInMinutes < 1440) {
        const hours = Math.floor(diffInMinutes / 60);
        return `${hours}h ago`;
    } 
    else if (diffInMinutes < 10080) {
        const days = Math.floor(diffInMinutes / 1440);
        return `${days}d ago`;
    } 
    else {
        // For timestamps older than a week, return the formatted date
        return date.toLocaleDateString('en-GB', {
            year: 'numeric',
            month: 'short',
            day: 'numeric',
            hour: '2-digit',
            minute: '2-digit'
        });
    }
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

document.addEventListener('DOMContentLoaded', function() {
    // Format all price elements
    function formatPrices() {
        document.querySelectorAll('[data-price]').forEach(element => {
            const price = parseFloat(element.dataset.price);
            if (!isNaN(price)) {
                element.textContent = window.currencyFormatter.format(price);
            }
        });
    }

    // Initial format
    formatPrices();

    document.addEventListener('pricesUpdated', formatPrices);
});

function formatPrice(price) {
    return window.currencyFormatter.format(price);
}

// Form Validation
function validateUrl(url) {
    try {
        new URL(url);
        return true;
    } catch {
        return false;
    }
}

function validatePrice(price, discountedPrice = null) {
    const priceValue = parseFloat(price);
    
    // Validate regular price
    if (isNaN(priceValue) || priceValue <= 0) {
        throw new Error('Price must be greater than 0');
    }
    
    // Validate discounted price if provided
    if (discountedPrice !== null) {
        const discountValue = parseFloat(discountedPrice);
        if (isNaN(discountValue)) {
            throw new Error('Invalid discounted price format');
        }
        if (discountValue <= 0) {
            throw new Error('Discounted price must be greater than 0');
        }
        if (discountValue >= priceValue) {
            throw new Error('Discounted price must be less than regular price');
        }
    }
    
    return true;
}

function validateSelection(name, errorMessage) {
    const selected = document.querySelector(`input[name="${name}"]:checked`);
    if (!selected) {
        const errorDiv = document.createElement('div');
        errorDiv.className = 'text-danger validation-message';
        errorDiv.textContent = errorMessage;
        
        // Find the container for this radio group
        const container = document.querySelector(`input[name="${name}"]`).closest('.row');
        // Remove any existing error messages
        container.querySelectorAll('.validation-message').forEach(el => el.remove());
        // Add the new error message
        container.appendChild(errorDiv);
        return false;
    }
    return true;
}

function validateVendorSelection() {
    return validateSelection('VendorId', 'Please select a vendor');
}

function validateCategorySelection() {
    return validateSelection('catId', 'Please select a category');
}

//---------------------------------------------------------------------------------------