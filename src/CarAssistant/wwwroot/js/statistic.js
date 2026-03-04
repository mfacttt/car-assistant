function switchTab(tab) {
    const btnFuel = document.getElementById('tabFuelStats');
    const btnConsumption = document.getElementById('tabConsumptionStats');
    const btnOther = document.getElementById('tabOtherStats');

    const statsFuel = document.getElementById('statsFuel');
    const statsConsumption = document.getElementById('statsConsumption');
    const statsOther = document.getElementById('statsOther');

    [btnFuel, btnConsumption, btnOther].forEach(btn => {
        btn.classList.remove('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        btn.classList.add('text-slate-400');
    });

    statsFuel.classList.add('hidden');
    statsConsumption.classList.add('hidden');
    statsOther.classList.add('hidden');

    if (tab === 'fuel') {
        btnFuel.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        statsFuel.classList.remove('hidden');
    } else if (tab === 'consumption') {
        btnConsumption.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        statsConsumption.classList.remove('hidden');
    } else if (tab === 'other') {
        btnOther.classList.add('active', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30');
        statsOther.classList.remove('hidden');
    }
}
