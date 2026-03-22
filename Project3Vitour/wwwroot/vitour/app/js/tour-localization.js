(function () {
    "use strict";

    var storageKey = "vitour-language";
    var supportedLanguages = {
        tr: { label: "Türkçe" },
        en: { label: "English" },
        fr: { label: "Français" },
        es: { label: "Español" }
    };

    var exactTranslations = {
        "Türkçe": { tr: "Türkçe", en: "English", fr: "Français", es: "Español" },
        "English": { tr: "Türkçe", en: "English", fr: "Français", es: "Español" },
        "Français": { tr: "Türkçe", en: "English", fr: "Français", es: "Español" },
        "Español": { tr: "Türkçe", en: "English", fr: "Français", es: "Español" },
        "Ana Sayfa": { tr: "Ana Sayfa", en: "Home", fr: "Accueil", es: "Inicio" },
        "Güncel Turlar": { tr: "Güncel Turlar", en: "Current Tours", fr: "Circuits Actuels", es: "Tours Actuales" },
        "Tur Paketleri": { tr: "Tur Paketleri", en: "Tour Packages", fr: "Forfaits Circuits", es: "Paquetes Turísticos" },
        "Tur Ara": { tr: "Tur Ara", en: "Search Tour", fr: "Rechercher un Circuit", es: "Buscar Tour" },
        "Süre": { tr: "Süre", en: "Duration", fr: "Durée", es: "Duración" },
        "Tüm Süreler": { tr: "Tüm Süreler", en: "All Durations", fr: "Toutes les Durées", es: "Todas las Duraciones" },
        "1 – 3 Gece": { tr: "1 – 3 Gece", en: "1 – 3 Nights", fr: "1 – 3 Nuits", es: "1 – 3 Noches" },
        "4 – 7 Gece": { tr: "4 – 7 Gece", en: "4 – 7 Nights", fr: "4 – 7 Nuits", es: "4 – 7 Noches" },
        "8 – 14 Gece": { tr: "8 – 14 Gece", en: "8 – 14 Nights", fr: "8 – 14 Nuits", es: "8 – 14 Noches" },
        "15+ Gece": { tr: "15+ Gece", en: "15+ Nights", fr: "15+ Nuits", es: "15+ Noches" },
        "Fiyat": { tr: "Fiyat", en: "Price", fr: "Prix", es: "Precio" },
        "Fiyata Göre": { tr: "Fiyata Göre", en: "By Price", fr: "Par Prix", es: "Por Precio" },
        "En Düşük Önce": { tr: "En Düşük Önce", en: "Lowest First", fr: "Le Moins Cher d'Abord", es: "Menor Precio Primero" },
        "En Yüksek Önce": { tr: "En Yüksek Önce", en: "Highest First", fr: "Le Plus Cher d'Abord", es: "Precio Más Alto Primero" },
        "₺30.000 ve üzeri": { tr: "₺30.000 ve üzeri", en: "₺30,000 and above", fr: "₺30 000 et plus", es: "₺30.000 o más" },
        "Ara": { tr: "Ara", en: "Search", fr: "Rechercher", es: "Buscar" },
        "Featured": { tr: "Öne Çıkan", en: "Featured", fr: "En Vedette", es: "Destacado" },
        "Price": { tr: "Fiyat", en: "Price", fr: "Prix", es: "Precio" },
        "New": { tr: "Yeni", en: "New", fr: "Nouveau", es: "Nuevo" },
        "Recently Added": { tr: "Son Eklenenler", en: "Recently Added", fr: "Récemment Ajouté", es: "Añadido Recientemente" },
        "Bilgi": { tr: "Bilgi", en: "Information", fr: "Informations", es: "Información" },
        "Tur Planı": { tr: "Tur Planı", en: "Tour Plan", fr: "Plan du Circuit", es: "Plan del Tour" },
        "Konum Paylaşımı": { tr: "Konum Paylaşımı", en: "Location Sharing", fr: "Partage de Localisation", es: "Compartir Ubicación" },
        "Yorumlar": { tr: "Yorumlar", en: "Reviews", fr: "Avis", es: "Reseñas" },
        "Fotoğraf Galerisi": { tr: "Fotoğraf Galerisi", en: "Photo Gallery", fr: "Galerie Photo", es: "Galería de Fotos" },
        "Tur Plani :": { tr: "Tur Planı :", en: "Tour Plan :", fr: "Plan du Circuit :", es: "Plan del Tour :" },
        "Gonul Rahatligiyla Rezervasyon Yapin": { tr: "Gönül Rahatlığıyla Rezervasyon Yapın", en: "Book with Peace of Mind", fr: "Réservez en Toute Sérénité", es: "Reserva con Tranquilidad" },
        "7/24 musteri destegi": { tr: "7/24 müşteri desteği", en: "24/7 customer support", fr: "Assistance client 24h/24 7j/7", es: "Atención al cliente 24/7" },
        "Ozenle secilmis tur ve aktiviteler": { tr: "Özenle seçilmiş tur ve aktiviteler", en: "Carefully selected tours and activities", fr: "Circuits et activités soigneusement sélectionnés", es: "Tours y actividades cuidadosamente seleccionados" },
        "Ucretsiz seyahat sigortasi": { tr: "Ücretsiz seyahat sigortası", en: "Free travel insurance", fr: "Assurance voyage gratuite", es: "Seguro de viaje gratuito" },
        "Sorunsuz en iyi fiyat garantisi": { tr: "Sorunsuz en iyi fiyat garantisi", en: "Best price guarantee without hassle", fr: "Garantie du meilleur prix sans souci", es: "Garantía del mejor precio sin complicaciones" },
        "Randevu Al": { tr: "Randevu Al", en: "Book Now", fr: "Réserver", es: "Reservar" },
        "Bu tur icin henuz gunluk plan bilgisi eklenmemis.": { tr: "Bu tur için henüz günlük plan bilgisi eklenmemiş.", en: "A daily itinerary has not been added for this tour yet.", fr: "Aucun programme journalier n'a encore été ajouté pour ce circuit.", es: "Aún no se ha añadido un itinerario diario para este tour." },
        "Bu tur icin henuz location share bilgisi eklenmemis.": { tr: "Bu tur için henüz konum bilgisi eklenmemiş.", en: "Location information has not been added for this tour yet.", fr: "Les informations de localisation n'ont pas encore été ajoutées pour ce circuit.", es: "La información de ubicación aún no se ha añadido para este tour." },
        "View the City Walls": { tr: "Şehir Surlarını Görün", en: "View the City Walls", fr: "Voir les Remparts de la Ville", es: "Ver las Murallas de la Ciudad" },
        "Hiking in the forest": { tr: "Ormanda Doğa Yürüyüşü", en: "Hiking in the forest", fr: "Randonnée en forêt", es: "Senderismo en el bosque" },
        "Discover the famous view point \"The Lark\"": { tr: "\"The Lark\" adlı ünlü manzara noktasını keşfedin", en: "Discover the famous view point \"The Lark\"", fr: "Découvrez le célèbre point de vue \"The Lark\"", es: "Descubre el famoso mirador \"The Lark\"" },
        "Sunset on the cruise": { tr: "Teknede gün batımı", en: "Sunset on the cruise", fr: "Coucher de soleil en croisière", es: "Atardecer en el crucero" },
        "Customer Review": { tr: "Müşteri Değerlendirmesi", en: "Customer Review", fr: "Avis des Clients", es: "Opinión del Cliente" },
        "overall Ratings": { tr: "Genel Puan", en: "overall Ratings", fr: "Note Générale", es: "Calificación General" },
        "Out of 5": { tr: "5 Üzerinden", en: "Out of 5", fr: "Sur 5", es: "De 5" },
        "Comfort": { tr: "Konfor", en: "Comfort", fr: "Confort", es: "Comodidad" },
        "Client’s Review": { tr: "Misafir Yorumları", en: "Client’s Review", fr: "Avis des Clients", es: "Reseñas de Clientes" },
        "leave a comment": { tr: "Yorum Bırak", en: "Leave a Comment", fr: "Laisser un Commentaire", es: "Dejar un Comentario" },
        "Value for Money*": { tr: "Fiyat Performans*", en: "Value for Money*", fr: "Rapport Qualité Prix*", es: "Relación Calidad Precio*" },
        "Destination*": { tr: "Destinasyon*", en: "Destination*", fr: "Destination*", es: "Destino*" },
        "Accommodation*": { tr: "Konaklama*", en: "Accommodation*", fr: "Hébergement*", es: "Alojamiento*" },
        "Transport*": { tr: "Ulaşım*", en: "Transport*", fr: "Transport*", es: "Transporte*" },
        "Ad Soyad": { tr: "Ad Soyad", en: "Full Name", fr: "Nom Complet", es: "Nombre Completo" },
        "Rezervasyondaki E-mail": { tr: "Rezervasyondaki E-posta", en: "Reservation E-mail", fr: "E-mail de Réservation", es: "Correo de la Reserva" },
        "Yorumunuzu yazın": { tr: "Yorumunuzu yazın", en: "Write your review", fr: "Écrivez votre avis", es: "Escribe tu reseña" },
        "Şartlar, iade ve gizlilik politikasını kabul ediyorum.": { tr: "Şartlar, iade ve gizlilik politikasını kabul ediyorum.", en: "I accept the terms, refund policy and privacy policy.", fr: "J'accepte les conditions, la politique de remboursement et la politique de confidentialité.", es: "Acepto los términos, la política de reembolso y la política de privacidad." },
        "Post Comment": { tr: "Yorumu Gönder", en: "Post Comment", fr: "Publier le Commentaire", es: "Publicar Comentario" },
        "Son Eklenen Turlar": { tr: "Son Eklenen Turlar", en: "Recently Added Tours", fr: "Circuits Récemment Ajoutés", es: "Tours Añadidos Recientemente" },
        "Tur Galerisi": { tr: "Tur Galerisi", en: "Tour Gallery", fr: "Galerie du Circuit", es: "Galería del Tour" },
        "Ready to adventure and enjoy natural": { tr: "Maceraya atılmaya ve doğanın tadını çıkarmaya hazır mısınız?", en: "Ready to adventure and enjoy nature?", fr: "Prêt pour l'aventure et pour profiter de la nature ?", es: "¿Listo para la aventura y para disfrutar de la naturaleza?" },
        "Lorem ipsum dolor sit amet, consectetur notted adipisicin": { tr: "Yeni rotalar ve unutulmaz deneyimler için bizimle yola çıkın.", en: "Set out with us for new routes and unforgettable experiences.", fr: "Partez avec nous pour de nouveaux itinéraires et des expériences inoubliables.", es: "Emprende el viaje con nosotros para nuevas rutas y experiencias inolvidables." },
        "Let,s get started": { tr: "Hemen Başlayalım", en: "Let's get started", fr: "Commençons", es: "Empecemos" },
        "The world’s first and largest digital market for crypto collectibles and non-fungible": { tr: "Dünyanın dört bir yanındaki gezginler için özel turlar ve eşsiz deneyimler sunuyoruz.", en: "We offer special tours and unique experiences for travelers around the world.", fr: "Nous proposons des circuits spéciaux et des expériences uniques aux voyageurs du monde entier.", es: "Ofrecemos tours especiales y experiencias únicas para viajeros de todo el mundo." },
        "Rezervasyon Yap": { tr: "Rezervasyon Yap", en: "Make a Reservation", fr: "Faire une Réservation", es: "Hacer una Reserva" },
        "Bizi Takip Edin :": { tr: "Bizi Takip Edin :", en: "Follow Us :", fr: "Suivez-nous :", es: "Síguenos :" },
        "Kültür Turları": { tr: "Kültür Turları", en: "Cultural Tours", fr: "Circuits Culturels", es: "Tours Culturales" },
        "Vizesiz Turlar": { tr: "Vizesiz Turlar", en: "Visa-Free Tours", fr: "Circuits Sans Visa", es: "Tours Sin Visa" },
        "Kıbrıs Turları": { tr: "Kıbrıs Turları", en: "Cyprus Tours", fr: "Circuits à Chypre", es: "Tours a Chipre" },
        "Rehberlerimiz": { tr: "Rehberlerimiz", en: "Our Guides", fr: "Nos Guides", es: "Nuestros Guías" },
        "Bize Yazın": { tr: "Bize Yazın", en: "Contact Us", fr: "Écrivez-nous", es: "Escríbenos" },
        "Aranacak kelime": { tr: "Aranacak kelime", en: "Search term", fr: "Mot à rechercher", es: "Palabra a buscar" },
        "Giriş Yap": { tr: "Giriş Yap", en: "Sign In", fr: "Se Connecter", es: "Iniciar Sesión" },
        "Dünyayı keşfetmeniz için güvenli, konforlu ve profesyonel tur hizmetleri sunuyoruz.": { tr: "Dünyayı keşfetmeniz için güvenli, konforlu ve profesyonel tur hizmetleri sunuyoruz.", en: "We provide safe, comfortable and professional tour services so you can explore the world.", fr: "Nous proposons des services de circuits sûrs, confortables et professionnels pour vous permettre de découvrir le monde.", es: "Ofrecemos servicios turísticos seguros, cómodos y profesionales para que descubras el mundo." },
        "Kurumsal": { tr: "Kurumsal", en: "Corporate", fr: "Entreprise", es: "Corporativo" },
        "Hakkımızda": { tr: "Hakkımızda", en: "About Us", fr: "À Propos", es: "Sobre Nosotros" },
        "Galeri": { tr: "Galeri", en: "Gallery", fr: "Galerie", es: "Galería" },
        "Ekibimiz": { tr: "Ekibimiz", en: "Our Team", fr: "Notre Équipe", es: "Nuestro Equipo" },
        "Blog Yazıları": { tr: "Blog Yazıları", en: "Blog Posts", fr: "Articles de Blog", es: "Artículos del Blog" },
        "İletişim": { tr: "İletişim", en: "Contact", fr: "Contact", es: "Contacto" },
        "Bülten Aboneliği": { tr: "Bülten Aboneliği", en: "Newsletter Subscription", fr: "Abonnement à la Newsletter", es: "Suscripción al Boletín" },
        "Tüm şartlar ve gizlilik politikamızı kabul ediyorum": { tr: "Tüm şartlar ve gizlilik politikamızı kabul ediyorum", en: "I accept all terms and our privacy policy", fr: "J'accepte toutes les conditions et notre politique de confidentialité", es: "Acepto todos los términos y nuestra política de privacidad" },
        "Tüm Hakları Saklıdır.": { tr: "Tüm Hakları Saklıdır.", en: "All Rights Reserved.", fr: "Tous Droits Réservés.", es: "Todos los Derechos Reservados." }
    };

    var regexTranslations = [
        {
            pattern: /Tur adı veya şehir\.\.\./g,
            values: {
                tr: "Tur adı veya şehir...",
                en: "Tour name or city...",
                fr: "Nom du circuit ou ville...",
                es: "Nombre del tour o ciudad..."
            }
        },
        {
            pattern: /Gösteriliyor:\s*(\d+)\s*of\s*(\d+)\s*Results/gi,
            values: {
                tr: function (_, shown, total) { return "Gösteriliyor: " + shown + " / " + total + " sonuç"; },
                en: function (_, shown, total) { return "Showing: " + shown + " of " + total + " results"; },
                fr: function (_, shown, total) { return "Affichage : " + shown + " sur " + total + " résultats"; },
                es: function (_, shown, total) { return "Mostrando: " + shown + " de " + total + " resultados"; }
            }
        },
        {
            pattern: /(\d+)\s*Gün\s*-\s*(\d+)\s*Gece/gi,
            values: {
                tr: function (_, day, night) { return day + " Gün - " + night + " Gece"; },
                en: function (_, day, night) { return day + " Days - " + night + " Nights"; },
                fr: function (_, day, night) { return day + " Jours - " + night + " Nuits"; },
                es: function (_, day, night) { return day + " Días - " + night + " Noches"; }
            }
        },
        {
            pattern: /\((\d+)\s*Değerlendirme\)/gi,
            values: {
                tr: function (_, count) { return "(" + count + " Değerlendirme)"; },
                en: function (_, count) { return "(" + count + " Reviews)"; },
                fr: function (_, count) { return "(" + count + " Avis)"; },
                es: function (_, count) { return "(" + count + " Reseñas)"; }
            }
        },
        {
            pattern: /(\d+)\s*Gün/gi,
            values: {
                tr: function (_, day) { return day + " Gün"; },
                en: function (_, day) { return day + " Days"; },
                fr: function (_, day) { return day + " Jours"; },
                es: function (_, day) { return day + " Días"; }
            }
        },
        {
            pattern: /Fiyat:\s*/gi,
            values: {
                tr: "Fiyat: ",
                en: "Price: ",
                fr: "Prix : ",
                es: "Precio: "
            }
        },
        {
            pattern: /Max Guests:\s*(\d+)/gi,
            values: {
                tr: function (_, count) { return "Maksimum Misafir: " + count; },
                en: function (_, count) { return "Max Guests: " + count; },
                fr: function (_, count) { return "Nombre Max. de Voyageurs : " + count; },
                es: function (_, count) { return "Máximo de Huéspedes: " + count; }
            }
        },
        {
            pattern: /\((\d+)\s*Yorum\)/gi,
            values: {
                tr: function (_, count) { return "(" + count + " Yorum)"; },
                en: function (_, count) { return "(" + count + " Reviews)"; },
                fr: function (_, count) { return "(" + count + " Avis)"; },
                es: function (_, count) { return "(" + count + " Reseñas)"; }
            }
        },
        {
            pattern: /(\d+)\s*Reviews/gi,
            values: {
                tr: function (_, count) { return count + " Yorum"; },
                en: function (_, count) { return count + " Reviews"; },
                fr: function (_, count) { return count + " Avis"; },
                es: function (_, count) { return count + " Reseñas"; }
            }
        },
        {
            pattern: /Rating\s*([0-9.]+)/gi,
            values: {
                tr: function (_, score) { return "Puan " + score; },
                en: function (_, score) { return "Rating " + score; },
                fr: function (_, score) { return "Note " + score; },
                es: function (_, score) { return "Puntuación " + score; }
            }
        },
        {
            pattern: /\(([0-9.]+)\s*out of 5\)/gi,
            values: {
                tr: function (_, score) { return "(" + score + " / 5)"; },
                en: function (_, score) { return "(" + score + " out of 5)"; },
                fr: function (_, score) { return "(" + score + " sur 5)"; },
                es: function (_, score) { return "(" + score + " de 5)"; }
            }
        },
        {
            pattern: /([0-9.,]+)\s*₺\s*'dan başlayan fiyatlarla/gi,
            values: {
                tr: function (_, price) { return price + " ₺'dan başlayan fiyatlarla"; },
                en: function (_, price) { return "Starting from " + price + " ₺"; },
                fr: function (_, price) { return "À partir de " + price + " ₺"; },
                es: function (_, price) { return "Desde " + price + " ₺"; }
            }
        }
    ];

    function translateCoreText(coreText, lang) {
        if (!coreText) {
            return coreText;
        }

        if (exactTranslations[coreText] && exactTranslations[coreText][lang]) {
            return exactTranslations[coreText][lang];
        }

        var translatedText = coreText;

        Object.keys(exactTranslations)
            .sort(function (a, b) { return b.length - a.length; })
            .forEach(function (key) {
                var translation = exactTranslations[key] && exactTranslations[key][lang];
                if (!translation || translatedText.indexOf(key) === -1) {
                    return;
                }

                translatedText = translatedText.split(key).join(translation);
            });

        for (var i = 0; i < regexTranslations.length; i++) {
            var rule = regexTranslations[i];
            var languageValue = rule.values[lang];
            translatedText = typeof languageValue === "function"
                ? translatedText.replace(rule.pattern, languageValue)
                : translatedText.replace(rule.pattern, languageValue);
        }

        return translatedText;
    }

    function translateText(text, lang) {
        if (typeof text !== "string" || !text.trim()) {
            return text;
        }

        var leading = text.match(/^\s*/);
        var trailing = text.match(/\s*$/);
        var prefix = leading ? leading[0] : "";
        var suffix = trailing ? trailing[0] : "";
        var core = text.trim();

        return prefix + translateCoreText(core, lang) + suffix;
    }

    function translateTextNodes(lang) {
        var walker = document.createTreeWalker(document.body, NodeFilter.SHOW_TEXT, {
            acceptNode: function (node) {
                if (!node.parentElement) {
                    return NodeFilter.FILTER_REJECT;
                }

                if (node.parentElement.closest("[data-i18n-skip='true']")) {
                    return NodeFilter.FILTER_REJECT;
                }

                var tagName = node.parentElement.tagName;
                if (tagName === "SCRIPT" || tagName === "STYLE" || tagName === "NOSCRIPT" || tagName === "TEXTAREA") {
                    return NodeFilter.FILTER_REJECT;
                }

                return node.textContent && node.textContent.trim()
                    ? NodeFilter.FILTER_ACCEPT
                    : NodeFilter.FILTER_REJECT;
            }
        });

        var node;
        while ((node = walker.nextNode())) {
            if (typeof node.__i18nOriginalText === "undefined") {
                node.__i18nOriginalText = node.textContent;
            }

            node.textContent = translateText(node.__i18nOriginalText, lang);
        }
    }

    function translateAttributes(lang) {
        var attrs = ["placeholder", "title", "aria-label"];
        document.querySelectorAll("[placeholder], [title], [aria-label]").forEach(function (element) {
            if (element.closest("[data-i18n-skip='true']")) {
                return;
            }

            attrs.forEach(function (attr) {
                var value = element.getAttribute(attr);
                if (!value) {
                    return;
                }

                var originalKey = "i18nOriginal" + attr.charAt(0).toUpperCase() + attr.slice(1).replace(/-([a-z])/g, function (_, letter) {
                    return letter.toUpperCase();
                });

                if (!element.dataset[originalKey]) {
                    element.dataset[originalKey] = value;
                }

                element.setAttribute(attr, translateText(element.dataset[originalKey], lang));
            });
        });
    }

    function updateLanguageSelector(lang) {
        var currentLabel = document.getElementById("selected-language-label");
        if (currentLabel && supportedLanguages[lang]) {
            currentLabel.textContent = supportedLanguages[lang].label;
        }

        document.querySelectorAll("[data-language-option]").forEach(function (option) {
            var isSelected = option.getAttribute("data-language-option") === lang;
            option.classList.toggle("selected", isSelected);
            option.classList.toggle("focus", isSelected);
        });
    }

    function applyLanguage(lang) {
        var selectedLanguage = supportedLanguages[lang] ? lang : "tr";

        document.documentElement.lang = selectedLanguage;
        document.documentElement.setAttribute("xml:lang", selectedLanguage);
        document.body.setAttribute("data-language", selectedLanguage);

        translateTextNodes(selectedLanguage);
        translateAttributes(selectedLanguage);
        updateLanguageSelector(selectedLanguage);

        try {
            localStorage.setItem(storageKey, selectedLanguage);
        } catch (error) {
        }
    }

    function bindLanguageOptions() {
        document.querySelectorAll("[data-language-option]").forEach(function (option) {
            option.addEventListener("click", function (event) {
                event.preventDefault();
                var lang = option.getAttribute("data-language-option");
                try {
                    localStorage.setItem(storageKey, lang);
                } catch (error) {
                }

                document.cookie = "site_lang=" + lang + "; path=/; max-age=31536000; SameSite=Lax";

                var url = new URL(window.location.href);
                url.searchParams.set("lang", lang);
                window.location.href = url.toString();
            });
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        bindLanguageOptions();

        var urlLanguage = new URL(window.location.href).searchParams.get("lang");
        var savedLanguage = "tr";
        try {
            savedLanguage = localStorage.getItem(storageKey) || "tr";
        } catch (error) {
        }

        applyLanguage(urlLanguage || savedLanguage);
    });
})();
