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

    if (!priceSpan || !stockSpan || !variantInput || !addBtn || !sizeSelect) return;

    let selectedColorId = null;
    let selectedSizeId = null;

    const resetUI = () => {
        priceSpan.innerText = '—';
        stockSpan.innerText = '';
        variantInput.value = '';
        addBtn.disabled = true;
    };

    const updateVariant = () => {
        if (!selectedColorId || !selectedSizeId) {
            resetUI();
            return;
        }

        // Находим вариант по цвету и размеру
        const variant = variants.find(v =>
            v.colorId === selectedColorId &&
            v.sizeId === selectedSizeId
        );

        if (!variant) {
            resetUI();
            return;
        }

        // Отображаем цену и остаток
        priceSpan.innerText = variant.price;
        stockSpan.innerText = `Остаток: ${variant.stockQuantity}`;
        variantInput.value = variant.id;

        // Кнопка активна только если вариант доступен и есть в наличии
        addBtn.disabled = !variant.isAvailable || variant.stockQuantity <= 0;
    };

    // Выбор цвета
    document.querySelectorAll('.color-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            selectedColorId = parseInt(this.dataset.colorId, 10);

            // Выделение выбранного цвета
            document.querySelectorAll('.color-btn').forEach(c => c.classList.remove('active'));
            this.classList.add('active');

            // Смена фото
            if (bikeImage && this.dataset.photo) {
                bikeImage.src = this.dataset.photo;
            }

            updateVariant();
        });
    });

    // Выбор размера
    sizeSelect.addEventListener('change', function () {
        selectedSizeId = parseInt(this.value, 10) || null;
        updateVariant();
    });

    // Начальное состояние
    resetUI();
}
