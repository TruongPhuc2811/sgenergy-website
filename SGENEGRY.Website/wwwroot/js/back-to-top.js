(function () {
    const btn = document.getElementById("backToTop");
    if (!btn) return;

    function update() {
        const y = window.scrollY || document.documentElement.scrollTop || 0;
        if (y > 300) btn.classList.add("show");  // <-- khớp CSS
        else btn.classList.remove("show");
    }

    window.addEventListener("scroll", update, { passive: true });
    window.addEventListener("load", update);

    btn.addEventListener("click", function () {
        window.scrollTo({ top: 0, behavior: "smooth" });
    });
})();