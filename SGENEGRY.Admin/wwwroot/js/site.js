// Hiển thị ảnh được chọn từ input file lên thẻ img
// (Thẻ input có thuộc tính data-img-preview trỏ đến id của thẻ img dung để hiển thị ảnh)
function previewImage(input) {
    if (!input.files || !input.files[0]) return;

    const previewId = input.dataset.imgPreview; // lấy data-img-preview
    if (!previewId) return;

    const img = document.getElementById(previewId);
    if (!img) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        img.src = e.target.result;
    };
    reader.readAsDataURL(input.files[0]);
}
function normalizeMoneyInputs(form) {
    if (!form) return;

    const moneyInputs = form.querySelectorAll("input.money-input");
    moneyInputs.forEach(input => {
        const raw = (input.value || "").trim();
        if (!raw) {
            input.value = "";
            return;
        }

        // Giữ lại chữ số, loại bỏ dấu chấm/phẩy/khoảng trắng/ký tự tiền tệ
        const digits = raw.replace(/[^\d]/g, "");
        input.value = digits;
    });
}

function toIsoDateString(value) {
    if (!value) return value;
    const raw = value.trim();

    // Đã là ISO yyyy-MM-dd thì giữ nguyên
    if (/^\d{4}-\d{2}-\d{2}$/.test(raw)) {
        return raw;
    }

    // dd/MM/yyyy -> yyyy-MM-dd
    const match = raw.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/);
    if (!match) return raw;

    const dd = match[1].padStart(2, "0");
    const mm = match[2].padStart(2, "0");
    const yyyy = match[3];

    return `${yyyy}-${mm}-${dd}`;
}

function prepareDateInputsForSubmit(form) {
    const dateInputs = form.querySelectorAll("input.date-picker");
    const backups = [];

    dateInputs.forEach(input => {
        const original = input.value || "";
        backups.push({ input, original });

        if (original.trim() !== "") {
            input.value = toIsoDateString(original);
        }
    });

    // Hàm restore để trả lại định dạng hiển thị ban đầu
    return function restore() {
        backups.forEach(x => {
            x.input.value = x.original;
        });
    };
}

// Tìm kiếm phân trang bằng AJAX
function paginationSearch(event, form, page) {
    if (event) event.preventDefault();
    if (!form) return;

    normalizeMoneyInputs(form);

    // Convert tạm ngày trước khi tạo FormData
    const restoreDateInputs = prepareDateInputsForSubmit(form);

    const url = form.action;
    const method = (form.method || "GET").toUpperCase();
    const targetId = form.dataset.target;

    const formData = new FormData(form);
    formData.append("page", page);

    // Trả lại giá trị hiển thị dd/MM/yyyy cho UI
    restoreDateInputs();

    let fetchUrl = url;
    if (method === "GET") {
        const params = new URLSearchParams(formData).toString();
        fetchUrl = url + "?" + params;
    }

    let targetEl = null;
    if (targetId) {
        targetEl = document.getElementById(targetId);
        if (targetEl) {
            targetEl.innerHTML = `
                <div class="text-center py-4">
                    <span>Đang tải dữ liệu...</span>
                </div>`;
        }
    }

    fetch(fetchUrl, {
        method: method,
        body: method === "GET" ? null : formData
    })
    .then(res => res.text())
    .then(html => {
        if (targetEl) {
            targetEl.innerHTML = html;
        }
    })
    .catch(() => {
        if (targetEl) {
            targetEl.innerHTML = `
                <div class="text-danger">
                    Không tải được dữ liệu
                </div>`;
        }
    });
}

// Mở modal và load nội dung từ link vào modal
(function () {
    //dialogModal là id của modal dùng chung đuơc định nghĩa trong _Layout.cshtml
    const modalEl = document.getElementById("dialogModal");
    if (!modalEl) return;

    const modalContent = modalEl.querySelector(".modal-content");

    // Clear nội dung khi modal đóng
    modalEl.addEventListener('hidden.bs.modal', function () {
        modalContent.innerHTML = '';
    });

    window.openModal = function (event, link) {
        if (!link) return;
        if (event) event.preventDefault();

        const url = link.getAttribute("href");

        // Hiển thị loading
        modalContent.innerHTML = `
            <div class="modal-body text-center py-5">
                <span>Đang tải dữ liệu...</span>
            </div>`;

// Khởi tạo modal (chỉ tạo 1 lần)
        let modal = bootstrap.Modal.getInstance(modalEl);
        if (!modal) {
            modal = new bootstrap.Modal(modalEl, {
                backdrop: 'static',
                keyboard: false
            });
        }

        modal.show();

        // Load nội dung
        fetch(url)
            .then(res => res.text())
            .then(html => {
                modalContent.innerHTML = html;
            })
            .catch(() => {
                modalContent.innerHTML = `
                    <div class="modal-body text-danger">
                        Không tải được dữ liệu
                    </div>`;
            });
    };
})();


