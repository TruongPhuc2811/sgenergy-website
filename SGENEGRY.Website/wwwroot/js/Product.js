(function () {
    // Sidebar accordion
    document.querySelectorAll(".js-accordion").forEach((acc) => {
        const btn = acc.querySelector(".js-accordion-btn");
        const body = acc.querySelector(".js-accordion-body");
        if (!btn || !body) return;

        // init
        if (!acc.classList.contains("is-open")) {
            body.style.display = "none";
        }

        btn.addEventListener("click", () => {
            const open = acc.classList.toggle("is-open");
            body.style.display = open ? "block" : "none";
        });
    });

    // Grid / List view toggle
    const grid = document.querySelector(".js-product-grid");
    if (grid) {
        document.querySelectorAll(".view-btn").forEach((btn) => {
            btn.addEventListener("click", () => {
                const view = btn.getAttribute("data-view");
                if (!view) return;

                document.querySelectorAll(".view-btn").forEach((b) => b.classList.remove("is-active"));
                btn.classList.add("is-active");

                grid.setAttribute("data-view", view);
            });
        });
    }
})();