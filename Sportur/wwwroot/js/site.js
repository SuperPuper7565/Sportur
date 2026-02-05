document.addEventListener('DOMContentLoaded', () => {
    initCatalogDetails();
    initAdminVariantFiltering();
    initCheckoutGuard();
})

function initCatalogDetails() {
    const detailsContainer = document.getElementById('catalogDetails');
    if (!detailsContainer) {
        return;
    }

    const variantsJson = detailsContainer.dataset.variants;
    if (!variantsJson) {
        return;
    }

    const variants = JSON.parse(variantsJson);
    let selectedColorId = null;
    let selectedSizeId = null;

    const variants = JSON.parse(variantsJson);
    let selectedColorId = null;
    let selectedSizeId = null;

    if (!priceSpan || !variantInput || !addBtn || !sizeSelect) {
        return;
    }

    document.querySelectorAll('.color-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            selectedColorId = parseInt(this.dataset.colorId, 10);

            document.querySelectorAll('.color-btn')
                .forEach(colorBtn => colorBtn.classList.remove('active'));

            this.classList.add('active');

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
}

function updateVariant() {
    if (!selectedColorId || !selectedSizeId) {
        resetUI();
        return;
    }

    const variant = variants.find(v =>
        v.colorId === selectedColorId &&
        v.sizeId === selectedSizeId &&
        v.isAvailable
    );

    if (!variant || variant.stockQuantity <= 0) {
        resetUI('Нет в наличии');
        return;
    }

    priceSpan.innerText = variant.price;
    variantInput.value = variant.id;
    addBtn.disabled = false;
}

function resetUI(text = '—') {
    priceSpan.innerText = text;
    variantInput.value = '';
    addBtn.disabled = true;
}

function initAdminVariantFiltering() {
    const form = document.querySelector('[data-variant-form="true"]');
    if (!form) {
        return;
    }

    const modelSelect = form.querySelector('[data-variant-model="true"]');
    const colorSelect = form.querySelector('[data-variant-color="true"]');
    const sizeSelect = form.querySelector('[data-variant-size="true"]');

    if (!modelSelect || !colorSelect || !sizeSelect) {
        return;
    }

    const filterSelect = (select, modelId) => {
        const options = Array.from(select.querySelectorAll('option[data-model-id]'));
        let hasVisibleSelected = false;

        options.forEach(option => {
            const matches = modelId && option.dataset.modelId === modelId;
            option.hidden = !matches;

            if (!matches && option.selected) {
                option.selected = false;
            }

            if (matches && option.selected) {
                hasVisibleSelected = true;
            }
        });

        if (!hasVisibleSelected) {
            const firstVisible = options.find(option => !option.hidden);
            if (firstVisible) {
                firstVisible.selected = true;
            }
        }
    };

    const applyFilters = () => {
        const modelId = modelSelect.value;
        filterSelect(colorSelect, modelId);
        filterSelect(sizeSelect, modelId);
    };

    modelSelect.addEventListener('change', applyFilters);
    applyFilters();
}

function initCheckoutGuard() {
    const checkoutForm = document.getElementById('checkoutForm');
    if (!checkoutForm) {
        return;
    }

    const isAuthenticated = checkoutForm.dataset.authenticated === 'true';
    if (isAuthenticated) {
        return;
    }

    checkoutForm.addEventListener('submit', event => {
        event.preventDefault();
    });
}

// ---------- ADMIN VARIANT FILTERING ----------
document.addEventListener('DOMContentLoaded', () => {
    const form = document.querySelector('[data-variant-form="true"]');
    if (!form) {
        return;
    }

    const modelSelect = form.querySelector('[data-variant-model="true"]');
    const colorSelect = form.querySelector('[data-variant-color="true"]');
    const sizeSelect = form.querySelector('[data-variant-size="true"]');

    if (!modelSelect || !colorSelect || !sizeSelect) {
        return;
    }

    const filterSelect = (select, modelId) => {
        const options = Array.from(select.querySelectorAll('option[data-model-id]'));
        let hasVisibleSelected = false;

        options.forEach(option => {
            const matches = modelId && option.dataset.modelId === modelId;
            option.hidden = !matches;
            if (!matches && option.selected) {
                option.selected = false;
            }
            if (matches && option.selected) {
                hasVisibleSelected = true;
            }
        });

        if (!hasVisibleSelected) {
            const firstVisible = options.find(option => !option.hidden);
            if (firstVisible) {
                firstVisible.selected = true;
            }
        }
    };

    const applyFilters = () => {
        const modelId = modelSelect.value;
        filterSelect(colorSelect, modelId);
        filterSelect(sizeSelect, modelId);
    };

    modelSelect.addEventListener('change', applyFilters);
    applyFilters();
});