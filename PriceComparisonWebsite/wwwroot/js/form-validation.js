function showError(element, message) { // Display validation error message
    const $element = $(element);
    const $validationSpan = $element.siblings('.text-danger');
    $validationSpan.text(message);
}

function validateUrl(url) { // Check if URL is valid
    try {
        new URL(url);
        return true;
    } catch {
        return false;
    }
}

function validatePrice(price, discountedPrice = null) { // Check if price is valid discounted price is null by default (no discount)
    const priceValue = parseFloat(price);
    
    if (isNaN(priceValue) || priceValue <= 0) {
        throw new Error('Price must be greater than 0');
    }
    
    if (discountedPrice !== null) { // Check if discounted price is valid
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

function validateSelection(name, errorMessage) { // Check if radio button group has a selection
    const $selected = $(`input[name="${name}"]:checked`);
    const $validationSpan = $(`[data-valmsg-for="${name}"]`);
    
    if ($selected.length === 0) {
        // Customize error message based on field name
        const friendlyName = name === 'VendorId' ? 'vendor' : 
                           name === 'CategoryId' ? 'category' : 
                           name.toLowerCase();
        $validationSpan.text(`Please select a ${friendlyName}`).show();
        return false;
    }
    
    $validationSpan.text('').hide();
    return true;
}

function initializeFormValidation(formSelector) { // Initialize form validation
    const $form = $(formSelector);
    
    $form.on('submit', function(e) {
        e.preventDefault();
        let isValid = true;
        
        // Clear all validation messages
        $('.text-danger').text('');
        
        // Common URL validation
        $('input[type="url"]').each(function() {
            if (!validateUrl($(this).val())) {
                showError(this, 'Please enter a valid URL');
                isValid = false;
            }
        });
        
        // Price validation if price fields exist
        const $price = $('#Price');
        const $isDiscounted = $('#isDiscounted');
        const $discountedPrice = $('#DiscountedPrice');
        
        if ($price.length) { // Check if price field exists
            try {
                validatePrice(
                    $price.val(),
                    $isDiscounted.is(':checked') ? $discountedPrice.val() : null
                );
            } catch (error) {
                showError($isDiscounted.is(':checked') ? $discountedPrice : $price, error.message);
                isValid = false;
            }
        }
        
        // Radio button group validation
        if ($('input[type="radio"]').length) {
            const radioGroups = [...new Set($('input[type="radio"]').map(function() {
                return $(this).attr('name');
            }).get())];
            
            radioGroups.forEach(name => {
                if (!validateSelection(name)) {
                    isValid = false;
                }
            });
        }
        
        if (isValid) {
            this.submit();
        }
    });
}
