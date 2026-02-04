let selectedColorId = null;
let selectedSizeId = null;

const priceSpan = document.getElementById('price');
const variantInput = document.getElementById('variantId');
const addBtn = document.getElementById('addToCartBtn');
const bikeImage = document.getElementById('bikeImage');

// ---------- ЦВЕТ ----------
document.querySelectorAll('.color-btn').forEach(btn => {
    btn.addEventListener('click', function () {

        selectedColorId = parseInt(this.dataset.colorId);

        // активная кнопка
        document.querySelectorAll('.color-btn')
            .forEach(b => b.classList.remove('active'));

        this.classList.add('active');

        // фото
        if (bikeImage && this.dataset.photo) {
            bikeImage.src = this.dataset.photo;
        }

        updateVariant();
    });
});

// ---------- РАЗМЕР ----------
const sizeSelect = document.getElementById('sizeSelect');

if (sizeSelect) {
    sizeSelect.addEventListener('change', function () {
        selectedSizeId = parseInt(this.value) || null;
        updateVariant();
    });
}

// ---------- ПОИСК ВАРИАНТА ----------
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
        resetUI("Нет в наличии");
        return;
    }

    // ✔ найден вариант
    priceSpan.innerText = variant.price;
    variantInput.value = variant.id;

    addBtn.disabled = false;
}

// ---------- СБРОС ----------
function resetUI(text = "—") {
    priceSpan.innerText = text;
    variantInput.value = "";
    addBtn.disabled = true;
}
