document.addEventListener('DOMContentLoaded', () => {
    initCheckoutGuard();
});

function initCheckoutGuard() {
    const checkoutForm = document.getElementById('checkoutForm');
    if (!checkoutForm) return;

    const isAuthenticated = checkoutForm.dataset.authenticated === 'true';
    if (isAuthenticated) return;

    const warning = document.getElementById('checkoutAuthWarning');

    checkoutForm.addEventListener('submit', event => {
        event.preventDefault();
        if (warning) {
            warning.classList.add('checkout-auth-warning-attention');
        }
    });
}
