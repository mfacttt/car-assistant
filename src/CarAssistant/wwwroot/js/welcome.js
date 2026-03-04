(function () {
    const vinInput = document.querySelector('input[placeholder*="WAUZZZ"]');
    if (vinInput) {
        vinInput.addEventListener('input', function () {
            this.value = this.value.toUpperCase();
        });
    }
})();
