document.addEventListener('DOMContentLoaded', () => {
    initAdminVariantFiltering();
});

function initAdminVariantFiltering() {
    const form = document.querySelector('[data-variant-form="true"]');
    if (!form) return;

    const modelSelect = form.querySelector('[data-variant-model="true"]');
    const colorSelect = form.querySelector('[data-variant-color="true"]');
    const sizeSelect = form.querySelector('[data-variant-size="true"]');

    if (!modelSelect || !colorSelect || !sizeSelect) return;

    const endpoint = modelSelect.dataset.optionsEndpoint;

    const resetSelect = (select, placeholder) => {
        select.innerHTML = `<option value="">${placeholder}</option>`;
        select.disabled = true;
    };

    const fillSelect = (select, placeholder, items) => {
        select.innerHTML = `<option value="">${placeholder}</option>`;
        items.forEach(item => {
            select.innerHTML += `<option value="${item.id}">${item.label}</option>`;
        });
        select.disabled = items.length === 0;
    };

    // стартовое состояние
    resetSelect(colorSelect, '-- выберите цвет --');
    resetSelect(sizeSelect, '-- выберите размер --');

    // если модель уже выбрана (например, Edit), сразу подгружаем варианты
    if (modelSelect.value) {
        loadOptions(parseInt(modelSelect.value, 10));
    }

    modelSelect.addEventListener('change', () => {
        const modelId = parseInt(modelSelect.value, 10);

        resetSelect(colorSelect, '-- выберите цвет --');
        resetSelect(sizeSelect, '-- выберите размер --');

        if (!modelId || modelId <= 0) return;

        loadOptions(modelId);
    });

    async function loadOptions(modelId) {
        try {
            const res = await fetch(`${endpoint}?modelId=${modelId}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!res.ok) return;

            const data = await res.json();

            fillSelect(colorSelect, '-- выберите цвет --', data.colors || []);
            fillSelect(sizeSelect, '-- выберите размер --', data.sizes || []);
        } catch (e) {
            console.error('Ошибка загрузки вариантов:', e);
        }
    }
}
