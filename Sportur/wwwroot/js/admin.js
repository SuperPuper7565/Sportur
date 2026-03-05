document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('[data-variant-form="true"]');
    if (!form) return;

    const modelSelect = form.querySelector('[data-variant-model="true"]');
    const colorSelect = form.querySelector('[data-variant-color="true"]');

    if (!modelSelect || !colorSelect) return;

    const endpoint = modelSelect.dataset.optionsEndpoint;

    const resetSelect = (select, placeholder) => {
        select.innerHTML = `<option value="">${placeholder}</option>`;
        select.disabled = true;
    };

    const fillColorSelect = (items) => {
        resetSelect(colorSelect, '-- выберите цвет --');
        items.forEach(item => {
            const option = document.createElement('option');
            option.value = item.id;
            option.textContent = item.label;
            colorSelect.appendChild(option);
        });
        colorSelect.disabled = items.length === 0;
    };

    const loadOptions = async (modelId) => {
        try {
            const res = await fetch(`${endpoint}?modelId=${modelId}`, {
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            });

            if (!res.ok) return;

            const data = await res.json();

            fillColorSelect(data.colors || []);
        } catch (e) {
            console.error('Ошибка загрузки опций модели:', e);
        }
    };

    resetSelect(colorSelect, '-- выберите цвет --');

    if (modelSelect.value) {
        const modelId = parseInt(modelSelect.value, 10);
        if (modelId > 0) loadOptions(modelId);
    }

    modelSelect.addEventListener('change', () => {
        const modelId = parseInt(modelSelect.value, 10);
        if (!modelId || modelId <= 0) {
            resetSelect(colorSelect, '-- выберите цвет --');
            return;
        }

        loadOptions(modelId);
    });
});