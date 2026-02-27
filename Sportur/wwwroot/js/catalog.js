document.addEventListener('DOMContentLoaded', () => {
    initCatalogDetails();
});

function initCatalogDetails() {
    const detailsContainer = document.getElementById('catalogDetails');
    if (!detailsContainer) return;

    const variantsJson = detailsContainer.dataset.variants;
    if (!variantsJson) return;

    const variants = JSON.parse(variantsJson);

    const priceSpan = document.getElementById('price');
    const stockSpan = document.getElementById('stock');
    const variantInput = document.getElementById('variantId');
    const addBtn = document.getElementById('addToCartBtn');
    const sizeSelect = document.getElementById('sizeSelect');
    const bikeImage = document.getElementById('bikeImage');
    const title = document.getElementById('bicycleVariantTitle');

    if (!priceSpan || !stockSpan || !variantInput || !addBtn || !sizeSelect)
        return;

    let selectedColorId = null;
    let selectedSize = null;

    const defaultTitle = title
        ? `Велосипед ${title.dataset.brand} ${title.dataset.model}`
        : '';

    // =========================
    // Сброс UI
    // =========================
    function resetUI() {
        priceSpan.innerText = '—';
        stockSpan.innerText = '';
        variantInput.value = '';
        addBtn.disabled = true;

        if (title) {
            title.innerText = defaultTitle;
        }
    }

    // =========================
    // Обновление размеров по цвету
    // =========================
    function updateSizesForColor(colorId) {

        const availableSizes = variants
            .filter(v => v.colorId === colorId)
            .map(v => v.sizeName);

        Array.from(sizeSelect.options).forEach(option => {
            if (!option.value) return;

            const isAvailable = availableSizes.includes(option.value);
            option.disabled = !isAvailable;
        });

        // сбрасываем выбранный размер если он не подходит
        if (!availableSizes.includes(selectedSize)) {
            sizeSelect.value = '';
            selectedSize = null;
        }

        // авто-выбор первого доступного
        const firstAvailable = variants.find(v =>
            v.colorId === colorId &&
            v.isAvailable &&
            v.stockQuantity > 0
        );

        if (firstAvailable) {
            sizeSelect.value = firstAvailable.sizeName;
            selectedSize = firstAvailable.sizeName;
        }
    }

    // =========================
    // Обновление варианта
    // =========================
    function updateVariant() {

        if (!selectedColorId || !selectedSize) {
            resetUI();
            return;
        }

        const variant = variants.find(v =>
            v.colorId === selectedColorId &&
            v.sizeName === selectedSize
        );

        if (!variant) {
            resetUI();
            return;
        }

        if (title) {
            title.innerText =
                `${defaultTitle} ${variant.colorName} ${variant.sizeName}`;
        }

        priceSpan.innerText = variant.price.toLocaleString('ru-RU');
        stockSpan.innerText = `Остаток: ${variant.stockQuantity}`;

        variantInput.value = variant.id;

        addBtn.disabled =
            !variant.isAvailable || variant.stockQuantity <= 0;
    }

    // =========================
    // Выбор цвета
    // =========================
    document.querySelectorAll('.color-btn').forEach(btn => {
        btn.addEventListener('click', function () {

            selectedColorId = parseInt(this.dataset.colorId, 10);

            document
                .querySelectorAll('.color-btn')
                .forEach(c => c.classList.remove('active'));

            this.classList.add('active');

            if (bikeImage && this.dataset.photo) {
                bikeImage.src = this.dataset.photo;
            }

            updateSizesForColor(selectedColorId);
            updateVariant();
        });
    });

    // =========================
    // Выбор размера
    // =========================
    sizeSelect.addEventListener('change', function () {
        selectedSize = this.value || null;
        updateVariant();
    });

    // =========================
    // Авто-выбор первого цвета
    // =========================
    const firstColorBtn = document.querySelector('.color-btn');
    if (firstColorBtn) {
        firstColorBtn.click();
    } else {
        resetUI();
    }
}