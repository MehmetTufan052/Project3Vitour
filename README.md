# 🌍 Vitour | Tur Rezervasyon ve Yönetim Sistemi

Vitour, kullanıcıların turları keşfedebildiği, detayları inceleyebildiği ve rezervasyon oluşturabildiği; yönetici tarafında ise tüm operasyonel süreçlerin **admin panel** üzerinden uçtan uca yönetilebildiği kapsamlı bir **Tur Rezervasyon ve Yönetim Sistemi**dir.

Bu proje, modern web geliştirme yaklaşımıyla oluşturulmuş olup kullanıcı deneyimi, çoklu dil desteği, raporlama, e-posta bildirimleri ve metin analizi gibi birçok özelliği bir araya getirmektedir.

---

## 📌 Proje Özeti

Vitour, hem son kullanıcıya hem de yöneticiye hitap eden full-stack bir platformdur.

### Kullanıcı tarafında:
- Tur listeleme ve detay sayfaları
- Filtreleme ve arama
- Online rezervasyon akışı
- Yorum ve puanlama sistemi
- Rehber ve tur içerik sayfaları
- Çok dilli kullanım deneyimi

### Admin panel tarafında:
- Tur, kategori, rehber ve içerik yönetimi
- Rezervasyon takibi ve durum güncelleme
- Yorum moderasyonu
- Mesaj ve müşteri geri bildirim yönetimi
- PDF ve Excel raporlama

---

## 🚀 Öne Çıkan Özellikler

| Kategori | Özellik |
|----------|---------|
| 🌐 Kullanıcı Arayüzü | Tur listeleme, tur detay, filtreleme, rezervasyon oluşturma |
| 🧾 Rezervasyon Sistemi | Online rezervasyon akışı ve rezervasyon detay yönetimi |
| 🌍 Çoklu Dil Desteği | Türkçe, İngilizce, Fransızca ve İspanyolca destek |
| 🛠️ Admin Panel | Tur, kategori, rehber, rezervasyon, mesaj ve yorum yönetimi |
| 📊 Dashboard | Gelir, rezervasyon ve popüler tur istatistikleri |
| 📄 Raporlama | PDF ve Excel export yapıları |
| 📧 Bildirim Sistemi | SMTP tabanlı rezervasyon detay e-postaları |
| 🧠 Akıllı Yorum Analizi | Hugging Face API ile Duygu Analizi (Sentiment Analysis) |

---

## 🧠 Akıllı Yorum Analizi

Projede, **Hugging Face API entegrasyonu** üzerinden çalışan bir **Duygu Analizi (Sentiment Analysis) yapısı** geliştirdim.

Bu yapı ile kullanıcı yorumları, **metin sınıflandırma** yaklaşımıyla:

- Olumlu
- Olumsuz
- Nötr

olarak kategorize edilmektedir.

Böylece yorum verilerinin yalnızca görüntülenmesi değil, aynı zamanda yönetim ve değerlendirme süreçlerinde kullanılabilecek anlamlı içgörülere dönüştürülmesi hedeflenmiştir.

---

## 📧 Bildirim ve İletişim Sistemi

Rezervasyon sonrası kullanıcıya rezervasyon detaylarını ileten bir **MailKit/SMTP tabanlı e-posta gönderim sistemi** geliştirilmiştir.

Bu sistem sayesinde:
- Rezervasyon bilgileri kullanıcıya otomatik gönderilir
- Süreç daha profesyonel hale gelir
- Kullanıcı deneyimi güçlenir

---

## 🌍 Çoklu Dil Desteği

Sistem, çok dilli kullanım senaryolarına uygun şekilde yapılandırılmıştır.

| Desteklenen Diller |
|--------------------|
| Türkçe |
| İngilizce |
| Fransızca |
| İspanyolca |

Dil yönetimi sayesinde kullanıcı deneyimi daha esnek ve erişilebilir hale getirilmiştir.

---

## 🛠️ Kullanılan Teknolojiler

| Teknoloji | Açıklama |
|----------|----------|
| ASP.NET Core MVC | Uygulamanın temel web mimarisi |
| MongoDB | NoSQL veritabanı yapısı |
| AutoMapper | Entity ve DTO dönüşümleri |
| MailKit / SMTP | E-posta gönderim sistemi |
| Translation | Çoklu dil desteği yapısı |
| PDF Export | PDF raporlama işlemleri |
| Excel Export | Excel raporlama işlemleri |
| Hugging Face API | NLP tabanlı Duygu Analizi (Sentiment Analysis) |

---

## 🏗️ Proje Mimarisi

Proje geliştirilirken modüler, sürdürülebilir ve geliştirilebilir bir mimari yapı hedeflenmiştir.

### Genel yapı:
- **MVC mimarisi**
- **DTO kullanımı**
- **Admin panel ve kullanıcı paneli ayrımı**
- **API tabanlı entegrasyonlar**
- **Raporlama ve bildirim servisleri**

---

## 📷 Uygulama Bölümleri

| Bölüm | Açıklama |
|------|----------|
| Ana Sayfa | Tur keşfi ve öne çıkan içerikler |
| Tur Listesi | Arama, filtreleme ve listeleme |
| Tur Detay | Tur bilgileri, içerikler ve yorumlar |
| Rezervasyon | Kullanıcı rezervasyon formu |
| Admin Panel | İçerik ve operasyon yönetimi |
| Dashboard | İstatistik ve özet veriler |
| Raporlama | PDF / Excel çıktı alma işlemleri |

---

## 📂 Projede Yer Alan Başlıca Modüller

- Tour Management
- Reservation Management
- Category Management
- Guide Management
- Review Management
- Message Management
- Dashboard Analytics
- Report Export
- Multilingual Support
- Sentiment Analysis Integration
- SMTP Mail Notification System

---

## 📸 Uygulama Görselleri

### 🌐 Kullanıcı Arayüzü

<table>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Tur Listesi" src="https://github.com/user-attachments/assets/f52bd35e-3f1f-4cdd-be1d-14832d42274c" />
      <br />
      <strong>Tur Listesi</strong>
    </td>
    <td align="center" valign="top">
      <img width="420" alt="Tur Detay" src="https://github.com/user-attachments/assets/c6a2fdb6-90a8-44c7-b07c-c537d27a6d82" />
      <br />
      <strong>Tur Detay</strong>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Tur Planı Sayfası" src="https://github.com/user-attachments/assets/3f046927-5d44-486e-b7b1-f150683ce6bd" />
      <br />
      <strong>Tur Planı Sayfası</strong>
    </td>
    <td align="center" valign="top">
      <img width="420" alt="Tur Yorumları Sayfası" src="https://github.com/user-attachments/assets/fcdd08c3-65ab-4776-98ca-97030509ea91" />
      <br />
      <strong>Tur Yorumları Sayfası</strong>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Tur Galerisi Sayfası" src="https://github.com/user-attachments/assets/95472422-6940-4e69-8e6a-3b3d4ba72748" />
      <br />
      <strong>Tur Galerisi Sayfası</strong>
    </td>
    <td align="center" valign="top">
      <img width="420" alt="Rezervasyon Sayfası" src="https://github.com/user-attachments/assets/aa8d0005-f7cc-4ff3-9679-2c1fb5d1b4f3" />
      <br />
      <strong>Rezervasyon Sayfası</strong>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Rezervasyon Mail Bildirimi" src="https://github.com/user-attachments/assets/15005209-2954-458c-ab78-9e064c78540e" />
      <br />
      <strong>Rezervasyon Mail Bildirimi</strong>
    </td>
    <td></td>
  </tr>
</table>

---

### 🛠️ Admin Panel

<table>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Admin Tur Paneli" src="https://github.com/user-attachments/assets/7ab1f90d-03ef-4f75-b247-b0bd92b8efbc" />
      <br />
      <strong>Admin Tur Paneli</strong>
    </td>
    <td align="center" valign="top">
      <img width="420" alt="Admin Rezervasyon Paneli" src="https://github.com/user-attachments/assets/d56c8ff0-bea3-495b-9595-f5e703682af9" />
      <br />
      <strong>Admin Rezervasyon Paneli</strong>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Admin Yorum Paneli" src="https://github.com/user-attachments/assets/4b57eb12-4051-4a7a-8a60-af902898f5af" />
      <br />
      <strong>Admin Yorum Paneli</strong>
    </td>
    <td align="center" valign="top">
      <img width="420" alt="Admin Raporlama Paneli" src="https://github.com/user-attachments/assets/aefe59d4-2ef3-4fdd-bd7f-20688a0c2b8f" />
      <br />
      <strong>Admin Raporlama Paneli</strong>
    </td>
  </tr>
  <tr>
    <td align="center" valign="top">
      <img width="420" alt="Admin Paneli Ayarlar" src="https://github.com/user-attachments/assets/c79fad0f-2d4c-4ccf-b228-8870d26f1a32" />
      <br />
      <strong>Admin Paneli Ayarlar</strong>
    </td>
    <td></td>
  </tr>
</table>
