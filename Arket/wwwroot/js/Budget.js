// Auth check
if (!localStorage.getItem('token')) window.location.href = '/login.html';

// ── Section config
const SECTIONS = [
    { id: 'income',    totalId: 't-income',    isIncome: true  },
    { id: 'loans',     totalId: 't-loans',     isIncome: false },
    { id: 'insurance', totalId: 't-insurance', isIncome: false },
    { id: 'housing',   totalId: 't-housing',   isIncome: false },
    { id: 'transport', totalId: 't-transport', isIncome: false },
    { id: 'health',    totalId: 't-health',    isIncome: false },
    { id: 'food',      totalId: 't-food',      isIncome: false },
    { id: 'clothes',   totalId: 't-clothes',   isIncome: false },
    { id: 'leisure',   totalId: 't-leisure',   isIncome: false },
];

// ── Helpers
function fmt(n) {
    return n.toLocaleString('nb-NO') + ' kr';
}

function sectionSum(sectionEl) {
    return Array.from(sectionEl.querySelectorAll('input'))
        .reduce((sum, inp) => sum + (parseFloat(inp.value) || 0), 0);
}

// ── Accordion
function toggle(id) {
    document.getElementById(id).classList.toggle('open');
}

// ── Recalculate summary
function recalc() {
    let totalIncome = 0, totalExpenses = 0;

    SECTIONS.forEach(({ id, totalId, isIncome }) => {
        const sec = document.getElementById('s-' + id);
        const sum = sectionSum(sec);
        document.getElementById(totalId).textContent = sum > 0 ? fmt(sum) : '0 kr';
        if (isIncome) totalIncome += sum;
        else          totalExpenses += sum;
    });

    const balance = totalIncome - totalExpenses;
    document.getElementById('sum-income').textContent   = fmt(totalIncome);
    document.getElementById('sum-expenses').textContent = fmt(totalExpenses);
    document.getElementById('sum-balance').textContent  = fmt(Math.abs(balance));

    const row = document.getElementById('sum-row-balance');
    row.querySelector('span:first-child').textContent = balance >= 0 ? 'Balance' : 'Deficit';
    row.classList.toggle('negative', balance < 0);
}

// ── Collect data and send to backend for Excel
async function exportToExcel() {
    const btn = document.querySelector('.export-btn');
    btn.disabled = true;
    btn.textContent = '⏳ Generating...';

    // Build payload matching BudgetDto
    const sections = SECTIONS.map(({ id, isIncome }) => {
        const sec   = document.getElementById('s-' + id);
        const name  = sec.querySelector('.section-name').textContent;
        const items = Array.from(sec.querySelectorAll('.field-row')).map(row => ({
            label:  row.querySelector('.field-name').textContent,
            amount: parseFloat(row.querySelector('input').value) || 0
        }));
        return { name, isIncome, items };
    });

    try {
        const res = await fetch('/api/budget/export', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify({ sections })
        });

        if (res.status === 401) { window.location.href = '/login.html'; return; }
        if (!res.ok) { alert('Export failed. Please try again.'); return; }

        // Download the file
        const blob = await res.blob();
        const url  = URL.createObjectURL(blob);
        const a    = document.createElement('a');
        a.href     = url;
        a.download = `budget-${new Date().toISOString().slice(0, 7)}.xlsx`;
        a.click();
        URL.revokeObjectURL(url);

    } catch {
        alert('Could not reach the server.');
    } finally {
        btn.disabled = false;
        btn.textContent = '⬇ Export Excel';
    }
}