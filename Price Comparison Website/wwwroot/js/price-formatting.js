document.addEventListener('DOMContentLoaded', function() { // Format prices on page load and when prices are updated
    function formatPrices() {
        document.querySelectorAll('[data-price]').forEach(element => {
            const price = parseFloat(element.dataset.price);
            if (!isNaN(price)) {
                element.textContent = window.currencyFormatter.format(price);
            }
        });
    }

    formatPrices();
    document.addEventListener('pricesUpdated', formatPrices);
});

function formatPrice(price) { 
    return window.currencyFormatter.format(price);
}
