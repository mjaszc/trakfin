// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    var startDateInput = document.querySelector('.addTimeInterval');

    if (startDateInput) {
        startDateInput.addEventListener('input', function () {
            var value = this.value;
            console.log(value);

            const date = new Date(`${value}`);
            date.setMonth(date.getMonth() + 1);
            var newValue = date.toISOString().slice(0, 10);

            document.querySelector('.autoIncrementDate').value = newValue;
            console.log(newValue);
        });
    }
});
