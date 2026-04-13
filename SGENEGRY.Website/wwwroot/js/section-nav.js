(function () {
    const nav = document.querySelector(".js-section-nav");
    if (!nav) return;

    const links = Array.from(nav.querySelectorAll(".section-nav__link"));
    const sections = Array.from(document.querySelectorAll("[data-spy-section]"));
    if (links.length === 0 || sections.length === 0) return;

    const navHeight = nav.getBoundingClientRect().height;

    function scrollToSection(id) {
        const el = document.getElementById(id);
        if (!el) return;

        const y = window.scrollY + el.getBoundingClientRect().top - navHeight + 1;
        window.scrollTo({ top: y, behavior: "smooth" });
    }

    links.forEach(a => {
        a.addEventListener("click", (e) => {
            const target = a.getAttribute("data-target");
            if (!target) return;
            e.preventDefault();
            scrollToSection(target);
            history.replaceState(null, "", `#${target}`);
        });
    });

    function setActive(id) {
        links.forEach(l => l.classList.toggle("is-active", l.getAttribute("data-target") === id));
    }

    const io = new IntersectionObserver((entries) => {
        const visible = entries
            .filter(e => e.isIntersecting)
            .sort((a, b) => (b.intersectionRatio - a.intersectionRatio))[0];

        if (!visible) return;
        const id = visible.target.getAttribute("id");
        if (id) setActive(id);
    }, {
        threshold: [0.18, 0.3, 0.42, 0.55],
        rootMargin: `-${navHeight + 6}px 0px -60% 0px`
    });

    sections.forEach(s => io.observe(s));

    const hash = (location.hash || "").replace("#", "");
    if (hash) setTimeout(() => scrollToSection(hash), 80);
})();