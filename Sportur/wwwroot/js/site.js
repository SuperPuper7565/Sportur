function changeColor(photoUrl) {
    const img = document.getElementById('bikeImage');
    if (img) {
        img.src = photoUrl;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const sizeSelect = document.getElementById('sizeSelect');
    const priceSpan = document.getElementById('price');

    if (!sizeSelect || !priceSpan) return;

    sizeSelect.addEventListener('change', function () {
        const price =
            this.options[this.selectedIndex].dataset.price;

        priceSpan.innerText = price;
    });
});
