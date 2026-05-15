const API_BASE = '';
 
// ── Auth helpers
 
function getToken()  { return localStorage.getItem('token'); }
function getUserId() { return localStorage.getItem('userId'); }
function getUserName() {
    const first = localStorage.getItem('firstName') || '';
    const last  = localStorage.getItem('lastName')  || '';
    return (first + ' ' + last).trim() || 'User';
}
 
function logout() {
    localStorage.clear();
    window.location.href = '/login.html';
}
 
function requireAuth() {
    if (!getToken()) {
        window.location.href = '/login.html';
        return false;
    }
    return true;
}
 
// ── Tab switcher (login.html)
 
function switchTab(tab) {
    const isLogin = tab === 'login';
    document.getElementById('formLogin').classList.toggle('hidden', !isLogin);
    document.getElementById('formRegister').classList.toggle('hidden', isLogin);
    document.getElementById('tabLogin').classList.toggle('active', isLogin);
    document.getElementById('tabRegister').classList.toggle('active', !isLogin);
    setAuthStatus('', false);
}
 
function setAuthStatus(msg, isError = true) {
    const el = document.getElementById('authStatus');
    if (!el) return;
    el.textContent = msg;
    el.className = 'auth-status' + (isError ? '' : ' ok');
}
 
// ── Register
 
async function doRegister() {
    const firstName   = document.getElementById('regFirst').value.trim();
    const lastName    = document.getElementById('regLast').value.trim();
    const email       = document.getElementById('regEmail').value.trim();
    const phoneNumber = document.getElementById('regPhone').value.trim();
    const password    = document.getElementById('regPassword').value;
 
    if (!firstName || !lastName || !email || !password) {
        setAuthStatus('Please fill in all required fields.');
        return;
    }
 
    try {
        const res = await fetch(`${API_BASE}/api/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ firstName, lastName, email, phoneNumber, password })
        });
 
        if (!res.ok) {
            const msg = await res.text();
            setAuthStatus(msg || 'Registration failed.');
            return;
        }
 
        setAuthStatus('Account created! You can now log in.', false);
        switchTab('login');
        document.getElementById('loginEmail').value = email;
    } catch {
        setAuthStatus('Could not reach the server. Is the backend running?');
    }
}
 
// ── Login
 
async function doLogin() {
    const email    = document.getElementById('loginEmail').value.trim();
    const password = document.getElementById('loginPassword').value;
 
    if (!email || !password) {
        setAuthStatus('Please enter email and password.');
        return;
    }
 
    try {
        const res = await fetch(`${API_BASE}/api/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
 
        if (!res.ok) {
            setAuthStatus('Invalid email or password.');
            return;
        }
 
        const data = await res.json();
 
        localStorage.setItem('token',     data.token);
        localStorage.setItem('userId',    data.id);
        localStorage.setItem('firstName', data.firstName);
        localStorage.setItem('lastName',  data.lastName);
 
        window.location.href = '/home.html';
    } catch {
        setAuthStatus('Could not reach the server. Is the backend running?');
    }
}
 
// ── CV form submit (index.html)
 
async function send() {
    if (!requireAuth()) return;
 
    const skills     = document.getElementById('skills')?.value || '';
    const education  = document.getElementById('education')?.value || '';
    const aboutMe    = document.getElementById('additionalInfo')?.value || '';
    const languages  = document.getElementById('languages')?.value || '';
    const template   = document.getElementById('templateSelect')?.value || 'modern';
 
    const status = document.getElementById('result');
    if (status) status.textContent = 'Generating PDF…';
 
    try {
        const res = await fetch(
            `${API_BASE}/api/cv/pdf/${getUserId()}`,
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${getToken()}`
                },
                body: JSON.stringify({ aboutMe, skills, education, languages })
            }
        );
 
        if (res.status === 401) {
            logout();
            return;
        }
 
        if (!res.ok) {
            if (status) status.textContent = 'Error generating PDF.';
            return;
        }
 
        // Download the PDF
        const blob = await res.blob();
        const url  = URL.createObjectURL(blob);
        const a    = document.createElement('a');
        a.href     = url;
        a.download = `cv-${template}.pdf`;
        a.click();
        URL.revokeObjectURL(url);
 
        if (status) status.textContent = 'CV downloaded successfully!';
    } catch {
        if (status) status.textContent = 'Could not reach the server.';
    }
}
 
// ── Home page init
 
function initHome() {
    if (!requireAuth()) return;
 
    const nameEl = document.getElementById('welcomeName');
    if (nameEl) nameEl.textContent = getUserName();
 
    const logoutBtn = document.getElementById('logoutBtn');
    if (logoutBtn) logoutBtn.addEventListener('click', logout);
}
 
// ── CV page init
 
function initCvPage() {
    if (!requireAuth()) return;
}
 
// ── Auto-init based on page
 
document.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname;
    if (path.includes('home.html') || path === '/' || path === '') initHome();
    if (path.includes('index.html')) initCvPage();
});
