// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll('table tbody tr td a').forEach(row => {
    row.addEventListener('click', function () {
        window.location.href = this.dataset.url;
    });
});

document.addEventListener('DOMContentLoaded', function () {
    var startDateInput = document.querySelector('.addTimeInterval');
    var billingCycleType = document.querySelector('.billingCycleType');
    var billingCycleTypeValue = '';

    function updateNextBillingDate() {
        var startDateValue = startDateInput.value;
        if (!startDateValue || !billingCycleTypeValue) {
            alert("Start Date or Billing Cycle Type is not set.");
            return;
        }

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
            // console.log(billingCycleTypeValue);
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