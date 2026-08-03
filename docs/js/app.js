// ----------------------------------------------------
// SAYFALAR ARASINDA GEÇİŞ
// ----------------------------------------------------

const menuLinks = document.querySelectorAll(".menu-link");
const pageSections = document.querySelectorAll(".page-section");
const pageTitle = document.getElementById("pageTitle");

const pageTitles = {
    dashboard: "Genel Bakış",
    products: "Ürünler",
    stockMovements: "Stok Hareketleri",
    orders: "Üretim Emirleri",
    scrapTracking: "Fire Takibi",
    alerts: "Akıllı Uyarılar"
};

function openPage(pageId) {
    pageSections.forEach(function (section) {
        section.classList.remove("active");
    });

    menuLinks.forEach(function (link) {
        link.classList.remove("active");
    });

    const selectedPage = document.getElementById(pageId);

    const selectedMenuLink = document.querySelector(
        '.menu-link[data-page="' + pageId + '"]'
    );

    if (selectedPage) {
        selectedPage.classList.add("active");
    }

    if (selectedMenuLink) {
        selectedMenuLink.classList.add("active");
    }

    pageTitle.textContent = pageTitles[pageId] || "CeramiQ";

    window.scrollTo({
        top: 0,
        behavior: "smooth"
    });
}

menuLinks.forEach(function (link) {
    link.addEventListener("click", function () {
        openPage(link.dataset.page);
    });
});


// ----------------------------------------------------
// SAYFA İÇİNDEKİ YÖNLENDİRME BUTONLARI
// ----------------------------------------------------

const targetButtons = document.querySelectorAll("[data-target]");

targetButtons.forEach(function (button) {
    button.addEventListener("click", function () {
        openPage(button.dataset.target);
    });
});


// ----------------------------------------------------
// AI AÇIKLAMALARI
// ----------------------------------------------------

const explanations = {
    stock: {
        title: "Kritik Stok Analizi",
        description:
            "Antrasit Zemin Seramiği stok miktarı 5 kutudur. " +
            "Minimum stok seviyesi 50 kutu olduğu için ürün kritik " +
            "stok durumuna düşmüştür. Mevcut stok, belirlenen güvenli " +
            "seviyenin 45 kutu altındadır.",
        recommendation:
            "İlgili ürün için üretim planı oluşturun veya satın alma " +
            "sürecini başlatın. Yaklaşan müşteri siparişlerini ve ürünün " +
            "son dönem tüketim hızını da kontrol edin."
    },

    scrap: {
        title: "Yüksek Fire Analizi",
        description:
            "URE-2026-001 numaralı üretim emrinin fire oranı %33,33'tür. " +
            "Bu değer, sistemde belirlenen %10 kritik fire sınırının " +
            "üzerindedir. Makine ayarları, hammadde kalitesi veya üretim " +
            "sürecindeki uygulama hataları bu duruma neden olabilir.",
        recommendation:
            "Hat 1 üzerindeki makine ayarlarını ve kullanılan hammadde " +
            "partisini kontrol edin. Benzer üretim emirlerinin geçmiş " +
            "fire oranlarıyla karşılaştırma yapın."
    },

    delay: {
        title: "Geciken Üretim Analizi",
        description:
            "İki üretim emrinin teslim tarihi geçmiş olmasına rağmen " +
            "üretim süreçleri tamamlanmamıştır. Üretim hattı yoğunluğu, " +
            "malzeme eksikliği veya planlama problemi gecikmeye neden olabilir.",
        recommendation:
            "Geciken emirlerin önceliklerini kontrol edin. Üretim hattı " +
            "kapasitesini ve gerekli malzeme durumunu inceleyerek güncel " +
            "bir tamamlanma planı oluşturun."
    }
};

const explanationModal =
    document.getElementById("explanationModal");

const modalTitle =
    document.getElementById("modalTitle");

const modalDescription =
    document.getElementById("modalDescription");

const modalRecommendation =
    document.getElementById("modalRecommendation");

const closeModalButton =
    document.getElementById("closeModal");

const aiButtons =
    document.querySelectorAll(".ai-button");

function closeModal() {
    explanationModal.classList.remove("show");
}

aiButtons.forEach(function (button) {
    button.addEventListener("click", function () {
        const explanationType =
            button.dataset.explanation;

        const selectedExplanation =
            explanations[explanationType];

        if (!selectedExplanation) {
            return;
        }

        modalTitle.textContent =
            selectedExplanation.title;

        modalDescription.textContent =
            selectedExplanation.description;

        modalRecommendation.textContent =
            selectedExplanation.recommendation;

        explanationModal.classList.add("show");
    });
});

closeModalButton.addEventListener("click", closeModal);

explanationModal.addEventListener("click", function (event) {
    if (event.target === explanationModal) {
        closeModal();
    }
});

document.addEventListener("keydown", function (event) {
    if (
        event.key === "Escape" &&
        explanationModal.classList.contains("show")
    ) {
        closeModal();
    }
});


// ----------------------------------------------------
// SAĞ ALT BİLDİRİM
// ----------------------------------------------------

const toast =
    document.getElementById("toast");

let toastTimer;

function showToast(message) {
    clearTimeout(toastTimer);

    toast.textContent = message;
    toast.classList.add("show");

    toastTimer = setTimeout(function () {
        toast.classList.remove("show");
    }, 3000);
}


// ----------------------------------------------------
// YENİ ÜRÜN VE ÜRETİM EMRİ EKLEME
// ----------------------------------------------------

const demoButtons =
    document.querySelectorAll(".demo-action");

demoButtons.forEach(function (button) {
    button.addEventListener("click", function () {
        const page = button.closest(".page-section");

        if (!page) {
            return;
        }

        if (page.id === "products") {
            showNewProductForm();
        }

        if (page.id === "orders") {
            showNewOrderForm();
        }

        if (page.id === "stockMovements") {
            showNewStockMovementForm();
        }
        if (page.id === "scrapTracking") {
            showNewScrapForm();
        }
        const recordId = Date.now();
    });
});


// ----------------------------------------------------
// YENİ ÜRÜN FORMU
// ----------------------------------------------------

function showNewProductForm() {
    actionModalTitle.textContent = "Yeni Ürün Ekle";

    actionModalBody.innerHTML = `
        <form id="newProductForm" class="demo-form">
            <label>
                Ürün Kodu

                <input id="newProductCode"
                       type="text"
                       placeholder="Örnek: SRM-004"
                       required>
            </label>

            <label>
                Ürün Adı

                <input id="newProductName"
                       type="text"
                       placeholder="Ürün adını yazın"
                       required>
            </label>

            <label>
                Kategori

                <input id="newProductCategory"
                       type="text"
                       placeholder="Örnek: Zemin Seramiği"
                       required>
            </label>

            <div class="form-row">
                <label>
                    Stok Miktarı

                    <input id="newProductStock"
                           type="number"
                           min="0"
                           value="0"
                           required>
                </label>

                <label>
                    Minimum Stok

                    <input id="newProductMinimum"
                           type="number"
                           min="0"
                           value="0"
                           required>
                </label>
            </div>

            <button type="submit"
                    class="primary-button">
                Ürünü Kaydet
            </button>
        </form>
    `;

    openActionModal();

    const newProductForm =
        document.getElementById("newProductForm");

    newProductForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const productCode =
                document
                    .getElementById("newProductCode")
                    .value
                    .trim();

            const productName =
                document
                    .getElementById("newProductName")
                    .value
                    .trim();

            const category =
                document
                    .getElementById("newProductCategory")
                    .value
                    .trim();

            const stock =
                Number(
                    document.getElementById(
                        "newProductStock"
                    ).value
                );

            const minimum =
                Number(
                    document.getElementById(
                        "newProductMinimum"
                    ).value
                );

            const productsTableBody =
                document.querySelector(
                    "#products table tbody"
                );

            if (!productsTableBody) {
                showToast("Ürün tablosu bulunamadı.");
                return;
            }

            let statusClass = "normal";
            let statusText = "Normal";
            let stockClass = "";

            if (stock <= minimum) {
                statusClass = "critical";
                statusText = "Kritik";
                stockClass = "danger-text";
            }

            const newRow =
                document.createElement("tr");

           newRow.innerHTML = `
    <td>${orderNumber}</td>
    <td>${product}</td>
    <td>${productionLine}</td>

    <td>
        ${Number(produced)
            .toLocaleString("tr-TR")} Kutu
    </td>

    <td>
        ${Number(scrap)
            .toLocaleString("tr-TR")} Kutu
    </td>

    <td>%${formattedRate}</td>

    <td>
        <div class="scrap-row-actions">
            <span class="status ${statusClass}">
                ${statusText}
            </span>

            <button type="button"
                    class="delete-scrap-button"
                    data-record-id="${recordId}">
                Sil
            </button>
        </div>
    </td>
`;

            productsTableBody.appendChild(newRow);

            closeActionModal();

            showToast(
                productName +
                " isimli ürün başarıyla eklendi."
            );
        }
    );
}


// ----------------------------------------------------
// YENİ ÜRETİM EMRİ FORMU
// ----------------------------------------------------

function showNewOrderForm() {
    actionModalTitle.textContent =
        "Yeni Üretim Emri Ekle";

    actionModalBody.innerHTML = `
        <form id="newOrderForm" class="demo-form">
            <label>
                Emir Numarası

                <input id="newOrderNumber"
                       type="text"
                       placeholder="Örnek: URE-2026-004"
                       required>
            </label>

            <label>
                Ürün

                <input id="newOrderProduct"
                       type="text"
                       placeholder="Ürün adını yazın"
                       required>
            </label>

            <label>
                Üretim Hattı

                <input id="newOrderLine"
                       type="text"
                       placeholder="Örnek: Hat 2"
                       required>
            </label>

            <div class="form-row">
                <label>
                    Planlanan Miktar

                    <input id="newOrderPlanned"
                           type="number"
                           min="1"
                           required>
                </label>

                <label>
                    Üretilen Miktar

                    <input id="newOrderProduced"
                           type="number"
                           min="0"
                           value="0"
                           required>
                </label>
            </div>

            <label>
                Fire Miktarı

                <input id="newOrderScrap"
                       type="number"
                       min="0"
                       value="0"
                       required>
            </label>

            <label>
                Teslim Tarihi

                <input id="newOrderDueDate"
                       type="date"
                       required>
            </label>

            <label>
                Durum

                <select id="newOrderStatus">
                    <option value="Planlandı">
                        Planlandı
                    </option>

                    <option value="Devam Ediyor">
                        Devam Ediyor
                    </option>

                    <option value="Gecikti">
                        Gecikti
                    </option>

                    <option value="Tamamlandı">
                        Tamamlandı
                    </option>
                </select>
            </label>

            <button type="submit"
                    class="primary-button">
                Üretim Emrini Kaydet
            </button>
        </form>
    `;

    openActionModal();

    const newOrderForm =
        document.getElementById("newOrderForm");

    newOrderForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const orderNumber =
                document
                    .getElementById("scrapOrderNumber")
                    .value
                    .trim()
                    .toUpperCase();

            const product =
                document
                    .getElementById("scrapProduct")
                    .value
                    .trim()
                    .replace(/\s+/g, " ");


            const orderNumberPattern =
                /^URE-\d{4}-\d{3}$/;

            if (!orderNumberPattern.test(orderNumber)) {
                showToast(
                    "Emir numarası URE-2026-001 biçiminde olmalıdır."
                );

                return;
            }

            if (product.length < 3) {
                showToast(
                    "Lütfen geçerli bir ürün adı girin."
                );

                return;
            }

            const productionLine =
                document
                    .getElementById("newOrderLine")
                    .value
                    .trim();

            const planned =
                document.getElementById(
                    "newOrderPlanned"
                ).value;

            const produced =
                document.getElementById(
                    "newOrderProduced"
                ).value;

            const scrap =
                document.getElementById(
                    "newOrderScrap"
                ).value;

            const dueDateValue =
                document.getElementById(
                    "newOrderDueDate"
                ).value;

            const status =
                document.getElementById(
                    "newOrderStatus"
                ).value;

            const dateParts =
                dueDateValue.split("-");

            const formattedDate =
                dateParts[2] +
                "." +
                dateParts[1] +
                "." +
                dateParts[0];

            let statusClass = "normal";

            if (status === "Gecikti") {
                statusClass = "critical";
            }

            if (status === "Devam Ediyor") {
                statusClass = "warning";
            }

            const ordersTableBody =
                document.querySelector(
                    "#orders table tbody"
                );

            if (!ordersTableBody) {
                showToast(
                    "Üretim emirleri tablosu bulunamadı."
                );

                return;
            }

            const newRow =
                document.createElement("tr");

            newRow.innerHTML = `
                <td>${orderNumber}</td>
                <td>${product}</td>
                <td>${productionLine}</td>
                <td>${planned}</td>
                <td>${produced}</td>
                <td>${scrap}</td>
                <td>${formattedDate}</td>
                <td>
                    <span class="status ${statusClass}">
                        ${status}
                    </span>
                </td>
                <td>
                    <div class="table-actions">
                        <button class="action-button detail-action">
                            Detay
                        </button>

                        <button class="action-button edit-action">
                            Güncelle
                        </button>
                    </div>
                </td>
            `;

            ordersTableBody.appendChild(newRow);

            closeActionModal();

            showToast(
                orderNumber +
                " numaralı üretim emri başarıyla eklendi."
            );
        }
    );
}
// ----------------------------------------------------
// GERÇEK ÇALIŞAN TABLO İŞLEMLERİ
// ----------------------------------------------------

const actionModal = document.createElement("div");

actionModal.className = "modal";

actionModal.innerHTML = `
    <div class="modal-content action-modal-content">
        <button class="close-button action-modal-close"
                aria-label="Pencereyi kapat">
            ×
        </button>

        <span class="eyebrow">CERAMIQ KAYIT İŞLEMİ</span>

        <h3 id="actionModalTitle">Kayıt İşlemi</h3>

        <div id="actionModalBody"></div>
    </div>
`;

document.body.appendChild(actionModal);

const actionModalTitle =
    document.getElementById("actionModalTitle");

const actionModalBody =
    document.getElementById("actionModalBody");

const actionModalClose =
    document.querySelector(".action-modal-close");
function openActionModal() {
    actionModal.classList.add("show");
}
function closeActionModal() {
    actionModal.classList.remove("show");
}

actionModalClose.addEventListener("click", closeActionModal);

actionModal.addEventListener("click", function (event) {
    if (event.target === actionModal) {
        closeActionModal();
    }
});


// ----------------------------------------------------
// ÜRÜN DETAY
// ----------------------------------------------------

function showProductDetail(row) {
    const cells = row.querySelectorAll("td");

    actionModalTitle.textContent = "Ürün Detayları";

    actionModalBody.innerHTML = `
        <div class="detail-list">
            <div>
                <span>Ürün Kodu</span>
                <strong>${cells[0].textContent.trim()}</strong>
            </div>

            <div>
                <span>Ürün Adı</span>
                <strong>${cells[1].textContent.trim()}</strong>
            </div>

            <div>
                <span>Kategori</span>
                <strong>${cells[2].textContent.trim()}</strong>
            </div>

            <div>
                <span>Stok Miktarı</span>
                <strong>${cells[3].textContent.trim()}</strong>
            </div>

            <div>
                <span>Minimum Stok</span>
                <strong>${cells[4].textContent.trim()}</strong>
            </div>

            <div>
                <span>Durum</span>
                <strong>${cells[5].textContent.trim()}</strong>
            </div>
        </div>
    `;

    actionModal.classList.add("show");
}


// ----------------------------------------------------
// ÜRÜN DÜZENLEME
// ----------------------------------------------------

function showProductEdit(row) {
    const cells = row.querySelectorAll("td");

    const productCode = cells[0].textContent.trim();
    const productName = cells[1].textContent.trim();
    const category = cells[2].textContent.trim();
    const stock = cells[3].textContent.replace("Kutu", "").trim();
    const minimum = cells[4].textContent.replace("Kutu", "").trim();

    actionModalTitle.textContent = "Ürünü Düzenle";

    actionModalBody.innerHTML = `
        <form id="productEditForm" class="demo-form">
            <label>
                Ürün Kodu
                <input id="editProductCode"
                       type="text"
                       value="${productCode}"
                       required>
            </label>

            <label>
                Ürün Adı
                <input id="editProductName"
                       type="text"
                       value="${productName}"
                       required>
            </label>

            <label>
                Kategori
                <input id="editProductCategory"
                       type="text"
                       value="${category}"
                       required>
            </label>

            <div class="form-row">
                <label>
                    Stok Miktarı
                    <input id="editProductStock"
                           type="number"
                           min="0"
                           value="${stock}"
                           required>
                </label>

                <label>
                    Minimum Stok
                    <input id="editProductMinimum"
                           type="number"
                           min="0"
                           value="${minimum}"
                           required>
                </label>
            </div>

            <button type="submit" class="primary-button">
                Değişiklikleri Kaydet
            </button>
        </form>
    `;

    actionModal.classList.add("show");

    const productEditForm =
        document.getElementById("productEditForm");

    productEditForm.addEventListener("submit", function (event) {
        event.preventDefault();

        const newCode =
            document.getElementById("editProductCode").value.trim();

        const newName =
            document.getElementById("editProductName").value.trim();

        const newCategory =
            document.getElementById("editProductCategory").value.trim();

        const newStock =
            Number(document.getElementById("editProductStock").value);

        const newMinimum =
            Number(document.getElementById("editProductMinimum").value);

        cells[0].textContent = newCode;
        cells[1].textContent = newName;
        cells[2].textContent = newCategory;
        cells[3].textContent = newStock + " Kutu";
        cells[4].textContent = newMinimum + " Kutu";

        if (newStock <= newMinimum) {
            cells[3].classList.add("danger-text");

            cells[5].innerHTML = `
                <span class="status critical">Kritik</span>
            `;
        } else {
            cells[3].classList.remove("danger-text");

            cells[5].innerHTML = `
                <span class="status normal">Normal</span>
            `;
        }

        closeActionModal();

        showToast("Ürün bilgileri başarıyla güncellendi.");
    });
}


// ----------------------------------------------------
// ÜRÜN SİLME
// ----------------------------------------------------

function deleteProduct(row) {
    const productName =
        row.querySelectorAll("td")[1].textContent.trim();

    const deleteApproved = confirm(
        productName +
        " isimli ürünü silmek istediğinize emin misiniz?"
    );

    if (!deleteApproved) {
        return;
    }

    row.remove();

    showToast("Ürün tablodan başarıyla silindi.");
}


// ----------------------------------------------------
// ÜRETİM EMRİ DETAY
// ----------------------------------------------------

function showOrderDetail(row) {
    const cells = row.querySelectorAll("td");

    actionModalTitle.textContent = "Üretim Emri Detayları";

    actionModalBody.innerHTML = `
        <div class="detail-list">
            <div>
                <span>Emir Numarası</span>
                <strong>${cells[0].textContent.trim()}</strong>
            </div>

            <div>
                <span>Ürün</span>
                <strong>${cells[1].textContent.trim()}</strong>
            </div>

            <div>
                <span>Üretim Hattı</span>
                <strong>${cells[2].textContent.trim()}</strong>
            </div>

            <div>
                <span>Planlanan</span>
                <strong>${cells[3].textContent.trim()}</strong>
            </div>

            <div>
                <span>Üretilen</span>
                <strong>${cells[4].textContent.trim()}</strong>
            </div>

            <div>
                <span>Fire</span>
                <strong>${cells[5].textContent.trim()}</strong>
            </div>

            <div>
                <span>Teslim Tarihi</span>
                <strong>${cells[6].textContent.trim()}</strong>
            </div>

            <div>
                <span>Durum</span>
                <strong>${cells[7].textContent.trim()}</strong>
            </div>
        </div>
    `;

    actionModal.classList.add("show");
}


// ----------------------------------------------------
// ÜRETİM EMRİ GÜNCELLEME
// ----------------------------------------------------

function showOrderEdit(row) {
    const cells = row.querySelectorAll("td");

    actionModalTitle.textContent = "Üretim Emrini Güncelle";

    actionModalBody.innerHTML = `
        <form id="orderEditForm" class="demo-form">
            <label>
                Emir Numarası
                <input id="editOrderNumber"
                       type="text"
                       value="${cells[0].textContent.trim()}"
                       required>
            </label>

            <label>
                Ürün
                <input id="editOrderProduct"
                       type="text"
                       value="${cells[1].textContent.trim()}"
                       required>
            </label>

            <label>
                Üretim Hattı
                <input id="editOrderLine"
                       type="text"
                       value="${cells[2].textContent.trim()}"
                       required>
            </label>

            <div class="form-row">
                <label>
                    Planlanan
                    <input id="editPlanned"
                           type="number"
                           min="0"
                           value="${cells[3].textContent.trim()}"
                           required>
                </label>

                <label>
                    Üretilen
                    <input id="editProduced"
                           type="number"
                           min="0"
                           value="${cells[4].textContent.trim()}"
                           required>
                </label>
            </div>

            <label>
                Fire
                <input id="editScrap"
                       type="number"
                       min="0"
                       value="${cells[5].textContent.trim()}"
                       required>
            </label>

            <label>
                Teslim Tarihi
                <input id="editDueDate"
                       type="text"
                       value="${cells[6].textContent.trim()}"
                       required>
            </label>

            <label>
                Durum
                <select id="editOrderStatus">
                    <option>Planlandı</option>
                    <option>Devam Ediyor</option>
                    <option>Gecikti</option>
                    <option>Tamamlandı</option>
                </select>
            </label>

            <button type="submit" class="primary-button">
                Üretim Emrini Güncelle
            </button>
        </form>
    `;

    document.getElementById("editOrderStatus").value =
        cells[7].textContent.trim();

    actionModal.classList.add("show");

    const orderEditForm =
        document.getElementById("orderEditForm");

    orderEditForm.addEventListener("submit", function (event) {
        event.preventDefault();

        cells[0].textContent =
            document.getElementById("editOrderNumber").value.trim();

        cells[1].textContent =
            document.getElementById("editOrderProduct").value.trim();

        cells[2].textContent =
            document.getElementById("editOrderLine").value.trim();

        cells[3].textContent =
            document.getElementById("editPlanned").value;

        cells[4].textContent =
            document.getElementById("editProduced").value;

        cells[5].textContent =
            document.getElementById("editScrap").value;

        cells[6].textContent =
            document.getElementById("editDueDate").value.trim();

        const newStatus =
            document.getElementById("editOrderStatus").value;

        let statusClass = "normal";

        if (newStatus === "Gecikti") {
            statusClass = "critical";
        } else if (newStatus === "Devam Ediyor") {
            statusClass = "warning";
        }

        cells[7].innerHTML = `
            <span class="status ${statusClass}">
                ${newStatus}
            </span>
        `;

        closeActionModal();

        showToast("Üretim emri başarıyla güncellendi.");
    });
}
// ----------------------------------------------------
// TABLO BUTONLARINI İŞLEMLERE BAĞLAMA
// ----------------------------------------------------

document.addEventListener("click", function (event) {
    const button =
        event.target.closest(".action-button");

    if (!button) {
        return;
    }

    const row = button.closest("tr");
    const page = button.closest(".page-section");

    if (!row || !page) {
        return;
    }

    if (page.id === "products") {
        if (
            button.classList.contains("detail-action")
        ) {
            showProductDetail(row);
        }

        if (
            button.classList.contains("edit-action")
        ) {
            showProductEdit(row);
        }

        if (
            button.classList.contains("delete-action")
        ) {
            deleteProduct(row);
        }
    }

    if (page.id === "orders") {
        if (
            button.classList.contains("detail-action")
        ) {
            showOrderDetail(row);
        }

        if (
            button.classList.contains("edit-action")
        ) {
            showOrderEdit(row);
        }
    }
});

// ----------------------------------------------------
// YENİ STOK HAREKETİ EKLEME
// ----------------------------------------------------

function showNewStockMovementForm() {
    actionModalTitle.textContent = "Yeni Stok Hareketi";

    actionModalBody.innerHTML = `
        <form id="newStockMovementForm"
              class="demo-form">

            <label>
                Ürün

                <input id="movementProduct"
                       type="text"
                       placeholder="Örnek: Mat Beyaz Seramik"
                       required>
            </label>

            <label>
                Hareket Türü

                <select id="movementType">
                    <option value="Giriş">
                        Stok Girişi
                    </option>

                    <option value="Çıkış">
                        Stok Çıkışı
                    </option>
                </select>
            </label>

            <label>
                Miktar

                <input id="movementQuantity"
                       type="number"
                       min="1"
                       placeholder="Örnek: 50"
                       required>
            </label>

            <label>
                Tarih

                <input id="movementDate"
                       type="datetime-local"
                       required>
            </label>

            <label>
                Açıklama

                <input id="movementDescription"
                       type="text"
                       placeholder="Örnek: Depoya yeni ürün girişi">
            </label>

            <button type="submit"
                    class="primary-button">
                Hareketi Kaydet
            </button>
        </form>
    `;

    const dateInput =
        document.getElementById("movementDate");

    const now = new Date();

    const localDate =
        new Date(
            now.getTime() -
            now.getTimezoneOffset() * 60000
        );

    dateInput.value =
        localDate.toISOString().slice(0, 16);

    openActionModal();

    const movementForm =
        document.getElementById(
            "newStockMovementForm"
        );

    movementForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const product =
                document
                    .getElementById("movementProduct")
                    .value
                    .trim();

            const movementType =
                document.getElementById(
                    "movementType"
                ).value;

            const quantity =
                document.getElementById(
                    "movementQuantity"
                ).value;

            const dateValue =
                document.getElementById(
                    "movementDate"
                ).value;

            const description =
                document
                    .getElementById(
                        "movementDescription"
                    )
                    .value
                    .trim();

            const movementTableBody =
                document.querySelector(
                    "#stockMovements table tbody"
                );

            if (!movementTableBody) {
                showToast(
                    "Stok hareketleri tablosu bulunamadı."
                );

                return;
            }

            const movementDate =
                new Date(dateValue);

            const formattedDate =
                movementDate.toLocaleString(
                    "tr-TR",
                    {
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit"
                    }
                );

            let movementClass = "normal";
            let quantitySign = "+";

            if (movementType === "Çıkış") {
                movementClass = "critical";
                quantitySign = "-";
            }

            const newRow =
                document.createElement("tr");

            newRow.innerHTML = `
                <td>${formattedDate}</td>

                <td>${product}</td>

                <td>
                    <span class="status ${movementClass}">
                        ${movementType}
                    </span>
                </td>

                <td>
                    <strong>
                        ${quantitySign}${quantity} Kutu
                    </strong>
                </td>

                <td>
                    ${description || "Açıklama eklenmedi"}
                </td>
            `;

            movementTableBody.prepend(newRow);

            closeActionModal();

            showToast(
                product +
                " için stok hareketi başarıyla kaydedildi."
            );
        }
    );
}
// ----------------------------------------------------
// YENİ STOK HAREKETİ EKLEME
// ----------------------------------------------------

function showNewStockMovementForm() {
    actionModalTitle.textContent = "Yeni Stok Hareketi";

    actionModalBody.innerHTML = `
        <form id="newStockMovementForm"
              class="demo-form">

            <label>
                Ürün

                <input id="movementProduct"
                       type="text"
                       placeholder="Örnek: Mat Beyaz Seramik"
                       required>
            </label>

            <label>
                Hareket Türü

                <select id="movementType">
                    <option value="Giriş">
                        Stok Girişi
                    </option>

                    <option value="Çıkış">
                        Stok Çıkışı
                    </option>
                </select>
            </label>

            <label>
                Miktar

                <input id="movementQuantity"
                       type="number"
                       min="1"
                       placeholder="Örnek: 50"
                       required>
            </label>

            <label>
                Tarih

                <input id="movementDate"
                       type="datetime-local"
                       required>
            </label>

            <label>
                Açıklama

                <input id="movementDescription"
                       type="text"
                       placeholder="Örnek: Üretimden depoya giriş">
            </label>

            <button type="submit"
                    class="primary-button">
                Hareketi Kaydet
            </button>
        </form>
    `;

    const dateInput =
        document.getElementById("movementDate");

    const now = new Date();

    const localDate = new Date(
        now.getTime() -
        now.getTimezoneOffset() * 60000
    );

    dateInput.value =
        localDate.toISOString().slice(0, 16);

    openActionModal();

    const movementForm =
        document.getElementById(
            "newStockMovementForm"
        );

    movementForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const product =
                document
                    .getElementById("movementProduct")
                    .value
                    .trim();

            const movementType =
                document.getElementById(
                    "movementType"
                ).value;

            const quantity =
                document.getElementById(
                    "movementQuantity"
                ).value;

            const dateValue =
                document.getElementById(
                    "movementDate"
                ).value;

            const description =
                document
                    .getElementById(
                        "movementDescription"
                    )
                    .value
                    .trim();

            const movementTableBody =
                document.querySelector(
                    "#stockMovements table tbody"
                );

            if (!movementTableBody) {
                showToast(
                    "Stok hareketleri tablosu bulunamadı."
                );

                return;
            }

            const movementDate =
                new Date(dateValue);

            const formattedDate =
                movementDate.toLocaleString(
                    "tr-TR",
                    {
                        day: "2-digit",
                        month: "2-digit",
                        year: "numeric",
                        hour: "2-digit",
                        minute: "2-digit"
                    }
                );

            let movementClass = "normal";
            let quantitySign = "+";

            if (movementType === "Çıkış") {
                movementClass = "critical";
                quantitySign = "-";
            }

            const newRow =
                document.createElement("tr");

            newRow.innerHTML = `
                <td>${formattedDate}</td>
                <td>${product}</td>

                <td>
                    <span class="status ${movementClass}">
                        ${movementType}
                    </span>
                </td>

                <td>
                    <strong>
                        ${quantitySign}${quantity} Kutu
                    </strong>
                </td>

                <td>
                    ${description || "Açıklama eklenmedi"}
                </td>
            `;

            movementTableBody.prepend(newRow);

            closeActionModal();

            showToast(
                product +
                " için stok hareketi başarıyla kaydedildi."
            );
        }
    );
}
// ----------------------------------------------------
// YENİ FİRE KAYDI EKLEME
// ----------------------------------------------------

function showNewScrapForm() {
    actionModalTitle.textContent = "Yeni Fire Kaydı";

    actionModalBody.innerHTML = `
        <form id="newScrapForm"
              class="demo-form">

            <label>
                Emir Numarası

                <input id="scrapOrderNumber"
                       type="text"
                       placeholder="Örnek: URE-2026-004"
                       required>
            </label>

            <label>
                Ürün

                <input id="scrapProduct"
                       type="text"
                       placeholder="Örnek: Mat Gri Seramik"
                       required>
            </label>

            <label>
                Üretim Hattı

                <select id="scrapProductionLine">
                    <option value="Hat 1">Hat 1</option>
                    <option value="Hat 2">Hat 2</option>
                    <option value="Hat 3">Hat 3</option>
                    <option value="Hat 4">Hat 4</option>
                </select>
            </label>

            <label>
                Üretilen Miktar

                <input id="scrapProduced"
                       type="number"
                       min="1"
                       placeholder="Örnek: 1000"
                       required>
            </label>

            <label>
                Fire Miktarı

                <input id="scrapQuantity"
                       type="number"
                       min="0"
                       placeholder="Örnek: 50"
                       required>
            </label>

            <button type="submit"
                    class="primary-button">
                Fire Kaydını Ekle
            </button>
        </form>
    `;

    openActionModal();

    const scrapForm =
        document.getElementById("newScrapForm");

    scrapForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const orderNumber =
    document
        .getElementById("scrapOrderNumber")
        .value
        .trim()
        .toUpperCase();

const product =
    document
        .getElementById("scrapProduct")
        .value
        .trim()
        .replace(/\s+/g, " ");

const orderNumberPattern =
    /^URE-\d{4}-\d{3}$/;

if (!orderNumberPattern.test(orderNumber)) {
    showToast(
        "Emir numarası URE-2026-001 biçiminde olmalıdır."
    );

    return;
}

if (product.length < 3) {
    showToast(
        "Lütfen geçerli bir ürün adı girin."
    );

    return;
}

const savedRecords =
    getSavedScrapRecords();

const isDuplicate =
    savedRecords.some(function (record) {
        return record.orderNumber === orderNumber;
    });

if (isDuplicate) {
    showToast(
        orderNumber +
        " numaralı üretim emri daha önce kaydedilmiş."
    );

    return;
}
            const productionLine =
                document.getElementById(
                    "scrapProductionLine"
                ).value;

            const produced =
                Number(
                    document.getElementById(
                        "scrapProduced"
                    ).value
                );

            const scrap =
                Number(
                    document.getElementById(
                        "scrapQuantity"
                    ).value
                );

            if (scrap > produced) {
                showToast(
                    "Fire miktarı üretilen miktardan büyük olamaz."
                );

                return;
            }

            const scrapRate =
                produced === 0
                    ? 0
                    : (scrap / produced) * 100;
            const recordId = Date.now();
            let statusText = "Normal";
            let statusClass = "normal";

            if (scrapRate >= 10) {
                statusText = "Yüksek Fire";
                statusClass = "critical";
            } else if (scrapRate >= 5) {
                statusText = "Dikkat";
                statusClass = "warning";
            }

            const scrapTableBody =
                document.querySelector(
                    "#scrapTracking table tbody"
                );

            if (!scrapTableBody) {
                showToast(
                    "Fire takip tablosu bulunamadı."
                );

                return;
            }

            const newRow =
                document.createElement("tr");

            newRow.innerHTML = `
                <td>${orderNumber}</td>
                <td>${product}</td>
                <td>${productionLine}</td>
                <td>${produced.toLocaleString("tr-TR")} Kutu</td>
                <td>${scrap.toLocaleString("tr-TR")} Kutu</td>
                <td>%${scrapRate.toFixed(1).replace(".", ",")}</td>

                <td>
    <div class="scrap-row-actions">
        <span class="status ${statusClass}">
            ${statusText}
        </span>

        <div class="scrap-record-buttons">
    <button type="button"
            class="edit-scrap-button"
            data-record-id="${recordId}">
        Düzenle
    </button>

    <button type="button"
            class="delete-scrap-button"
            data-record-id="${recordId}">
        Sil
    </button>
</div>
    </div>
</td>
            `;

            scrapTableBody.prepend(newRow);

            updateScrapSummary(produced, scrap);

            if (scrapRate >= 10) {
                addHighScrapAlert(
                    orderNumber,
                    product,
                    productionLine,
                    scrapRate
                );
            }

            saveScrapRecord({
                id: recordId,
                orderNumber: orderNumber,
                product: product,
                productionLine: productionLine,
                produced: produced,
                scrap: scrap,
                scrapRate: scrapRate,
                statusText: statusText,
                statusClass: statusClass
            });

            closeActionModal();

            showToast(
                orderNumber +
                " numaralı fire kaydı başarıyla eklendi."
            );
        }
    );
}
// Fire özeti için başlangıç değerleri
let totalProducedAmount = 3450;
let totalScrapAmount = 285;

function updateScrapSummary(produced, scrap) {
    totalProducedAmount += produced;
    totalScrapAmount += scrap;

    const averageScrapRate =
        totalProducedAmount === 0
            ? 0
            : (totalScrapAmount / totalProducedAmount) * 100;

    const totalProducedElement =
        document.getElementById("totalProducedValue");

    const totalScrapElement =
        document.getElementById("totalScrapValue");

    const averageRateElement =
        document.getElementById(
            "averageScrapRateValue"
        );

    totalProducedElement.textContent =
        totalProducedAmount.toLocaleString("tr-TR") +
        " Kutu";

    totalScrapElement.textContent =
        totalScrapAmount.toLocaleString("tr-TR") +
        " Kutu";

    averageRateElement.textContent =
        "%" +
        averageScrapRate
            .toFixed(1)
            .replace(".", ",");
}
// ----------------------------------------------------
// YÜKSEK FİRE UYARISI OLUŞTURMA
// ----------------------------------------------------

function addHighScrapAlert(
    orderNumber,
    product,
    productionLine,
    scrapRate,
     showNotification = true
) {
    const alertList =
        document.querySelector("#alerts .alert-list");

    const activeAlertCount =
        document.getElementById("activeAlertCount");

    const highScrapAlertCount =
        document.getElementById(
            "highScrapAlertCount"
        );

    if (
        !alertList ||
        !activeAlertCount ||
        !highScrapAlertCount
    ) {
        showToast(
            "Akıllı uyarı alanı bulunamadı."
        );

        return;
    }

    const formattedRate =
        scrapRate
            .toFixed(2)
            .replace(".", ",");

    const newAlert =
        document.createElement("article");

    newAlert.className =
        "alert-card warning-alert";

    newAlert.innerHTML = `
        <div>
            <span class="alert-type">
                YÜKSEK FİRE
            </span>

            <h4>
                ${orderNumber} üretim emrinde
                yüksek fire
            </h4>

            <p>
                ${product} ürünü için fire oranı
                %${formattedRate} olarak hesaplandı
                ve belirlenen %10 sınırını geçti.
            </p>
        </div>

        <button type="button"
                class="ai-button"
                data-explanation="scrap">
            ✦ AI ile Açıkla
        </button>
    `;

    alertList.prepend(newAlert);

    const currentActiveCount =
        Number.parseInt(
            activeAlertCount.textContent,
            10
        ) || 0;

    activeAlertCount.textContent =
        currentActiveCount +
        1 +
        " aktif uyarı";

    const currentHighScrapCount =
        Number(highScrapAlertCount.textContent) || 0;

    highScrapAlertCount.textContent =
        currentHighScrapCount + 1;
    const newAiButton =
        newAlert.querySelector(".ai-button");

    newAiButton.addEventListener(
        "click",
        function () {
            actionModalTitle.textContent =
                "Yüksek Fire Açıklaması";

            actionModalBody.innerHTML = `
            <div class="ai-explanation">
                <p>
                    <strong>${orderNumber}</strong>
                    numaralı üretim emrinde
                    <strong>${product}</strong>
                    ürünü için fire oranı
                    <strong>%${formattedRate}</strong>
                    olarak hesaplanmıştır.
                </p>

                <p>
                    Bu değer, sistemde belirlenen
                    <strong>%10 kritik fire sınırının</strong>
                    üzerindedir. Makine ayarları, hammadde
                    kalitesi veya üretim sürecindeki uygulama
                    hataları bu duruma neden olabilir.
                </p>

                <h4>Önerilen işlem</h4>

                <p>
                    <strong>${productionLine}</strong>
                    üzerindeki makine ayarlarını ve kullanılan
                    hammadde partisini kontrol edin. Benzer
                    üretim emirlerinin geçmiş fire oranlarıyla
                    karşılaştırma yapın.
                </p>
            </div>
        `;

            openActionModal();
        }
    );

    if (showNotification) {
        showToast(
            orderNumber +
            " için yüksek fire uyarısı oluşturuldu."
        );
    }
}
// ----------------------------------------------------
// FİRE KAYITLARINI TARAYICI HAFIZASINDA SAKLAMA
// ----------------------------------------------------

function getSavedScrapRecords() {
    const savedRecords =
        localStorage.getItem("ceramiqScrapRecords");

    if (!savedRecords) {
        return [];
    }

    try {
        return JSON.parse(savedRecords);
    } catch (error) {
        console.error(
            "Fire kayıtları okunamadı:",
            error
        );

        return [];
    }
}


function saveScrapRecord(record) {
    const records = getSavedScrapRecords();

    records.push(record);

    localStorage.setItem(
        "ceramiqScrapRecords",
        JSON.stringify(records)
    );
}


function loadSavedScrapRecords() {
    const records = getSavedScrapRecords();

    const scrapTableBody =
        document.querySelector(
            "#scrapTracking table tbody"
        );

    if (!scrapTableBody) {
        return;
    }

    records.forEach(function (record, index) {
        if (!record.id) {
            record.id = Date.now() + index;
        }

        const newRow =
            document.createElement("tr");

        const formattedRate =
            Number(record.scrapRate)
                .toFixed(1)
                .replace(".", ",");

        newRow.innerHTML = `
            <td>${record.orderNumber}</td>
            <td>${record.product}</td>
            <td>${record.productionLine}</td>

            <td>
                ${Number(record.produced)
                    .toLocaleString("tr-TR")} Kutu
            </td>

            <td>
                ${Number(record.scrap)
                    .toLocaleString("tr-TR")} Kutu
            </td>

            <td>%${formattedRate}</td>

            <td>
                <div class="scrap-row-actions">
                    <span class="status ${record.statusClass}">
                        ${record.statusText}
                    </span>

                    <div class="scrap-record-buttons">
                        <button type="button"
                                class="edit-scrap-button"
                                data-record-id="${record.id}">
                            Düzenle
                        </button>

                        <button type="button"
                                class="delete-scrap-button"
                                data-record-id="${record.id}">
                            Sil
                        </button>
                    </div>
                </div>
            </td>
        `;

        scrapTableBody.prepend(newRow);

        updateScrapSummary(
            Number(record.produced),
            Number(record.scrap)
        );

        if (Number(record.scrapRate) >= 10) {
            addHighScrapAlert(
                record.orderNumber,
                record.product,
                record.productionLine,
                Number(record.scrapRate),
                false
            );
        }
    });

    localStorage.setItem(
        "ceramiqScrapRecords",
        JSON.stringify(records)
    );
}

window.addEventListener(
    "DOMContentLoaded",
    loadSavedScrapRecords
);
// ----------------------------------------------------
// FİRE KAYDI SİLME
// ----------------------------------------------------

document.addEventListener(
    "click",
    function (event) {
        const deleteButton =
            event.target.closest(
                ".delete-scrap-button"
            );

        if (!deleteButton) {
            return;
        }

        const shouldDelete = window.confirm(
            "Bu fire kaydını silmek istediğinize emin misiniz?"
        );

        if (!shouldDelete) {
            return;
        }

        const recordId =
            deleteButton.dataset.recordId;

        const records =
            getSavedScrapRecords();

        const remainingRecords =
            records.filter(function (record) {
                return String(record.id) !== recordId;
            });

        localStorage.setItem(
            "ceramiqScrapRecords",
            JSON.stringify(remainingRecords)
        );

        window.location.reload();
    }
);


// ----------------------------------------------------
// FİRE KAYDI DÜZENLEME
// ----------------------------------------------------

document.addEventListener(
    "click",
    function (event) {
        const editButton =
            event.target.closest(".edit-scrap-button");

        if (!editButton) {
            return;
        }

        const recordId =
            editButton.dataset.recordId;

        const records =
            getSavedScrapRecords();

        const record =
            records.find(function (item) {
                return String(item.id) === recordId;
            });

        if (!record) {
            showToast("Düzenlenecek kayıt bulunamadı.");
            return;
        }

        showEditScrapForm(record);
    }
);

function showEditScrapForm(record) {
    actionModalTitle.textContent =
        "Fire Kaydını Düzenle";

    actionModalBody.innerHTML = `
        <form id="editScrapForm"
              class="demo-form">

            <label>
                Emir Numarası

                <input id="editScrapOrderNumber"
                       type="text"
                       value="${record.orderNumber}"
                       required>
            </label>

            <label>
                Ürün

                <input id="editScrapProduct"
                       type="text"
                       value="${record.product}"
                       required>
            </label>

            <label>
                Üretim Hattı

                <select id="editScrapProductionLine">
                    <option value="Hat 1">Hat 1</option>
                    <option value="Hat 2">Hat 2</option>
                    <option value="Hat 3">Hat 3</option>
                    <option value="Hat 4">Hat 4</option>
                </select>
            </label>

            <label>
                Üretilen Miktar

                <input id="editScrapProduced"
                       type="number"
                       min="1"
                       value="${record.produced}"
                       required>
            </label>

            <label>
                Fire Miktarı

                <input id="editScrapQuantity"
                       type="number"
                       min="0"
                       value="${record.scrap}"
                       required>
            </label>

            <button type="submit"
                    class="primary-button">
                Değişiklikleri Kaydet
            </button>
        </form>
    `;

    document
        .getElementById("editScrapProductionLine")
        .value = record.productionLine;

    openActionModal();

    const editForm =
        document.getElementById("editScrapForm");

    editForm.addEventListener(
        "submit",
        function (event) {
            event.preventDefault();

            const orderNumber =
                document
                    .getElementById(
                        "editScrapOrderNumber"
                    )
                    .value
                    .trim()
                    .toUpperCase();

            const product =
                document
                    .getElementById(
                        "editScrapProduct"
                    )
                    .value
                    .trim()
                    .replace(/\s+/g, " ");

            const productionLine =
                document.getElementById(
                    "editScrapProductionLine"
                ).value;

            const produced =
                Number(
                    document.getElementById(
                        "editScrapProduced"
                    ).value
                );

            const scrap =
                Number(
                    document.getElementById(
                        "editScrapQuantity"
                    ).value
                );

            const orderNumberPattern =
                /^URE-\d{4}-\d{3}$/;

            if (!orderNumberPattern.test(orderNumber)) {
                showToast(
                    "Emir numarası URE-2026-001 biçiminde olmalıdır."
                );

                return;
            }

            if (product.length < 3) {
                showToast(
                    "Lütfen geçerli bir ürün adı girin."
                );

                return;
            }

            if (scrap > produced) {
                showToast(
                    "Fire miktarı üretilen miktardan büyük olamaz."
                );

                return;
            }

            const records =
                getSavedScrapRecords();

            const isDuplicate =
                records.some(function (item) {
                    return (
                        item.orderNumber === orderNumber &&
                        String(item.id) !== String(record.id)
                    );
                });

            if (isDuplicate) {
                showToast(
                    orderNumber +
                    " numaralı üretim emri zaten bulunuyor."
                );

                return;
            }

            const scrapRate =
                (scrap / produced) * 100;

            let statusText = "Normal";
            let statusClass = "normal";

            if (scrapRate >= 10) {
                statusText = "Yüksek Fire";
                statusClass = "critical";
            } else if (scrapRate >= 5) {
                statusText = "Dikkat";
                statusClass = "warning";
            }

            const updatedRecords =
                records.map(function (item) {
                    if (
                        String(item.id) !==
                        String(record.id)
                    ) {
                        return item;
                    }

                    return {
                        id: item.id,
                        orderNumber: orderNumber,
                        product: product,
                        productionLine: productionLine,
                        produced: produced,
                        scrap: scrap,
                        scrapRate: scrapRate,
                        statusText: statusText,
                        statusClass: statusClass
                    };
                });

            localStorage.setItem(
                "ceramiqScrapRecords",
                JSON.stringify(updatedRecords)
            );

            window.location.reload();
        }
    );
}
// ----------------------------------------------------
// FİRE RAPORUNU CSV OLARAK İNDİRME
// ----------------------------------------------------

const exportScrapRecordsButton =
    document.getElementById("exportScrapRecordsButton");

if (exportScrapRecordsButton) {
    exportScrapRecordsButton.addEventListener(
        "click",
        function () {
            const records = getSavedScrapRecords();

            if (records.length === 0) {
                showToast(
                    "İndirilecek fire kaydı bulunmuyor."
                );

                return;
            }

            const csvRows = [
                [
                    "Emir Numarası",
                    "Ürün",
                    "Üretim Hattı",
                    "Üretilen Miktar",
                    "Fire Miktarı",
                    "Fire Oranı",
                    "Durum"
                ]
            ];

            records.forEach(function (record) {
                csvRows.push([
                    record.orderNumber,
                    record.product,
                    record.productionLine,
                    record.produced,
                    record.scrap,
                    "%" +
                    Number(record.scrapRate)
                        .toFixed(1)
                        .replace(".", ","),
                    record.statusText
                ]);
            });

            const csvContent =
                csvRows
                    .map(function (row) {
                        return row
                            .map(function (value) {
                                return (
                                    '"' +
                                    String(value)
                                        .replace(/"/g, '""') +
                                    '"'
                                );
                            })
                            .join(";");
                    })
                    .join("\n");

            const blob = new Blob(
                ["\uFEFF" + csvContent],
                {
                    type: "text/csv;charset=utf-8;"
                }
            );

            const downloadUrl =
                URL.createObjectURL(blob);

            const downloadLink =
                document.createElement("a");

            const today =
                new Date()
                    .toISOString()
                    .slice(0, 10);

            downloadLink.href = downloadUrl;
            downloadLink.download =
                "ceramiq-fire-raporu-" +
                today +
                ".csv";

            document.body.appendChild(downloadLink);
            downloadLink.click();
            downloadLink.remove();

            URL.revokeObjectURL(downloadUrl);

            showToast(
                "Fire raporu başarıyla indirildi."
            );
        }
    );
}

