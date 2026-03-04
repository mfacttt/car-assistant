(function () {
    const loginTab = document.getElementById('loginTabBtn');
    const registerTab = document.getElementById('registerTabBtn');
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');

    function setActiveTab(active) {
        loginTab.classList.remove('active-tab', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30', 'shadow-sm');
        registerTab.classList.remove('active-tab', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30', 'shadow-sm');

        if (active === 'login') {
            loginTab.classList.add('active-tab', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30', 'shadow-sm');
            registerTab.classList.add('text-slate-300');
            loginForm.classList.remove('hidden');
            registerForm.classList.add('hidden');
        } else {
            registerTab.classList.add('active-tab', 'bg-purple-600/20', 'text-purple-300', 'border', 'border-purple-500/30', 'shadow-sm');
            loginTab.classList.add('text-slate-300');
            registerForm.classList.remove('hidden');
            loginForm.classList.add('hidden');
        }
    }

    loginTab.addEventListener('click', (e) => {
        e.preventDefault();
        setActiveTab('login');
    });

    registerTab.addEventListener('click', (e) => {
        e.preventDefault();
        setActiveTab('register');
    });

    setActiveTab('login');
})();
