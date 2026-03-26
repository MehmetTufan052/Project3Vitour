using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Project3Vitour.Dtos.ReservationDtos;
using Project3Vitour.Settings;

namespace Project3Vitour.Services.MailService
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendReservationCreatedEmailAsync(CreateReservationDto reservation, CancellationToken cancellationToken = default)
        {
            if (!_smtpSettings.Enabled)
            {
                throw new InvalidOperationException("SMTP ayarlari aktif degil.");
            }

            ValidateSettings();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpSettings.FromName, _smtpSettings.FromEmail));
            message.To.Add(MailboxAddress.Parse(reservation.Email));
            message.Subject = $"Rezervasyonunuz alindi - Kod: {reservation.ReservationCode}";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildReservationMailBody(reservation)
            };

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            var socketOptions = _smtpSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;

            await client.ConnectAsync(_smtpSettings.Host, _smtpSettings.Port, socketOptions, cancellationToken);

            if (!string.IsNullOrWhiteSpace(_smtpSettings.UserName))
            {
                await client.AuthenticateAsync(_smtpSettings.UserName, _smtpSettings.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }

        private void ValidateSettings()
        {
            if (string.IsNullOrWhiteSpace(_smtpSettings.Host) ||
                _smtpSettings.Port <= 0 ||
                string.IsNullOrWhiteSpace(_smtpSettings.FromEmail))
            {
                throw new InvalidOperationException("SMTP ayarlari eksik. Host, Port ve FromEmail degerlerini kontrol edin.");
            }
        }

        private static string BuildReservationMailBody(CreateReservationDto reservation)
        {
            return $@"
                <div style='font-family:Arial,sans-serif;line-height:1.6;color:#1f2937;'>
                    <h2 style='color:#166534;'>Rezervasyonunuz olusturuldu</h2>
                    <p>Merhaba {reservation.NameSurname},</p>
                    <p>Rezervasyonunuz basariyla alinmistir. Detaylariniz asagidadir:</p>
                    <table style='border-collapse:collapse;width:100%;max-width:600px;'>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Rezervasyon Kodu</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.ReservationCode}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Ad Soyad</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.NameSurname}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>E-posta</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.Email}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Telefon</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.Phone}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Sehir / Ulke</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.City} / {reservation.Country}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Tur Kodu</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.TourId}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Rezervasyon Tarihi</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.ReservationDate:dd.MM.yyyy}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Kisi Sayisi</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.PersonCount}</td></tr>
                        <tr><td style='padding:8px;border:1px solid #e5e7eb;'><strong>Toplam Tutar</strong></td><td style='padding:8px;border:1px solid #e5e7eb;'>{reservation.TotalPrice:N2} TL</td></tr>
                    </table>
                    <p style='margin-top:16px;'>Bizi tercih ettiginiz icin tesekkur ederiz.</p>
                    <p>Vitour</p>
                </div>";
        }
    }
}
