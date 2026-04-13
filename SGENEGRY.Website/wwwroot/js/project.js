(function () {
    const cards = document.querySelectorAll(".project-card");
    if (!("IntersectionObserver" in window) || cards.length === 0) return;

    cards.forEach(c => c.classList.add("reveal"));

    const io = new IntersectionObserver((entries) => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.classList.add("is-visible");
                io.unobserve(e.target);
            }
        });
    }, { threshold: 0.12 });

    cards.forEach(c => io.observe(c));
})();