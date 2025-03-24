// Sanitize input value based on regex pattern and preserve cursor position
function sanitizeInputWithCursor(input, pattern, replacement = '') {
    // Store cursor position before replacing
    const cursorPos = input.selectionStart;
    // Get original value and length
    const originalValue = input.value;
    const originalLength = originalValue.length;

    // Replace unwanted characters
    const sanitizedValue = originalValue.replace(pattern, replacement);

    // Only update if values are different
    if (originalValue !== sanitizedValue) {
        // Set the sanitized value
        input.value = sanitizedValue;

        // Calculate how many characters were removed
        const diff = originalLength - sanitizedValue.length;

        // Adjust cursor position (move it back by the number of removed characters)
        const newPos = Math.max(0, cursorPos - diff);

        // Restore cursor position
        input.setSelectionRange(newPos, newPos);
    }

    return sanitizedValue;
}

// Format number with commas for thousands
function formatNumberWithCommas(value) {
    return value.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
}

// Format cents with leading zero
function formatCents(input) {
    let value = parseInt(input.value) || 0;
    value = Math.min(99, Math.max(0, value));
    input.value = value.toString().padStart(2, '0');
}

// Remove non-numeric characters
function sanitizeNumeric(value) {
    return value.replace(/[^0-9]/g, '');
}

// Remove leading zeros except for a single one if value would be zero
function removeLeadingZeros(value) {
    if (value.length > 1) {
        return value.replace(/^0+/, '');
    } else if (value === '') {
        return '0';
    }
    return value;
}