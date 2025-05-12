let priceChart;

function fillMissingDates(data, days) {
    const filled = [];
    const endDate = moment();
    const startDate = moment().subtract(days, 'days');
    
    // Create a map of existing prices
    const priceMap = new Map();
    let lastKnownPrice = null;
    let firstPriceDate = null;

    if (data && data.dates && data.dates.length > 0) {
        // Sort dates to ensure chronological order
        const sortedDates = data.dates.map((date, i) => ({
            date: moment(date, "YYYY-MM-DD"),
            price: data.prices[i]
        })).sort((a, b) => a.date - b.date);

        // Set the first price date and populate price map
        firstPriceDate = sortedDates[0].date;
        sortedDates.forEach(({ date, price }) => {
            priceMap.set(date.format('YYYY-MM-DD'), price);
            lastKnownPrice = price; // Update last known price
        });
    }

    let currentDate = startDate;
    while (currentDate <= endDate) {
        const dateStr = currentDate.format('YYYY-MM-DD');
        let price = priceMap.get(dateStr);

        if (firstPriceDate && currentDate.isSameOrAfter(firstPriceDate)) {
            price = price || lastKnownPrice;
        }
        
        // Only include points if we have a price
        if (price !== undefined) {
            filled.push({
                date: currentDate.format('MMM DD'),
                fullDate: dateStr,
                price: price
            });
            lastKnownPrice = price;
        }

        currentDate = currentDate.add(1, 'day');
    }

    return filled;
}

function createChart(ctx, data) {
    if (priceChart) {
        priceChart.destroy();
    }

    priceChart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.map(d => d.date),
            datasets: [{
                label: 'Price',
                data: data.map(d => ({ x: d.date, y: d.price, fullDate: d.fullDate })),
                borderColor: '#2ecc71',
                backgroundColor: 'rgba(46, 204, 113, 0.1)',
                fill: true,
                tension: 0.2,
                pointRadius: 3,
                borderWidth: 2,
                spanGaps: false
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: 'nearest'
            },
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        title: function(context) {
                            if (context[0].raw && context[0].raw.fullDate) {
                                return moment(context[0].raw.fullDate).format('MMMM D, YYYY');
                            }
                            return '';
                        },
                        label: function(context) {
                            if (context.raw && context.raw.y !== null) {
                                return window.currencyFormatter.format(context.raw.y);
                            }
                            return 'No price data';
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        maxTicksLimit: 10
                    }
                },
                y: {
                    beginAtZero: false,
                    ticks: {
                        callback: function(value) {
                            return window.currencyFormatter.format(value);
                        }
                    }
                }
            }
        }
    });
}

function showError(chartContainer, message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'text-center text-muted my-5';
    errorDiv.innerHTML = `
        <i class="bi bi-exclamation-circle fs-1"></i>
        <p class="mt-2">${message}</p>
    `;
    chartContainer.innerHTML = '';
    chartContainer.appendChild(errorDiv);
}

async function updateChart(productId, days) {
    const chartContainer = document.getElementById('priceHistoryChart').parentElement;
    const canvas = document.getElementById('priceHistoryChart');

    try {
        const response = await fetch(`/Product/GetPriceHistory/${productId}`);
        const data = await response.json();

        if (!data || (!data.dates && !data.prices)) {
            // No price history - show message and use current price
            const singlePointData = {
                dates: [moment().format('MMM DD')],
                prices: [window.currentPrice]
            };
            const filledData = fillMissingDates(singlePointData, days);
            createChart(canvas.getContext('2d'), filledData);  
            showError(chartContainer, 'No price history available yet. Showing current price only.');
            return;
        }

        if (data.dates.length === 0) {
            showError(chartContainer, 'No price history available.');
            return;
        }

        const filledData = fillMissingDates(data, days);
        createChart(canvas.getContext('2d'), filledData);

    } catch (error) {
        console.error('Error fetching price history:', error);
        showError(chartContainer, 'Failed to load price history.');
    }
}

document.addEventListener('DOMContentLoaded', function() {
    const productId = document.querySelector('input[name="id"]').value;
    window.currentPrice = parseFloat(document.querySelector('[data-price]')?.dataset.price) || 0;

    // Initial load with 90 days
    updateChart(productId, 90);

    // Handle range button clicks
    document.querySelectorAll('[data-range]').forEach(button => {
        button.addEventListener('click', function() {
            // Update active state
            document.querySelectorAll('[data-range]').forEach(b => b.classList.remove('active'));
            this.classList.add('active');
            
            // Update chart
            updateChart(productId, parseInt(this.dataset.range));
        });
    });
});
