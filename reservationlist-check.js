            <button type="button" class="btn btn-ghost" onclick="closeModalById('exportModal')" style="flex:1;">
                <i class="fa-solid fa-xmark"></i> Ýptal
            </button>
            <button type="button" class="btn" id="expDownloadBtn"
                    style="flex:2;background:linear-gradient(135deg,#10b981,#059669);color:#fff;box-shadow:0 2px 14px rgba(16,185,129,.35);">
                <i class="fa-solid fa-download"></i> Ýndir
            </button>
        </div>

    </div>
</div>

@section Scripts {
<script>
/* ==============================================
   FÝLTRELEME
============================================== */
function applyFilters() {
    var search  = document.getElementById('searchInput').value.toLowerCase().trim();
    var tour    = document.getElementById('tourFilter').value;
    var date    = document.getElementById('dateFilter').value;
    var persons = document.getElementById('personFilter').value;

    var rows    = document.querySelectorAll('#tableBody tr');
    var visible = 0;

    rows.forEach(function(row) {
        var name    = row.dataset.name    || '';
        var email   = row.dataset.email   || '';
        var code    = row.dataset.code    || '';
        var rowTour = row.dataset.tour    || '';
        var rowDate = row.dataset.date    || '';
        var cnt     = parseInt(row.dataset.persons) || 0;

        var matchSearch  = !search  || name.includes(search) || email.includes(search) || code.includes(search);
        var matchTour    = !tour    || rowTour === tour;
        var matchDate    = !date    || rowDate === date;
        var matchPersons = !persons || matchPersonCount(cnt, persons);

        var show = matchSearch && matchTour && matchDate && matchPersons;
        row.dataset.filteredOut = show ? 'false' : 'true';
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    document.getElementById('visibleCount').textContent = visible;
    document.getElementById('emptyState').style.display = visible === 0 ? 'flex' : 'none';

    var hasFilter = search || tour || date || persons;
    document.getElementById('clearBtn').style.display = hasFilter ? 'inline-flex' : 'none';
    curPage = 1;
    renderPage();
}

function matchPersonCount(cnt, filter) {
    if (filter === '1')    return cnt === 1;
    if (filter === '2-5')  return cnt >= 2 && cnt <= 5;
    if (filter === '6-10') return cnt >= 6 && cnt <= 10;
    if (filter === '10+')  return cnt > 10;
    return true;
}

function clearFilters() {
    document.getElementById('searchInput').value = '';
    document.getElementById('tourFilter').value  = '';
    document.getElementById('dateFilter').value  = '';
    document.getElementById('personFilter').value= '';
    applyFilters();
}

/* ==============================================
   SIRALAMA
============================================== */
var sortState = {};
function sortTable(col) {
    sortState[col] = !sortState[col]; /* toggle asc/desc */
    var asc  = sortState[col];
    var tbody = document.getElementById('tableBody');
    var rows  = Array.from(tbody.querySelectorAll('tr'));

    rows.sort(function(a, b) {
        var va, vb;
        if (col === 'name')  { va = a.dataset.name;  vb = b.dataset.name;  return asc ? va.localeCompare(vb) : vb.localeCompare(va); }
        if (col === 'code')  { va = a.dataset.code;  vb = b.dataset.code;  return asc ? va.localeCompare(vb) : vb.localeCompare(va); }
        if (col === 'date')  { va = a.dataset.date;  vb = b.dataset.date;  return asc ? va.localeCompare(vb) : vb.localeCompare(va); }
        if (col === 'price') { va = parseFloat(a.dataset.price); vb = parseFloat(b.dataset.price); return asc ? va - vb : vb - va; }
        return 0;
    });

    rows.forEach(function(r){ tbody.appendChild(r); });
}

/* ==============================================
   CHECKBOX
============================================== */
function toggleAll(master) {
    document.querySelectorAll('.row-check').forEach(function(cb){
        cb.checked = master.checked;
    });
}

/* ==============================================
   DETAY BUTONU — EVENT DELEGATION
============================================== */
function getDetailDataFromRow(row) {
    return {
        id       : row.getAttribute('data-detail-id')       || '',
        code     : row.getAttribute('data-detail-code')     || '',
        name     : row.getAttribute('data-detail-name')     || '',
        phone    : row.getAttribute('data-detail-phone')    || '',
        email    : row.getAttribute('data-detail-email')    || '',
        city     : row.getAttribute('data-detail-city')     || '',
        country  : row.getAttribute('data-detail-country')  || '',
        tourName : row.getAttribute('data-detail-tour')     || '',
        tour     : row.getAttribute('data-detail-tour')     || '',
        date     : row.getAttribute('data-detail-date')     || '',
        persons  : parseInt(row.getAttribute('data-detail-persons'))  || 0,
        price    : row.getAttribute('data-detail-price')    || '0,00',
        rawPrice : parseFloat(row.getAttribute('data-detail-rawprice')) || 0,
        status   : row.getAttribute('data-detail-status')   || ''
    };
}

function openDetailFromRowButton(btn) {
    var row = btn.closest('tr');
    if (!row) return;
    openDetail(getDetailDataFromRow(row));
}

function openDetailByRowId(rowId) {
    var row = document.getElementById(rowId);
    if (!row) return false;
    openDetail(getDetailDataFromRow(row));
    return false;
}
var _currentResCode = '';

function openDetail(data) {
    _currentResCode = data.code || '';

    /* ¦¦ Avatar ¦¦ */
    var parts    = (data.name || '').split(' ');
    var initials = parts.length >= 2
        ? (parts[0][0] + parts[parts.length-1][0]).toUpperCase()
        : (data.name || '?')[0].toUpperCase();
    var avColors = [
        'linear-gradient(135deg,#3b82f6,#6366f1)',
        'linear-gradient(135deg,#10b981,#06b6d4)',
        'linear-gradient(135deg,#f59e0b,#ef4444)',
        'linear-gradient(135deg,#8b5cf6,#ec4899)',
        'linear-gradient(135deg,#06b6d4,#3b82f6)'
    ];
    var ci = 0;
    for (var x=0; x<(data.name||'').length; x++) ci += (data.name||'').charCodeAt(x);
    var avColor = avColors[ci % avColors.length];
    document.getElementById('dmAvatar').style.background = avColor;
    document.getElementById('dmAvatar').textContent      = initials;

    /* ¦¦ Header bilgileri ¦¦ */
    document.getElementById('dmName').textContent  = data.name  || '—';
    document.getElementById('dmEmail').textContent = data.email || '—';
    document.getElementById('dmCodePill').textContent = data.code || '—';

    /* ¦¦ Durum badge ¦¦ */
    var statusMap = {
        'Aktif'      : '<span class="badge b-green"><span class="bd"></span>Aktif</span>',
        'Yakýnda'    : '<span class="badge b-amber"><span class="bd"></span>Yakýnda</span>',
        'Bugün'      : '<span class="badge b-blue"><span class="bd"></span>Bugün</span>',
        'Tamamlandý' : '<span class="badge b-gray"><span class="bd"></span>Tamamlandý</span>'
    };
    document.getElementById('dmStatusBadge').innerHTML = statusMap[data.status] || ('<span class="badge b-gray">' + escHtml(data.status||'') + '</span>');

    /* ¦¦ Fiyat bloðu ¦¦ */
    document.getElementById('dmPrice').textContent   = '?' + (data.price || '0,00');
    var pprice = (data.persons > 0 && data.rawPrice > 0)
        ? '?' + (data.rawPrice / data.persons).toLocaleString('tr-TR', {minimumFractionDigits:2}) + ' / kiþi baþý'
        : 'Toplam tutar';
    document.getElementById('dmPriceSub').textContent = pprice;

    /* ¦¦ Kiþi baloncuklarý ¦¦ */
    var cnt = parseInt(data.persons) || 0;
    var bubbleHtml = '<div class="dm-person-bubbles">';
    var show = Math.min(cnt, 6);
    for (var i=0; i<show; i++) {
        bubbleHtml += '<div class="dm-pbubble"><i class="fa-solid fa-user" style="font-size:9px;"></i></div>';
    }
    if (cnt > 6) bubbleHtml += '<div class="dm-pbubble extra">+' + (cnt-6) + '</div>';
    bubbleHtml += '</div>';
    document.getElementById('dmPersonBubbles').innerHTML = bubbleHtml;
    document.getElementById('dmPersonLabel').textContent = cnt + ' Kiþi';

    /* ¦¦ Tur & Tarih ¦¦ */
    document.getElementById('dmTour').textContent = data.tourName || data.tour || '—';
    document.getElementById('dmDate').textContent = data.date    || '—';
    document.getElementById('dmId').textContent   = (data.id || '').substring(0, 16) + (data.id && data.id.length > 16 ? '…' : '');

    /* ¦¦ Müþteri ¦¦ */
    document.getElementById('dmFullName').textContent  = data.name    || '—';
    document.getElementById('dmPhone').textContent     = data.phone   || '—';
    document.getElementById('dmEmailFull').textContent = data.email   || '—';

    /* ¦¦ Konum ¦¦ */
    document.getElementById('dmCity').textContent    = data.city    || '—';
    document.getElementById('dmCountry').textContent = data.country || '—';
    document.getElementById('dmPerPerson').textContent = cnt + ' kiþi';

    /* ¦¦ Düzenle linki ¦¦ */
    document.getElementById('mdEditLink').href = '/AdminReservation/Edit/' + data.id;

    /* ¦¦ Aç ¦¦ */
    document.getElementById('detailModal').classList.add('open');
}

function copyResCode() {
    if (!_currentResCode) return;
    navigator.clipboard.writeText(_currentResCode).then(function(){
        var btn = document.getElementById('dmCopyBtn');
        btn.innerHTML = '<i class="fa-solid fa-check"></i> Kopyalandý!';
        btn.style.color = 'var(--green)';
        setTimeout(function(){
            btn.innerHTML = '<i class="fa-regular fa-copy"></i> Kodu Kopyala';
            btn.style.color = '';
        }, 2000);
        if (typeof showToast === 'function') showToast('Rezervasyon kodu kopyalandý.', 's');
    });
}

function escHtml(s) {
    return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}

/* ==============================================
   SÝLME MODALI
============================================== */
function confirmDelete(id, name, code) {
    document.getElementById('delDesc').innerHTML =
        '<strong>' + escHtml(name) + '</strong> adlý müþteriye ait <strong>' + escHtml(code) + '</strong> kodlu rezervasyon kalýcý olarak silinecek. Bu iþlem geri alýnamaz.';
    document.getElementById('delConfirmBtn').href = '/AdminReservation/Delete/' + id;
    document.getElementById('deleteModal').classList.add('open');
}

/* ==============================================
   MODAL KAPAT
============================================== */
function closeModal(event, modalId) {
    if (event.target === document.getElementById(modalId))
        closeModalById(modalId);
}
function closeModalById(modalId) {
    document.getElementById(modalId).classList.remove('open');
}
document.addEventListener('keydown', function(e){
    if (e.key === 'Escape') {
        document.querySelectorAll('.modal-ov.open').forEach(function(m){ m.classList.remove('open'); });
    }
});

/* ==============================================
   SAYFALAMA (basit client-side)
============================================== */
var curPage = 1;
var pageSize = 10;

function changePage(dir) {
    var rows = Array.from(document.querySelectorAll('#tableBody tr'));
    var total = rows.filter(function(r) { return r.dataset.filteredOut !== 'true'; }).length;
    var pages = Math.ceil(total / pageSize);
    if (pages < 1) pages = 1;
    curPage = Math.max(1, Math.min(curPage + dir, pages));
    renderPage();
}

function renderPage() {
    var rows = Array.from(document.querySelectorAll('#tableBody tr'));
    var visibleRows = rows.filter(function(r) { return r.dataset.filteredOut !== 'true'; });
    var total = visibleRows.length;
    var pages = Math.ceil(total / pageSize);
    if (pages < 1) pages = 1;
    var start = (curPage - 1) * pageSize;
    var end   = start + pageSize;

    rows.forEach(function(r) {
        if (r.dataset.filteredOut === 'true') {
            r.style.display = 'none';
        }
    });
    visibleRows.forEach(function(r, i) {
        r.style.display = (i >= start && i < end) ? '' : 'none';
    });

    document.getElementById('prevBtn').disabled = curPage <= 1;
    document.getElementById('nextBtn').disabled = curPage >= pages;
    document.getElementById('pageInfo').textContent =
        total === 0 ? '0 / 0' : (start + 1) + '–' + Math.min(end, total) + ' / ' + total;

    /* Aktif sayfa butonu */
    ['page1Btn','page2Btn','page3Btn'].forEach(function(id, i){
        var btn = document.getElementById(id);
        if (btn) btn.classList.toggle('on', curPage === i + 1);
    });
}

/* ==============================================
   EXPORT BUTONU — EVENT DELEGATION (satýr bazlý)
============================================== */
var _expCurrentRow = null;
var _expFormat     = 'excel';

function getExportDataFromRow(row) {
    return {
        name    : row.getAttribute('data-detail-name')    || '',
        email   : row.getAttribute('data-detail-email')   || '',
        phone   : row.getAttribute('data-detail-phone')   || '',
        city    : row.getAttribute('data-detail-city')    || '',
        country : row.getAttribute('data-detail-country') || '',
        tour    : row.getAttribute('data-detail-tour')    || row.getAttribute('data-tour') || '',
        date    : row.getAttribute('data-detail-date')    || '',
        persons : row.getAttribute('data-detail-persons') || '0',
        price   : row.getAttribute('data-detail-rawprice')|| '0',
        code    : (row.getAttribute('data-detail-code')   || '').toUpperCase(),
        status  : row.getAttribute('data-detail-status')  || '',
        fields  : 'all'
    };
}

function openExportModalByRowId(rowId, fmt) {
    var row = document.getElementById(rowId);
    if (!row) return false;
    _expCurrentRow = row;
    openExportModal(row, fmt || 'excel');
    return false;
}

function openExportModal(row, fmt) {
    var tourName = row.getAttribute('data-detail-tour')    || row.getAttribute('data-tour') || '—';
    var date     = row.getAttribute('data-detail-date')    || '—';
    var persons  = row.getAttribute('data-detail-persons') || '—';
    var code     = (row.getAttribute('data-detail-code')   || '—').toUpperCase();

    document.getElementById('expTourName').textContent = tourName;
    document.getElementById('expTourMeta').textContent = date + '  \u00b7  ' + persons + ' ki\u015fi  \u00b7  Kod: ' + code;
    document.getElementById('expModalSub').textContent = 'Rapor format\u0131n\u0131 se\u00e7in';

    selectFormat(fmt);
    document.getElementById('exportModal').classList.add('open');
}

function selectFormat(fmt) {
    _expFormat = fmt;
    document.getElementById('cardExcel').classList.toggle('selected', fmt === 'excel');
    document.getElementById('cardPdf').classList.toggle('selected',   fmt === 'pdf');

    var btn = document.getElementById('expDownloadBtn');
    if (fmt === 'excel') {
        btn.style.background = 'linear-gradient(135deg,#10b981,#059669)';
        btn.style.boxShadow  = '0 2px 14px rgba(16,185,129,.35)';
        btn.innerHTML        = '<i class="fa-solid fa-file-excel"></i> Excel Olarak \u0130ndir';
    } else {
        btn.style.background = 'linear-gradient(135deg,#f43f5e,#dc2626)';
        btn.style.boxShadow  = '0 2px 14px rgba(244,63,94,.35)';
        btn.innerHTML        = '<i class="fa-solid fa-file-pdf"></i> PDF Olarak \u0130ndir';
    }
}

function doExport() {
    if (!_expCurrentRow) return;
    var d = getExportDataFromRow(_expCurrentRow);
    d.fields = document.getElementById('expFields').value;

    var btn      = document.getElementById('expDownloadBtn');
    var origHtml = btn.innerHTML;
    btn.innerHTML = '<span class="exp-spinner"></span> Haz\u0131rlan\u0131yor\u2026';
    btn.disabled  = true;

    var ok = false;
    try {
        if (_expFormat === 'excel') {
            ok = exportRowExcel(d);
        } else {
            ok = exportRowPdf(d);
        }
    } catch(err) {
        console.error('Export hatas\u0131:', err);
        if (typeof showToast === 'function') showToast('Export s\u0131ras\u0131nda hata olu\u015ftu.', 'e');
    }

    btn.innerHTML = origHtml;
    btn.disabled  = false;

    if (ok) {
        closeModalById('exportModal');
        if (typeof showToast === 'function') {
            showToast((_expFormat === 'excel' ? 'Excel' : 'PDF') + ' raporu indirildi.', 's');
        }
    }
}

/* ================================================
   EXCEL — sekme ayrýmlý .xls (Excel'de açýlýr)
================================================ */
function exportRowExcel(d) {
    var sep  = '\t';
    var nl   = '\r\n';
    var rows = [];

    rows.push('REZERVASYON RAPORU' + sep + d.tour);
    rows.push('Olu\u015fturma Tarihi:' + sep + new Date().toLocaleDateString('tr-TR'));
    rows.push('');

    var allFields = [
        ['Rezervasyon Kodu',  d.code],
        ['Ad Soyad',          d.name],
        ['E-posta',           d.email],
        ['Telefon',           d.phone],
        ['Tur',               d.tour],
        ['Rezervasyon Tarihi',d.date],
        ['Ki\u015fi Say\u0131s\u0131',    d.persons],
        ['Toplam Tutar',      formatPrice(d.price)],
        ['\u015eehir',             d.city],
        ['\u00dclke',            d.country],
        ['Durum',             d.status]
    ];

    var filtered = allFields;
    var f = d.fields;
    if (f === 'basic')     filtered = allFields.filter(function(r){ return ['Rezervasyon Kodu','Ad Soyad','Tur','Rezervasyon Tarihi','Ki\u015fi Say\u0131s\u0131','Durum'].indexOf(r[0]) > -1; });
    if (f === 'financial') filtered = allFields.filter(function(r){ return ['Rezervasyon Kodu','Ad Soyad','Tur','Toplam Tutar','Ki\u015fi Say\u0131s\u0131'].indexOf(r[0]) > -1; });
    if (f === 'contact')   filtered = allFields.filter(function(r){ return ['Ad Soyad','E-posta','Telefon','\u015eehir','\u00dclke'].indexOf(r[0]) > -1; });

    rows.push('Alan' + sep + 'De\u011fer');
    filtered.forEach(function(pair) { rows.push(pair[0] + sep + (pair[1] || '')); });

    var content = '\ufeff' + rows.join(nl);
    var blob    = new Blob([content], { type: 'application/vnd.ms-excel;charset=utf-8;' });
    downloadBlob(blob, 'Rezervasyon_' + (d.code || 'rapor') + '.xls');
    return true;
}

/* ================================================
   PDF — yeni pencere + window.print()
================================================ */
function exportRowPdf(d) {
    var personCount = parseInt(d.persons) || 0;
    var priceNum    = parseFloat(d.price) || 0;
    var pprice      = (personCount > 0 && priceNum > 0)
        ? formatPrice(String(priceNum / personCount)) + ' / ki\u015fi ba\u015f\u0131'
        : '\u2014';

    var scMap = {
        'Aktif'      : 'background:#dcfce7;color:#15803d;border:1px solid #bbf7d0',
        'Yak\u0131nda'    : 'background:#fef3c7;color:#b45309;border:1px solid #fde68a',
        'Bug\u00fcn'      : 'background:#dbeafe;color:#1d4ed8;border:1px solid #bfdbfe',
        'Tamamland\u0131' : 'background:#f1f5f9;color:#64748b;border:1px solid #e2e8f0'
    };
    var sc = scMap[d.status] || scMap['Tamamland\u0131'];

    var now     = new Date();
    var nowStr  = now.toLocaleDateString('tr-TR') + ' ' + now.toLocaleTimeString('tr-TR', {hour:'2-digit', minute:'2-digit'});

    function row(label, value, fullWidth) {
        var span = fullWidth ? ' style="grid-column:1/-1"' : '';
        return '<div' + span + '><div class="fl">' + label + '</div><div class="fv">' + he(value) + '</div></div>';
    }

    function he(s) {
        return String(s || '\u2014')
            .replace(/&/g,'&amp;')
            .replace(/</g,'&lt;')
            .replace(/>/g,'&gt;')
            .replace(/"/g,'&quot;');
    }

    var html = [
        '<!DOCTYPE html>',
        '<html lang="tr">',
        '<head>',
        '<meta charset="UTF-8">',
        '<title>Rezervasyon Raporu - ' + he(d.code) + '</title>',
        '<style>',
        '*{box-sizing:border-box;margin:0;padding:0}',
        'body{font-family:Arial,Helvetica,sans-serif;color:#1e293b;background:#fff}',
        '.hd{background:#0f172a;color:#fff;padding:28px 36px 24px}',
        '.hd h1{font-size:20px;font-weight:800;margin-bottom:4px}',
        '.hd .sub{font-size:12px;opacity:.6}',
        '.hd .ts{font-size:10px;opacity:.4;margin-top:12px}',
        '.bd{padding:28px 36px}',
        '.sec{font-size:9px;font-weight:700;letter-spacing:2px;text-transform:uppercase;color:#94a3b8;',
        '     border-bottom:1px solid #e2e8f0;padding-bottom:5px;margin:20px 0 12px}',
        '.sec:first-child{margin-top:0}',
        '.gr{display:grid;grid-template-columns:1fr 1fr;gap:10px 20px}',
        '.fl{font-size:9px;font-weight:700;letter-spacing:1px;text-transform:uppercase;color:#94a3b8;margin-bottom:3px}',
        '.fv{font-size:13px;font-weight:600;color:#1e293b}',
        '.pb{' + sc + ';border-radius:20px;padding:3px 11px;font-size:11px;font-weight:700;display:inline-block}',
        '.pricebox{background:#f0fdf4;border:1px solid #bbf7d0;border-radius:10px;',
        '          padding:14px 18px;margin:14px 0;display:flex;align-items:center;gap:14px}',
        '.pv{font-size:24px;font-weight:800;color:#15803d;letter-spacing:-.5px}',
        '.ps{font-size:11px;color:#16a34a;margin-top:2px}',
        '.ft{border-top:1px solid #e2e8f0;padding:12px 36px;font-size:9px;color:#94a3b8;',
        '    display:flex;justify-content:space-between}',
        '@@media print{',
        ' body{-webkit-print-color-adjust:exact;print-color-adjust:exact}',
        ' .hd{background:#0f172a !important;-webkit-print-color-adjust:exact}',
        '}',
        '</style>',
        '</head>',
        '<body>',
        '<div class="hd">',
        '  <h1>Rezervasyon Raporu</h1>',
        '  <div class="sub">' + he(d.tour) + '</div>',
        '  <div class="ts">Olu\u015fturma: ' + he(nowStr) + '</div>',
        '</div>',
        '<div class="bd">',
        '  <div class="sec">Genel Bilgiler</div>',
        '  <div class="gr">',
        row('Rezervasyon Kodu', d.code),
        '    <div><div class="fl">Durum</div><div class="fv"><span class="pb">' + he(d.status) + '</span></div></div>',
        row('Tur', d.tour),
        row('Rezervasyon Tarihi', d.date),
        '  </div>',
        '  <div class="pricebox">',
        '    <div>',
        '      <div class="pv">' + formatPrice(d.price) + '</div>',
        '      <div class="ps">' + he(d.persons) + ' ki\u015fi  \u00b7  ' + he(pprice) + '</div>',
        '    </div>',
        '  </div>',
        '  <div class="sec">M\u00fc\u015fteri Bilgileri</div>',
        '  <div class="gr">',
        row('Ad Soyad', d.name),
        row('Telefon', d.phone),
        row('E-posta', d.email, true),
        '  </div>',
        '  <div class="sec">Konum</div>',
        '  <div class="gr">',
        row('\u015eehir', d.city),
        row('\u00dclke', d.country),
        '  </div>',
        '</div>',
        '<div class="ft">',
        '  <span>Vitour Admin Panel</span>',
        '  <span>Gizlidir \u2014 Yaln\u0131zca yetkili personel</span>',
        '</div>',
        '</body>',
        '</html>'
    ].join('\n');

    var win = window.open('', '_blank', 'width=820,height=940,scrollbars=yes');
    if (!win) {
        alert('Pop-up engellendi. L\u00fctfen bu site i\u00e7in pop-up\u2019lara izin verin.');
        return false;
    }
    win.document.open();
    win.document.write(html);
    win.document.close();
    setTimeout(function() { win.focus(); win.print(); }, 400);
    return true;
}

/* ================================================
   YARDIMCI
================================================ */
function formatPrice(val) {
    var n = parseFloat(val) || 0;
    return '\u20ba' + n.toLocaleString('tr-TR', {minimumFractionDigits:2, maximumFractionDigits:2});
}

function downloadBlob(blob, filename) {
    var url = URL.createObjectURL(blob);
    var a   = document.createElement('a');
    a.href  = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    setTimeout(function() { document.body.removeChild(a); URL.revokeObjectURL(url); }, 200);
}

/* ==============================================
   CSV EXPORT (genel tablo)
============================================== */
function exportCSV() {
    var headers = ['Ad Soyad','E-posta','Telefon','Rezervasyon Kodu','Tur','Tarih','Kiþi','Tutar','Þehir','Ülke'];
    var rows    = Array.from(document.querySelectorAll('#tableBody tr')).filter(function(r){ return r.style.display !== 'none'; });

    var lines = [headers.join(',')];
    rows.forEach(function(r) {
        var cells = r.querySelectorAll('td');
        var name  = (r.dataset.name  || '').replace(/,/g,'');
        var email = (r.dataset.email || '').replace(/,/g,'');
        var code  = (r.dataset.code  || '').replace(/,/g,'');
        var tour  = (r.dataset.tour  || '').replace(/,/g,'');
        var date  = (r.dataset.date  || '');
        var pers  = (r.dataset.persons || '');
        var price = (r.dataset.price || '');
        var city  = '', country = '';
        /* Konum hücresi: son meta hücresinden al */
        var loc = cells[7];
        if (loc) {
            var divs = loc.querySelectorAll('div');
            city    = divs[0] ? divs[0].textContent.trim() : '';
            country = divs[1] ? divs[1].textContent.trim() : '';
        }
        lines.push([name,email,'',code,tour,date,pers,price,city,country].join(','));
    });

    var blob = new Blob(['\uFEFF' + lines.join('\n')], {type:'text/csv;charset=utf-8;'});
    var url  = URL.createObjectURL(blob);
    var a    = document.createElement('a');
    a.href   = url; a.download = 'rezervasyonlar.csv'; a.click();
    URL.revokeObjectURL(url);
    if (typeof showToast === 'function') showToast('CSV dýþa aktarýldý.', 's');
}

document.addEventListener('DOMContentLoaded', function() {
    Array.from(document.querySelectorAll('#tableBody tr')).forEach(function(row) {
        row.dataset.filteredOut = 'false';
    });
