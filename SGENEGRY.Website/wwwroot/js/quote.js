(function () {
    const form = document.querySelector(".js-quote-form");
    if (!form) return;

    // ---------- Helpers ----------
    function formatVnd(n) {
        const num = Number(n || 0);
        return num.toLocaleString("vi-VN");
    }

    function setRangeOutput(range) {
        const id = range.id;
        const out = document.querySelector(`.js-range-out[data-for="${id}"]`);
        if (!out) return;

        const fmt = range.dataset.format || "number";
        const val = range.value;

        out.textContent = (fmt === "vnd") ? formatVnd(val) : String(val);
    }

    function setCustomValidity(el) {
        // reset
        el.setCustomValidity("");

        const name = el.name || el.id;

        if (el.required && !String(el.value || "").trim()) {
            el.setCustomValidity("Trường này là bắt buộc.");
            return;
        }

        if (name === "email") {
            const v = String(el.value || "").trim();
            // HTML5 already checks type=email, but we give nicer msg
            const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(v);
            if (!ok) el.setCustomValidity("Email không hợp lệ.");
            return;
        }

        if (name === "phone") {
            const v = String(el.value || "").replace(/\s|\.|-/g, "");
            // VN: 0xxxxxxxxx hoặc +84xxxxxxxxx, 9-11 digits (demo rule)
            const ok = /^(\+84|0)\d{8,10}$/.test(v);
            if (!ok) el.setCustomValidity("Số điện thoại không hợp lệ (VD: 090xxxxxxx).");
            return;
        }

        if (name === "fullName") {
            const v = String(el.value || "").trim();
            if (v.length < 2) el.setCustomValidity("Vui lòng nhập họ tên (tối thiểu 2 ký tự).");
            return;
        }
    }

    function setFieldUI(el) {
        const wrap = el.closest(".qinput");
        const err = document.querySelector(`[data-error-for="${el.id}"], [data-error-for="${el.name}"]`);

        setCustomValidity(el);

        const valid = el.checkValidity();
        const msg = valid ? "" : (el.validationMessage || "Dữ liệu không hợp lệ.");

        if (wrap) {
            wrap.classList.toggle("is-invalid", !valid);
            wrap.classList.toggle("is-valid", valid && String(el.value || "").trim().length > 0);
        }
        if (err) err.textContent = msg;
        return valid;
    }

    // ---------- Range init ----------
    const ranges = Array.from(document.querySelectorAll(".js-range"));
    ranges.forEach(r => {
        setRangeOutput(r);
        r.addEventListener("input", () => setRangeOutput(r));
        r.addEventListener("change", () => setRangeOutput(r));
    });

    // ---------- Validate inputs ----------
    const fields = [
        form.querySelector("#fullName"),
        form.querySelector("#email"),
        form.querySelector("#phone"),
        form.querySelector("#city")
    ].filter(Boolean);

    fields.forEach(f => {
        const ev = (f.tagName === "SELECT") ? "change" : "input";
        f.addEventListener(ev, () => setFieldUI(f));
        f.addEventListener("blur", () => setFieldUI(f));
    });

    // ---------- Submit ----------
    form.addEventListener("submit", (e) => {
        e.preventDefault();

        let ok = true;
        fields.forEach(f => {
            if (!setFieldUI(f)) ok = false;
        });

        if (!ok) {
            // focus field invalid đầu tiên
            const firstInvalid = fields.find(f => !f.checkValidity());
            if (firstInvalid) firstInvalid.focus();
            return;
        }

        // Demo: bạn thay đoạn này bằng gọi API backend của bạn
        const payload = {
            roofArea: Number(form.querySelector("#roofArea")?.value || 0),
            monthlyBill: Number(form.querySelector("#monthlyBill")?.value || 0),
            region: String(form.querySelector("#region")?.value || ""),
            dayPercent: Number(form.querySelector("#dayPercent")?.value || 0),
            fullName: String(form.querySelector("#fullName")?.value || ""),
            email: String(form.querySelector("#email")?.value || ""),
            phone: String(form.querySelector("#phone")?.value || ""),
            city: String(form.querySelector("#city")?.value || "")
        };

        console.log("QUOTE REQUEST:", payload);
        form.submit();
    });
})();