using Microsoft.AspNetCore.Mvc;
using Project3Vitour.Dtos.ReservationDtos;
using Project3Vitour.Services.ReservationService;
using System.Globalization;
using System.Text;

namespace Project3Vitour.Controllers
{
    public class AdminSettingsController : Controller
    {
        private readonly IReservationService _reservationService;

        public AdminSettingsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            var values = await _reservationService.GetAllReservationAsync();
            return View(values);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAllExcel()
        {
            var reservations = await _reservationService.GetAllReservationAsync();
            var content = BuildExcelContent(reservations);
            var fileName = $"Vitour_Rezervasyon_Raporu_{DateTime.Now:yyyy-MM-dd}.xls";

            return File(Encoding.UTF8.GetBytes(content), "application/vnd.ms-excel; charset=utf-8", fileName);
        }

        [HttpGet]
        public async Task<IActionResult> ExportAllPdf()
        {
            var reservations = await _reservationService.GetAllReservationAsync();
            var fileName = $"Vitour_Rezervasyon_Raporu_{DateTime.Now:yyyy-MM-dd}.pdf";

            return File(SimplePdfGenerator.Generate(reservations), "application/pdf", fileName);
        }

        private static string BuildExcelContent(List<ResultReservationDto> reservations)
        {
            var tr = CultureInfo.GetCultureInfo("tr-TR");
            var sb = new StringBuilder();
            var totalRevenue = reservations.Sum(x => x.TotalPrice);

            sb.Append('\uFEFF');
            sb.AppendLine($"VITOUR REZERVASYON RAPORU\tOlusturma: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", tr)}");
            sb.AppendLine($"Toplam Kayit\t{reservations.Count}");
            sb.AppendLine($"Toplam Gelir\t{totalRevenue.ToString("N2", tr)} TL");
            sb.AppendLine();
            sb.AppendLine("Rezervasyon Kodu\tAd Soyad\tE-posta\tTelefon\tTur\tTarih\tKisi Sayisi\tToplam Tutar\tSehir\tUlke\tDurum");

            foreach (var item in reservations.OrderBy(x => x.ReservationDate))
            {
                sb.AppendLine(string.Join('\t', new[]
                {
                    SanitizeCell(item.ReservationCode),
                    SanitizeCell(item.NameSurname),
                    SanitizeCell(item.Email),
                    SanitizeCell(item.Phone),
                    SanitizeCell(item.TourTitle),
                    item.ReservationDate.ToString("dd.MM.yyyy", tr),
                    item.PersonCount.ToString(tr),
                    item.TotalPrice.ToString("N2", tr),
                    SanitizeCell(item.City),
                    SanitizeCell(item.Country),
                    GetReservationStatus(item.ReservationDate)
                }));
            }

            return sb.ToString();
        }

        private static string GetReservationStatus(DateTime reservationDate)
        {
            var today = DateTime.Today;
            if (reservationDate.Date == today)
            {
                return "Bugun";
            }

            if (reservationDate.Date > today)
            {
                return "Aktif";
            }

            return "Tamamlandi";
        }

        private static string SanitizeCell(string? value)
        {
            return (value ?? string.Empty)
                .Replace('\t', ' ')
                .Replace('\r', ' ')
                .Replace('\n', ' ');
        }

        private static class SimplePdfGenerator
        {
            public static byte[] Generate(List<ResultReservationDto> reservations)
            {
                var tr = CultureInfo.GetCultureInfo("tr-TR");
                var lines = new List<string>
                {
                    "Vitour Reservation Report",
                    $"Generated: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", tr)}",
                    $"Total Reservations: {reservations.Count}",
                    $"Total Revenue: {reservations.Sum(x => x.TotalPrice).ToString("N2", tr)} TL",
                    string.Empty
                };

                foreach (var item in reservations.OrderBy(x => x.ReservationDate))
                {
                    lines.Add(ToPdfLatin($"{item.ReservationCode} | {item.NameSurname} | {item.TourTitle}"));
                    lines.Add(ToPdfLatin($"{item.ReservationDate:dd.MM.yyyy} | {item.PersonCount} kisi | {item.TotalPrice.ToString("N2", tr)} TL | {GetReservationStatus(item.ReservationDate)}"));
                    lines.Add(ToPdfLatin($"{item.Email} | {item.Phone} | {item.City} / {item.Country}"));
                    lines.Add(string.Empty);
                }

                return BuildPdfDocument(lines);
            }

            private static byte[] BuildPdfDocument(List<string> lines)
            {
                const int linesPerPage = 38;
                var pages = lines
                    .Select((line, index) => new { line, index })
                    .GroupBy(x => x.index / linesPerPage)
                    .Select(g => g.Select(x => x.line).ToList())
                    .ToList();

                if (pages.Count == 0)
                {
                    pages.Add(new List<string> { "No reservation data found." });
                }

                using var stream = new MemoryStream();
                using var writer = new StreamWriter(stream, new UTF8Encoding(false));
                var offsets = new List<long>();

                void WriteObject(int id, string content)
                {
                    writer.Flush();
                    offsets.Add(stream.Position);
                    writer.Write($"{id} 0 obj\n{content}\nendobj\n");
                }

                writer.Write("%PDF-1.4\n");

                const int catalogId = 1;
                const int pagesId = 2;
                const int fontId = 3;
                var nextId = 4;
                var pageIds = new List<int>();
                var contentIds = new List<int>();

                for (var i = 0; i < pages.Count; i++)
                {
                    pageIds.Add(nextId++);
                    contentIds.Add(nextId++);
                }

                WriteObject(catalogId, "<< /Type /Catalog /Pages 2 0 R >>");
                WriteObject(pagesId, $"<< /Type /Pages /Count {pages.Count} /Kids [{string.Join(" ", pageIds.Select(id => $"{id} 0 R"))}] >>");
                WriteObject(fontId, "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

                for (var i = 0; i < pages.Count; i++)
                {
                    var content = BuildPageContent(pages[i]);
                    var length = Encoding.ASCII.GetByteCount(content);
                    WriteObject(pageIds[i], $"<< /Type /Page /Parent {pagesId} 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 {fontId} 0 R >> >> /Contents {contentIds[i]} 0 R >>");
                    WriteObject(contentIds[i], $"<< /Length {length} >>\nstream\n{content}\nendstream");
                }

                writer.Flush();
                var xrefStart = stream.Position;
                writer.Write($"xref\n0 {offsets.Count + 1}\n");
                writer.Write("0000000000 65535 f \n");

                foreach (var offset in offsets)
                {
                    writer.Write($"{offset:D10} 00000 n \n");
                }

                writer.Write("trailer\n");
                writer.Write($"<< /Size {offsets.Count + 1} /Root {catalogId} 0 R >>\n");
                writer.Write("startxref\n");
                writer.Write($"{xrefStart}\n");
                writer.Write("%%EOF");
                writer.Flush();

                return stream.ToArray();
            }

            private static string BuildPageContent(List<string> lines)
            {
                var sb = new StringBuilder();
                sb.AppendLine("BT");
                sb.AppendLine("/F1 11 Tf");
                sb.AppendLine("40 800 Td");
                sb.AppendLine("14 TL");

                foreach (var line in lines)
                {
                    sb.AppendLine($"({EscapePdfText(line)}) Tj");
                    sb.AppendLine("T*");
                }

                sb.AppendLine("ET");
                return sb.ToString();
            }

            private static string EscapePdfText(string value)
            {
                return value
                    .Replace("\\", "\\\\")
                    .Replace("(", "\\(")
                    .Replace(")", "\\)");
            }

            private static string ToPdfLatin(string value)
            {
                return value
                    .Replace("Ç", "C").Replace("ç", "c")
                    .Replace("Ğ", "G").Replace("ğ", "g")
                    .Replace("İ", "I").Replace("ı", "i")
                    .Replace("Ö", "O").Replace("ö", "o")
                    .Replace("Ş", "S").Replace("ş", "s")
                    .Replace("Ü", "U").Replace("ü", "u");
            }
        }
    }
}
