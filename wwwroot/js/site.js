// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// BUDGET PRICE VALIDATION IN EXPENSE
var priceElement = document.getElementById('priceInput');
var budgetSelect = document.getElementById('budgetSelect');

if (priceElement && budgetSelect) {
    var priceElementValue;
    var pricePartFromInput;

    var selectedBudgetValue = budgetSelect.value;
    var selectedBudgetText = budgetSelect.options[budgetSelect.selectedIndex].text;

    function UpdateBudgetAndPriceValue() {
        selectedBudgetValue = budgetSelect.value;
        selectedBudgetText = budgetSelect.options[budgetSelect.selectedIndex].text;

        pricePartFromInput = selectedBudgetText.split(',')[1].trim();
    }

    priceInput.addEventListener('blur', function () {
        priceElementValue = this.value;
    });

    budgetSelect.addEventListener('input', function () {
        UpdateBudgetAndPriceValue();

        if (priceElementValue > pricePartFromInput) {
            console.log("Price is too big for that budget!");
        }
        else {
            console.log("Its ok");
        }
    });
}

// PAGE THEME LOGIC
var themeSwitch = document.getElementById('themeSwitch')

if (themeSwitch) {
    themeSwitch.addEventListener('click', () => {
        let currentTheme = document.documentElement.getAttribute('data-bs-theme');
        let newTheme = currentTheme === "dark" ? "light" : "dark";
        document.documentElement.setAttribute('data-bs-theme', newTheme);
        localStorage.setItem('themePreference', newTheme);
    });
}


document.addEventListener('DOMContentLoaded', () => {
    const savedTheme = localStorage.getItem('themePreference');
    if (savedTheme) {
        document.documentElement.setAttribute('data-bs-theme', savedTheme);
    }
});


// RECURRING TRANSACTIONS 
var addRecurringElement = document.querySelector('.addRecurring');

if (addRecurringElement) {
    addRecurringElement.addEventListener('click', function () {
        window.location.href = this.dataset.url;
    });
}

function resetSelectElement(elementId) {
    var selectElement = document.getElementById(elementId);
    selectElement.selectedIndex = 0;
}


// SUBSCRIPTION AUTOCOMPLETE DATE FUNCTION
document.addEventListener('DOMContentLoaded', function () {
    var startDateInput = document.querySelector('.addTimeInterval');
    var billingCycleType = document.querySelector('.billingCycleType');
    var billingCycleTypeValue = '';

    function updateNextBillingDate() {
        var startDateValue = startDateInput.value;

        const startDate = new Date(`${startDateValue}`);
        switch (billingCycleTypeValue) {
            case "Daily":
                startDate.setDate(startDate.getDate() + 1);
                break;
            case "Weekly":
                startDate.setDate(startDate.getDate() + 7);
                break;
            case "Monthly":
                startDate.setMonth(startDate.getMonth() + 1);
                break;
            case "Yearly":
                startDate.setFullYear(startDate.getFullYear() + 1);
                break;
            default:
                alert("Error: Invalid Billing Cycle Type");
                return;
        }
        var newNextBillingDate = startDate.toISOString().slice(0, 10); // Slicing the time part
        document.querySelector('.autoIncrementDate').value = newNextBillingDate;
        // console.log(newNextBillingDate);
    }

    if (billingCycleType) {
        billingCycleType.addEventListener('change', function () {
            billingCycleTypeValue = this.options[this.selectedIndex].text;
            updateNextBillingDate();
        });
    }

    if (startDateInput) {
        startDateInput.addEventListener('input', function () {
            // console.log(this.value);
            updateNextBillingDate();
        });
    }
});