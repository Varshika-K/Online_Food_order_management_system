document.addEventListener("DOMContentLoaded", () => {
    const toast = document.getElementById("toast");
    if (toast) {
        setTimeout(() => {
            toast.style.opacity = "0";
            toast.style.transition = "opacity .4s";
            setTimeout(() => toast.remove(), 450);
        }, 2800);
    }
});
