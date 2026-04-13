(function () {
    const nav = document.querySelector(".js-solutions-nav");
    const links = Array.from(document.querySelectorAll(".solutions-nav__link"));
    const sections = Array.from(document.querySelectorAll("[data-spy-section]"));

    if (!nav || links.length === 0 || sections.length === 0) return;

    // Smooth scroll with offset (sticky nav height)
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

    // Scrollspy (IntersectionObserver)
    const io = new IntersectionObserver((entries) => {
        // lấy section đang visible nhiều nhất
        const visible = entries
            .filter(e => e.isIntersecting)
            .sort((a, b) => (b.intersectionRatio - a.intersectionRatio))[0];

        if (!visible) return;

        // section id
        const id = visible.target.getAttribute("id");
        if (id) setActive(id);
    }, {
        root: null,
        threshold: [0.18, 0.28, 0.38, 0.48, 0.6],
        rootMargin: `-${navHeight + 6}px 0px -60% 0px`
    });

    sections.forEach(s => io.observe(s));

    // If loaded with hash
    const hash = (location.hash || "").replace("#", "");
    if (hash) {
        // delay to allow layout paint
        setTimeout(() => scrollToSection(hash), 80);
    }
})();