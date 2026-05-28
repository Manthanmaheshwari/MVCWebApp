/**
 * Utility helpers for UI interactions, toast notifications, and client-side catalog filtering.
 */

/**
 * Toggles the visibility class of a modal element.
 * @param {string} id - The DOM element ID of the target modal.
 */
function toggleModal(id) {
    document.getElementById(id).classList.toggle('show');
}

/**
 * Dynamically constructs and displays a temporary toast notification on the viewport.
 * @param {string} type - The category of toast notification (e.g., 'success', 'error').
 * @param {string} message - The text content to display in the notification.
 */
function showToast(type, message) {
    var div = document.createElement('div');
    div.className = 'toast-' + type;
    div.innerText = message;
    div.style.position = 'fixed';
    div.style.right = '20px';
    div.style.bottom = '20px';
    div.style.zIndex = 9999;
    document.body.appendChild(div);
    
    setTimeout(() => div.remove(), 3000);
}

/**
 * Filters the visibility of matching elements on the catalog page based on user search input.
 */
function filterProducts() {
    var q = document.getElementById('searchBox').value.toLowerCase();
    document.querySelectorAll('.product-card').forEach(c => {
        c.style.display = c.innerText.toLowerCase().includes(q) ? 'block' : 'none';
    });
}
