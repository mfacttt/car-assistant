function setFormEnabled(container, enabled) {
    if (!container) return;
    const controls = container.querySelectorAll('input, select, textarea');
    controls.forEach(ctrl => {
        ctrl.disabled = !enabled;
    });
}

function switchTab(tab) {
    const btnService = document.getElementById('tabService');
    const btnFuel = document.getElementById('tabFuel');
    const btnOther = document.getElementById('tabOther');

    const formService = document.getElementById('formService');
    const formFuel = document.getElementById('formFuel');
    const formOther = document.getElementById('formOther');

    [btnService, btnFuel, btnOther].forEach(btn => {
        if (!btn) return;
        btn.classList.remove('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        btn.classList.add('text-slate-400');
    });

    if (formService) formService.classList.add('hidden');
    if (formFuel) formFuel.classList.add('hidden');
    if (formOther) formOther.classList.add('hidden');

    const typeInput = document.getElementById('expenseType');

    if (tab === 'service') {
        if (btnService) {
            btnService.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        }
        if (formService) formService.classList.remove('hidden');
        setFormEnabled(formService, true);
        setFormEnabled(formFuel, false);
        setFormEnabled(formOther, false);
        if (typeInput) typeInput.value = 'Service';
    } else if (tab === 'fuel') {
        if (btnFuel) {
            btnFuel.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        }
        if (formFuel) formFuel.classList.remove('hidden');
        setFormEnabled(formService, false);
        setFormEnabled(formFuel, true);
        setFormEnabled(formOther, false);
        if (typeInput) typeInput.value = 'Fuel';
    } else if (tab === 'other') {
        if (btnOther) {
            btnOther.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        }
        if (formOther) formOther.classList.remove('hidden');
        setFormEnabled(formService, false);
        setFormEnabled(formFuel, false);
        setFormEnabled(formOther, true);
        if (typeInput) typeInput.value = 'Other';
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('expenseForm');
    if (form) {
        // Инициализируем состояние: активен сервисный таб по умолчанию
        switchTab('service');

        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const formData = new FormData(form);

            fetch(form.action, {
                method: 'POST',
                body: formData
            }).then(response => {
                if (response.redirected) {
                    window.location.href = response.url;
                } else {
                    window.location.reload();
                }
            }).catch(error => {
                console.error('[consumation] error submitting expense form', error);
            });
        });
    }
});
