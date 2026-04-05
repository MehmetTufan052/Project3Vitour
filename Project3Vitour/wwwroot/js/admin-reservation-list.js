(function () {
    var tableBody = document.getElementById("tableBody");
    if (!tableBody) {
        return;
    }

    var curPage = 1;
    var pageSize = 10;
    var sortState = {};
    var currentResCode = "";
    var exportCurrentRow = null;
    var exportFormat = "excel";

    function qs(id) {
        return document.getElementById(id);
    }

    function escHtml(value) {
        return String(value || "")
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;");
    }

    function formatPrice(val) {
        var n = parseFloat(val) || 0;
        return "\u20ba" + n.toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    function downloadBlob(blob, filename) {
        var url = URL.createObjectURL(blob);
        var link = document.createElement("a");
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        setTimeout(function () {
            if (link.parentNode) {
                link.parentNode.removeChild(link);
            }
            URL.revokeObjectURL(url);
        }, 200);
    }

    function matchPersonCount(cnt, filter) {
        if (filter === "1") return cnt === 1;
        if (filter === "2-5") return cnt >= 2 && cnt <= 5;
        if (filter === "6-10") return cnt >= 6 && cnt <= 10;
        if (filter === "10+") return cnt > 10;
        return true;
    }

    function renderPage() {
        var rows = Array.from(document.querySelectorAll("#tableBody tr"));
        var visibleRows = rows.filter(function (r) { return r.dataset.filteredOut !== "true"; });
        var total = visibleRows.length;
        var pages = Math.ceil(total / pageSize);
        if (pages < 1) pages = 1;
        if (curPage > pages) curPage = pages;

        var start = (curPage - 1) * pageSize;
        var end = start + pageSize;

        rows.forEach(function (row) {
            row.style.display = row.dataset.filteredOut === "true" ? "none" : "";
        });

        visibleRows.forEach(function (row, index) {
            row.style.display = index >= start && index < end ? "" : "none";
        });

        if (qs("prevBtn")) qs("prevBtn").disabled = curPage <= 1;
        if (qs("nextBtn")) qs("nextBtn").disabled = curPage >= pages;
        if (qs("pageInfo")) {
            qs("pageInfo").textContent = total === 0 ? "0 / 0" : (start + 1) + "\u2013" + Math.min(end, total) + " / " + total;
        }

        ["page1Btn", "page2Btn", "page3Btn"].forEach(function (id, idx) {
            var btn = qs(id);
            if (btn) btn.classList.toggle("on", curPage === idx + 1);
        });
    }

    function applyFilters() {
        var searchInput = qs("searchInput");
        var tourFilter = qs("tourFilter");
        var dateFilter = qs("dateFilter");
        var personFilter = qs("personFilter");
        if (!searchInput || !tourFilter || !dateFilter || !personFilter) {
            return;
        }

        var search = searchInput.value.toLowerCase().trim();
        var tour = tourFilter.value;
        var date = dateFilter.value;
        var persons = personFilter.value;
        var visible = 0;

        Array.from(document.querySelectorAll("#tableBody tr")).forEach(function (row) {
            var name = row.dataset.name || "";
            var email = row.dataset.email || "";
            var code = row.dataset.code || "";
            var rowTour = row.dataset.tour || "";
            var rowDate = row.dataset.date || "";
            var cnt = parseInt(row.dataset.persons, 10) || 0;

            var show = (!search || name.includes(search) || email.includes(search) || code.includes(search))
                && (!tour || rowTour === tour)
                && (!date || rowDate === date)
                && (!persons || matchPersonCount(cnt, persons));

            row.dataset.filteredOut = show ? "false" : "true";
            if (show) visible++;
        });

        if (qs("visibleCount")) qs("visibleCount").textContent = visible;
        if (qs("emptyState")) qs("emptyState").style.display = visible === 0 ? "flex" : "none";
        if (qs("clearBtn")) qs("clearBtn").style.display = (search || tour || date || persons) ? "inline-flex" : "none";

        curPage = 1;
        renderPage();
    }

    function clearFilters() {
        if (qs("searchInput")) qs("searchInput").value = "";
        if (qs("tourFilter")) qs("tourFilter").value = "";
        if (qs("dateFilter")) qs("dateFilter").value = "";
        if (qs("personFilter")) qs("personFilter").value = "";
        applyFilters();
    }

    function sortTable(col) {
        sortState[col] = !sortState[col];
        var asc = sortState[col];
        var rows = Array.from(tableBody.querySelectorAll("tr"));

        rows.sort(function (a, b) {
            var va;
            var vb;
            if (col === "name") {
                va = a.dataset.name || "";
                vb = b.dataset.name || "";
                return asc ? va.localeCompare(vb) : vb.localeCompare(va);
            }
            if (col === "code") {
                va = a.dataset.code || "";
                vb = b.dataset.code || "";
                return asc ? va.localeCompare(vb) : vb.localeCompare(va);
            }
            if (col === "date") {
                va = a.dataset.date || "";
                vb = b.dataset.date || "";
                return asc ? va.localeCompare(vb) : vb.localeCompare(va);
            }
            if (col === "price") {
                va = parseFloat(a.dataset.price) || 0;
                vb = parseFloat(b.dataset.price) || 0;
                return asc ? va - vb : vb - va;
            }
            return 0;
        });

        rows.forEach(function (row) {
            tableBody.appendChild(row);
        });

        renderPage();
    }

    function toggleAll(master) {
        document.querySelectorAll(".row-check").forEach(function (cb) {
            cb.checked = !!master.checked;
        });
    }

    function getDetailDataFromRow(row) {
        return {
            id: row.getAttribute("data-detail-id") || "",
            code: row.getAttribute("data-detail-code") || "",
            name: row.getAttribute("data-detail-name") || "",
            phone: row.getAttribute("data-detail-phone") || "",
            email: row.getAttribute("data-detail-email") || "",
            city: row.getAttribute("data-detail-city") || "",
            country: row.getAttribute("data-detail-country") || "",
            tourName: row.getAttribute("data-detail-tour") || "",
            tour: row.getAttribute("data-detail-tour") || "",
            date: row.getAttribute("data-detail-date") || "",
            persons: parseInt(row.getAttribute("data-detail-persons"), 10) || 0,
            price: row.getAttribute("data-detail-price") || "0,00",
            rawPrice: parseFloat(row.getAttribute("data-detail-rawprice")) || 0,
            status: row.getAttribute("data-detail-status") || ""
        };
    }

    function openDetail(data) {
        currentResCode = data.code || "";

        var nameParts = (data.name || "").split(" ").filter(Boolean);
        var initials = nameParts.length >= 2
            ? (nameParts[0][0] + nameParts[nameParts.length - 1][0]).toUpperCase()
            : ((data.name || "?")[0] || "?").toUpperCase();

        var avColors = [
            "linear-gradient(135deg,#3b82f6,#6366f1)",
            "linear-gradient(135deg,#10b981,#06b6d4)",
            "linear-gradient(135deg,#f59e0b,#ef4444)",
            "linear-gradient(135deg,#8b5cf6,#ec4899)",
            "linear-gradient(135deg,#06b6d4,#3b82f6)"
        ];
        var colorIndex = 0;
        for (var i = 0; i < (data.name || "").length; i++) {
            colorIndex += data.name.charCodeAt(i);
        }

        if (qs("dmAvatar")) {
            qs("dmAvatar").style.background = avColors[colorIndex % avColors.length];
            qs("dmAvatar").textContent = initials;
        }
        if (qs("dmName")) qs("dmName").textContent = data.name || "\u2014";
        if (qs("dmEmail")) qs("dmEmail").textContent = data.email || "\u2014";
        if (qs("dmCodePill")) qs("dmCodePill").textContent = data.code || "\u2014";

        var statusMap = {
            "Aktif": '<span class="badge b-green"><span class="bd"></span>Aktif</span>',
            "Yakinda": '<span class="badge b-amber"><span class="bd"></span>Yakinda</span>',
            "Yakında": '<span class="badge b-amber"><span class="bd"></span>Yakinda</span>',
            "Bugun": '<span class="badge b-blue"><span class="bd"></span>Bugun</span>',
            "Bugün": '<span class="badge b-blue"><span class="bd"></span>Bugun</span>',
            "Tamamlandi": '<span class="badge b-gray"><span class="bd"></span>Tamamlandi</span>',
            "Tamamlandı": '<span class="badge b-gray"><span class="bd"></span>Tamamlandi</span>'
        };
        if (qs("dmStatusBadge")) {
            qs("dmStatusBadge").innerHTML = statusMap[data.status] || ('<span class="badge b-gray">' + escHtml(data.status || "") + "</span>");
        }

        if (qs("dmPrice")) qs("dmPrice").textContent = "\u20ba" + (data.price || "0,00");
        if (qs("dmPriceSub")) {
            qs("dmPriceSub").textContent = (data.persons > 0 && data.rawPrice > 0)
                ? "\u20ba" + (data.rawPrice / data.persons).toLocaleString("tr-TR", { minimumFractionDigits: 2 }) + " / kisi basi"
                : "Toplam tutar";
        }

        var bubbleHtml = '<div class="dm-person-bubbles">';
        var showCount = Math.min(data.persons || 0, 6);
        for (var pb = 0; pb < showCount; pb++) {
            bubbleHtml += '<div class="dm-pbubble"><i class="fa-solid fa-user" style="font-size:9px;"></i></div>';
        }
        if ((data.persons || 0) > 6) {
            bubbleHtml += '<div class="dm-pbubble extra">+' + ((data.persons || 0) - 6) + "</div>";
        }
        bubbleHtml += "</div>";
        if (qs("dmPersonBubbles")) qs("dmPersonBubbles").innerHTML = bubbleHtml;
        if (qs("dmPersonLabel")) qs("dmPersonLabel").textContent = (data.persons || 0) + " Kisi";

        if (qs("dmTour")) qs("dmTour").textContent = data.tourName || data.tour || "\u2014";
        if (qs("dmDate")) qs("dmDate").textContent = data.date || "\u2014";
        if (qs("dmId")) {
            qs("dmId").textContent = (data.id || "").substring(0, 16) + ((data.id || "").length > 16 ? "\u2026" : "");
        }
        if (qs("dmFullName")) qs("dmFullName").textContent = data.name || "\u2014";
        if (qs("dmPhone")) qs("dmPhone").textContent = data.phone || "\u2014";
        if (qs("dmEmailFull")) qs("dmEmailFull").textContent = data.email || "\u2014";
        if (qs("dmCity")) qs("dmCity").textContent = data.city || "\u2014";
        if (qs("dmCountry")) qs("dmCountry").textContent = data.country || "\u2014";
        if (qs("dmPerPerson")) qs("dmPerPerson").textContent = (data.persons || 0) + " kisi";
        if (qs("mdEditLink")) qs("mdEditLink").href = "/AdminReservation/Edit/" + data.id;

        if (qs("detailModal")) {
            qs("detailModal").classList.add("open");
        }
    }

    function openDetailFromRowButton(btn) {
        var row = btn.closest("tr");
        if (row) openDetail(getDetailDataFromRow(row));
    }

    function closeModalById(modalId) {
        var modal = qs(modalId);
        if (modal) modal.classList.remove("open");
    }

    function confirmDelete(id, name, code) {
        if (qs("delDesc")) {
            qs("delDesc").innerHTML =
                "<strong>" + escHtml(name) + "</strong> adli musterinin <strong>" + escHtml(code) + "</strong> kodlu rezervasyonu kalici olarak silinecek. Bu islem geri alinamaz.";
        }
        if (qs("delConfirmBtn")) {
            qs("delConfirmBtn").href = "/AdminReservation/DeleteReservation/" + id;
        }
        if (qs("deleteModal")) {
            qs("deleteModal").classList.add("open");
        }
    }

    function getExportDataFromRow(row) {
        return {
            name: row.getAttribute("data-detail-name") || "",
            email: row.getAttribute("data-detail-email") || "",
            phone: row.getAttribute("data-detail-phone") || "",
            city: row.getAttribute("data-detail-city") || "",
            country: row.getAttribute("data-detail-country") || "",
            tour: row.getAttribute("data-detail-tour") || row.getAttribute("data-tour") || "",
            date: row.getAttribute("data-detail-date") || "",
            persons: row.getAttribute("data-detail-persons") || "0",
            price: row.getAttribute("data-detail-rawprice") || "0",
            code: (row.getAttribute("data-detail-code") || "").toUpperCase(),
            status: row.getAttribute("data-detail-status") || "",
            fields: "all"
        };
    }

    function selectFormat(fmt) {
        exportFormat = fmt;
        if (qs("cardExcel")) qs("cardExcel").classList.toggle("selected", fmt === "excel");
        if (qs("cardPdf")) qs("cardPdf").classList.toggle("selected", fmt === "pdf");

        var btn = qs("expDownloadBtn");
        if (!btn) return;

        if (fmt === "excel") {
            btn.style.background = "linear-gradient(135deg,#10b981,#059669)";
            btn.style.boxShadow = "0 2px 14px rgba(16,185,129,.35)";
            btn.innerHTML = '<i class="fa-solid fa-file-excel"></i> Excel Olarak Indir';
        } else {
            btn.style.background = "linear-gradient(135deg,#f43f5e,#dc2626)";
            btn.style.boxShadow = "0 2px 14px rgba(244,63,94,.35)";
            btn.innerHTML = '<i class="fa-solid fa-file-pdf"></i> PDF Olarak Indir';
        }
    }

    function openExportModal(row, fmt) {
        exportCurrentRow = row;
        if (qs("expTourName")) qs("expTourName").textContent = row.getAttribute("data-detail-tour") || row.getAttribute("data-tour") || "\u2014";
        if (qs("expTourMeta")) {
            qs("expTourMeta").textContent =
                (row.getAttribute("data-detail-date") || "\u2014") + "  \u00b7  " +
                (row.getAttribute("data-detail-persons") || "\u2014") + " kisi  \u00b7  Kod: " +
                ((row.getAttribute("data-detail-code") || "\u2014").toUpperCase());
        }
        if (qs("expModalSub")) qs("expModalSub").textContent = "Rapor formatini secin";
        selectFormat(fmt || "excel");
        if (qs("exportModal")) qs("exportModal").classList.add("open");
    }

    function exportRowExcel(data) {
        var sep = "\t";
        var nl = "\r\n";
        var rows = [];

        rows.push("REZERVASYON RAPORU" + sep + data.tour);
        rows.push("Olusturma Tarihi:" + sep + new Date().toLocaleDateString("tr-TR"));
        rows.push("");

        var allFields = [
            ["Rezervasyon Kodu", data.code],
            ["Ad Soyad", data.name],
            ["E-posta", data.email],
            ["Telefon", data.phone],
            ["Tur", data.tour],
            ["Rezervasyon Tarihi", data.date],
            ["Kisi Sayisi", data.persons],
            ["Toplam Tutar", formatPrice(data.price)],
            ["Sehir", data.city],
            ["Ulke", data.country],
            ["Durum", data.status]
        ];

        var filtered = allFields;
        if (data.fields === "basic") filtered = allFields.filter(function (r) { return ["Rezervasyon Kodu", "Ad Soyad", "Tur", "Rezervasyon Tarihi", "Kisi Sayisi", "Durum"].indexOf(r[0]) > -1; });
        if (data.fields === "financial") filtered = allFields.filter(function (r) { return ["Rezervasyon Kodu", "Ad Soyad", "Tur", "Toplam Tutar", "Kisi Sayisi"].indexOf(r[0]) > -1; });
        if (data.fields === "contact") filtered = allFields.filter(function (r) { return ["Ad Soyad", "E-posta", "Telefon", "Sehir", "Ulke"].indexOf(r[0]) > -1; });

        rows.push("Alan" + sep + "Deger");
        filtered.forEach(function (pair) {
            rows.push(pair[0] + sep + (pair[1] || ""));
        });

        downloadBlob(new Blob(["\uFEFF" + rows.join(nl)], { type: "application/vnd.ms-excel;charset=utf-8;" }), "Rezervasyon_" + (data.code || "rapor") + ".xls");
        return true;
    }

    function exportRowPdf(data) {
        var personCount = parseInt(data.persons, 10) || 0;
        var priceNum = parseFloat(data.price) || 0;
        var perPerson = personCount > 0 && priceNum > 0 ? formatPrice(String(priceNum / personCount)) + " / kisi basi" : "-";
        var now = new Date();
        var nowStr = now.toLocaleDateString("tr-TR") + " " + now.toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit" });

        function he(s) {
            return String(s || "-")
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/"/g, "&quot;");
        }

        function row(label, value, fullWidth) {
            var span = fullWidth ? ' style="grid-column:1/-1"' : "";
            return "<div" + span + '><div class="fl">' + label + '</div><div class="fv">' + he(value) + "</div></div>";
        }

        var html = [
            "<!DOCTYPE html>",
            '<html lang="tr">',
            "<head>",
            '<meta charset="UTF-8">',
            "<title>Rezervasyon Raporu - " + he(data.code) + "</title>",
            "<style>",
            "*{box-sizing:border-box;margin:0;padding:0}",
            "body{font-family:Arial,Helvetica,sans-serif;color:#1e293b;background:#fff}",
            ".hd{background:#0f172a;color:#fff;padding:28px 36px 24px}",
            ".hd h1{font-size:20px;font-weight:800;margin-bottom:4px}",
            ".hd .sub{font-size:12px;opacity:.6}",
            ".hd .ts{font-size:10px;opacity:.4;margin-top:12px}",
            ".bd{padding:28px 36px}",
            ".sec{font-size:9px;font-weight:700;letter-spacing:2px;text-transform:uppercase;color:#94a3b8;border-bottom:1px solid #e2e8f0;padding-bottom:5px;margin:20px 0 12px}",
            ".sec:first-child{margin-top:0}",
            ".gr{display:grid;grid-template-columns:1fr 1fr;gap:10px 20px}",
            ".fl{font-size:9px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#94a3b8;margin-bottom:3px}",
            ".fv{font-size:13px;font-weight:600;color:#1e293b}",
            ".pb{background:#f1f5f9;color:#64748b;border:1px solid #e2e8f0;border-radius:20px;padding:3px 11px;font-size:11px;font-weight:700;display:inline-block}",
            ".pricebox{background:#f0fdf4;border:1px solid #bbf7d0;border-radius:10px;padding:14px 18px;margin:14px 0;display:flex;align-items:center;gap:14px}",
            ".pv{font-size:24px;font-weight:800;color:#15803d;letter-spacing:-.5px}",
            ".ps{font-size:11px;color:#16a34a;margin-top:2px}",
            ".ft{border-top:1px solid #e2e8f0;padding:12px 36px;font-size:9px;color:#94a3b8;display:flex;justify-content:space-between}",
            "@media print{body{-webkit-print-color-adjust:exact;print-color-adjust:exact}.hd{background:#0f172a !important;-webkit-print-color-adjust:exact}}",
            "</style>",
            "</head>",
            "<body>",
            '<div class="hd"><h1>Rezervasyon Raporu</h1><div class="sub">' + he(data.tour) + '</div><div class="ts">Olusturma: ' + he(nowStr) + "</div></div>",
            '<div class="bd">',
            '<div class="sec">Genel Bilgiler</div>',
            '<div class="gr">',
            row("Rezervasyon Kodu", data.code),
            '<div><div class="fl">Durum</div><div class="fv"><span class="pb">' + he(data.status) + "</span></div></div>",
            row("Tur", data.tour),
            row("Rezervasyon Tarihi", data.date),
            "</div>",
            '<div class="pricebox"><div><div class="pv">' + formatPrice(data.price) + '</div><div class="ps">' + he(data.persons) + " kisi · " + he(perPerson) + "</div></div></div>",
            '<div class="sec">Musteri Bilgileri</div>',
            '<div class="gr">',
            row("Ad Soyad", data.name),
            row("Telefon", data.phone),
            row("E-posta", data.email, true),
            "</div>",
            '<div class="sec">Konum</div>',
            '<div class="gr">',
            row("Sehir", data.city),
            row("Ulke", data.country),
            "</div>",
            "</div>",
            '<div class="ft"><span>Vitour Admin Panel</span><span>Gizlidir</span></div>',
            "</body>",
            "</html>"
        ].join("\n");

        var win = window.open("", "_blank", "width=820,height=940,scrollbars=yes");
        if (!win) {
            alert("Pop-up engellendi. Lutfen bu site icin pop-up izni verin.");
            return false;
        }

        win.document.open();
        win.document.write(html);
        win.document.close();
        setTimeout(function () {
            win.focus();
            win.print();
        }, 300);
        return true;
    }

    function doExport() {
        if (!exportCurrentRow) return;

        var data = getExportDataFromRow(exportCurrentRow);
        if (qs("expFields")) data.fields = qs("expFields").value;

        var btn = qs("expDownloadBtn");
        var original = btn ? btn.innerHTML : "";
        if (btn) {
            btn.innerHTML = '<span class="exp-spinner"></span> Hazirlaniyor...';
            btn.disabled = true;
        }

        var ok = false;
        try {
            ok = exportFormat === "excel" ? exportRowExcel(data) : exportRowPdf(data);
        } catch (err) {
            console.error("ReservationList export error:", err);
            if (typeof window.showToast === "function") window.showToast("Export sirasinda hata olustu.", "e");
        }

        if (btn) {
            btn.innerHTML = original;
            btn.disabled = false;
        }

        if (ok) {
            closeModalById("exportModal");
            if (typeof window.showToast === "function") {
                window.showToast((exportFormat === "excel" ? "Excel" : "PDF") + " raporu indirildi.", "s");
            }
        }
    }

    function exportCSV() {
        var headers = ["Ad Soyad", "E-posta", "Telefon", "Rezervasyon Kodu", "Tur", "Tarih", "Kisi", "Tutar", "Sehir", "Ulke"];
        var lines = [headers.join(",")];

        Array.from(document.querySelectorAll("#tableBody tr")).filter(function (row) {
            return row.style.display !== "none";
        }).forEach(function (row) {
            var cells = row.querySelectorAll("td");
            var city = "";
            var country = "";
            var loc = cells[7];
            if (loc) {
                var divs = loc.querySelectorAll("div");
                city = divs[0] ? divs[0].textContent.trim() : "";
                country = divs[1] ? divs[1].textContent.trim() : "";
            }

            lines.push([
                (row.dataset.name || "").replace(/,/g, ""),
                (row.dataset.email || "").replace(/,/g, ""),
                "",
                (row.dataset.code || "").replace(/,/g, ""),
                (row.getAttribute("data-detail-tour") || row.dataset.tour || "").replace(/,/g, ""),
                row.dataset.date || "",
                row.dataset.persons || "",
                row.dataset.price || "",
                city.replace(/,/g, ""),
                country.replace(/,/g, "")
            ].join(","));
        });

        downloadBlob(new Blob(["\uFEFF" + lines.join("\n")], { type: "text/csv;charset=utf-8;" }), "rezervasyonlar.csv");
        if (typeof window.showToast === "function") window.showToast("CSV disa aktarildi.", "s");
    }

    function bindEvents() {
        ["searchInput", "tourFilter", "dateFilter", "personFilter"].forEach(function (id) {
            var element = qs(id);
            if (!element) return;
            var eventName = id === "searchInput" ? "input" : "change";
            element.addEventListener(eventName, applyFilters);
        });

        if (qs("clearBtn")) qs("clearBtn").addEventListener("click", clearFilters);
        if (qs("checkAll")) qs("checkAll").addEventListener("change", function () { toggleAll(this); });
        if (qs("prevBtn")) qs("prevBtn").addEventListener("click", function () { curPage = Math.max(1, curPage - 1); renderPage(); });
        if (qs("nextBtn")) qs("nextBtn").addEventListener("click", function () { curPage += 1; renderPage(); });
        if (qs("dmCopyBtn")) qs("dmCopyBtn").addEventListener("click", function () {
            if (!currentResCode || !navigator.clipboard) return;
            navigator.clipboard.writeText(currentResCode).then(function () {
                var btn = qs("dmCopyBtn");
                if (!btn) return;
                btn.innerHTML = '<i class="fa-solid fa-check"></i> Kopyalandi!';
                btn.style.color = "var(--green)";
                setTimeout(function () {
                    btn.innerHTML = '<i class="fa-regular fa-copy"></i> Kodu Kopyala';
                    btn.style.color = "";
                }, 2000);
                if (typeof window.showToast === "function") window.showToast("Rezervasyon kodu kopyalandi.", "s");
            });
        });
        if (qs("cardExcel")) qs("cardExcel").addEventListener("click", function () { selectFormat("excel"); });
        if (qs("cardPdf")) qs("cardPdf").addEventListener("click", function () { selectFormat("pdf"); });
        if (qs("expDownloadBtn")) qs("expDownloadBtn").addEventListener("click", doExport);
        if (qs("exportAllBtn")) qs("exportAllBtn").addEventListener("click", exportCSV);

        document.querySelectorAll("[data-sort]").forEach(function (el) {
            el.addEventListener("click", function () {
                sortTable(el.getAttribute("data-sort"));
            });
        });

        document.querySelectorAll(".detail-btn").forEach(function (btn) {
            btn.addEventListener("click", function () {
                openDetailFromRowButton(btn);
            });
        });

        document.querySelectorAll(".export-row-btn").forEach(function (btn) {
            btn.addEventListener("click", function () {
                var row = btn.closest("tr");
                if (row) openExportModal(row, btn.getAttribute("data-format") || "excel");
            });
        });

        document.querySelectorAll(".delete-btn").forEach(function (btn) {
            btn.addEventListener("click", function () {
                confirmDelete(
                    btn.getAttribute("data-delete-id"),
                    btn.getAttribute("data-delete-name"),
                    btn.getAttribute("data-delete-code")
                );
            });
        });

        document.querySelectorAll("[data-close-modal]").forEach(function (btn) {
            btn.addEventListener("click", function () {
                closeModalById(btn.getAttribute("data-close-modal"));
            });
        });

        document.querySelectorAll("[data-close-on-overlay]").forEach(function (overlay) {
            overlay.addEventListener("click", function (event) {
                if (event.target === overlay) {
                    overlay.classList.remove("open");
                }
            });
        });

        document.addEventListener("keydown", function (event) {
            if (event.key === "Escape") {
                document.querySelectorAll(".modal-ov.open").forEach(function (modal) {
                    modal.classList.remove("open");
                });
            }
        });
    }

    Array.from(document.querySelectorAll("#tableBody tr")).forEach(function (row) {
        row.dataset.filteredOut = "false";
    });

    bindEvents();
    applyFilters();

    window.applyFilters = applyFilters;
    window.clearFilters = clearFilters;
    window.sortTable = sortTable;
    window.toggleAll = toggleAll;
    window.openDetailFromRowButton = openDetailFromRowButton;
    window.closeModalById = closeModalById;
    window.confirmDelete = confirmDelete;
    window.selectFormat = selectFormat;
    window.doExport = doExport;
    window.exportCSV = exportCSV;
})();
