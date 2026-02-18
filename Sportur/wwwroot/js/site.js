document.addEventListener('DOMContentLoaded', () => {
    // Инициализируем логику страницы детального просмотра товара.
    initCatalogDetails();

    // Инициализируем фильтрацию цветов/ростовок в админ-форме варианта.
    initAdminVariantFiltering();

    // Инициализируем защиту оформления заказа для неавторизованных пользователей.
    initCheckoutGuard();
});

function initCatalogDetails() {
    // Скрипт работает только на странице товара, где есть контейнер с данными вариантов.
    const detailsContainer = document.getElementById('catalogDetails');
    if (!detailsContainer) {
        return;
    }

    // Варианты приходят с сервера в формате JSON через data-атрибут.
    const variantsJson = detailsContainer.dataset.variants;
    if (!variantsJson) {
        return;
    }

    const variants = JSON.parse(variantsJson);

    const priceSpan = document.getElementById('price');
    const stockSpan = document.getElementById('stock');
    const variantInput = document.getElementById('variantId');
    const addBtn = document.getElementById('addToCartBtn');
    const sizeSelect = document.getElementById('sizeSelect');
    const bikeImage = document.getElementById('bikeImage');

    // Если обязательные элементы отсутствуют, выходим без ошибок.
    if (!priceSpan || !stockSpan || !variantInput || !addBtn || !sizeSelect) {
        return;
    }

    let selectedColorId = null;
    let selectedSizeId = null;

    const resetUI = (text = '—', stockText = '') => {
        priceSpan.innerText = text;
        stockSpan.innerText = stockText;
        variantInput.value = '';
        addBtn.disabled = true;
    };

    const updateVariant = () => {
        // Пока не выбран цвет и размер, не позволяем добавлять в корзину.
        if (!selectedColorId || !selectedSizeId) {
            resetUI();
            return;
        }

        // Ищем только подходящий и доступный вариант.
        const variant = variants.find(v =>
            v.colorId === selectedColorId &&
            v.sizeId === selectedSizeId &&
            v.isAvailable
        );

        if (!variant || variant.stockQuantity <= 0) {
            resetUI('Нет в наличии', 'Остаток: 0');
            return;
        }

        // Обновляем UI и скрытое поле, которое отправляется в корзину.
        priceSpan.innerText = variant.price;
        stockSpan.innerText = `Остаток: ${variant.stockQuantity}`;
        variantInput.value = variant.id;
        addBtn.disabled = false;
    };

    document.querySelectorAll('.color-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            selectedColorId = parseInt(this.dataset.colorId, 10);

            // Визуально выделяем выбранный цвет.
            document.querySelectorAll('.color-btn')
                .forEach(colorBtn => colorBtn.classList.remove('active'));

            this.classList.add('active');

            // При наличии фото цвета сразу обновляем картинку велосипеда.
            if (bikeImage && this.dataset.photo) {
                bikeImage.src = this.dataset.photo;
            }

            updateVariant();
        });
    });

    sizeSelect.addEventListener('change', function () {
        selectedSizeId = parseInt(this.value, 10) || null;
        updateVariant();
    });

    // Начальное состояние страницы до выбора параметров.
    resetUI();
}

function initAdminVariantFiltering() {
    const form = document.querySelector('[data-variant-form="true"]');
    if (!form) return;

    const modelSelect = form.querySelector('[data-variant-model="true"]');
    const colorSelect = form.querySelector('[data-variant-color="true"]');
    const sizeSelect = form.querySelector('[data-variant-size="true"]');

    if (!modelSelect || !colorSelect || !sizeSelect) return;

    const endpoint = modelSelect.dataset.optionsEndpoint;

    const clearSelect = (select, placeholder) => {
        select.innerHTML = '';
        const opt = document.createElement('option');
        opt.value = '';
        opt.textContent = placeholder;
        select.appendChild(opt);
        select.value = '';
        select.disabled = true;
    };

    const fillSelect = (select, placeholder, items) => {
        select.innerHTML = '';
        const opt = document.createElement('option');
        opt.value = '';
        opt.textContent = placeholder;
        select.appendChild(opt);

        items.forEach(x => {
            const o = document.createElement('option');
            o.value = x.id;
            o.textContent = x.label;
            select.appendChild(o);
        });

        select.disabled = items.length === 0;
    };

    clearSelect(colorSelect, '-- выберите цвет --');
    clearSelect(sizeSelect, '-- выберите размер --');

    modelSelect.addEventListener('change', async () => {
        const modelId = parseInt(modelSelect.value, 10);

        clearSelect(colorSelect, '-- выберите цвет --');
        clearSelect(sizeSelect, '-- выберите размер --');

        if (!Number.isInteger(modelId) || modelId <= 0) {
            return;
        }

        const res = await fetch(`${endpoint}?modelId=${modelId}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        if (!res.ok) return;

        const data = await res.json();

        fillSelect(colorSelect, '-- выберите цвет --', data.colors || []);
        fillSelect(sizeSelect, '-- выберите размер --', data.sizes || []);
    });

}



    modelSelect.addEventListener('change', () => {
        loadModelOptions(modelSelect.value);
    });

    if (modelSelect.value) {
        loadModelOptions(modelSelect.value);
    } else {
        resetDependentSelects();
    }
}

function initCheckoutGuard() {
    // Скрипт нужен только на странице корзины.
    const checkoutForm = document.getElementById('checkoutForm');
    if (!checkoutForm) {
        return;
    }

    // Признак авторизации приходит с сервера через data-атрибут.
    const isAuthenticated = checkoutForm.dataset.authenticated === 'true';
    if (isAuthenticated) {
        return;
    }

    const warning = document.getElementById('checkoutAuthWarning');

    // Даже если кнопку активируют вручную через DevTools, отправку всё равно блокируем.
    checkoutForm.addEventListener('submit', event => {
        event.preventDefault();

        // Подсветим предупреждение, чтобы пользователь явно увидел причину блокировки.
        if (warning) {
            warning.classList.add('checkout-auth-warning-attention');
        }
    });
}

