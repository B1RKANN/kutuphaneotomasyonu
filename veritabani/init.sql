CREATE TABLE kategoriler (
    id SERIAL PRIMARY KEY,
    ad VARCHAR(100) NOT NULL,
    aciklama VARCHAR(250)
);

CREATE TABLE yazarlar (
    id SERIAL PRIMARY KEY,
    ad VARCHAR(100) NOT NULL,
    soyad VARCHAR(100) NOT NULL,
    ulke VARCHAR(100),
    dogum_yili INT
);

CREATE TABLE yayinevleri (
    id SERIAL PRIMARY KEY,
    ad VARCHAR(150) NOT NULL,
    telefon VARCHAR(20),
    adres VARCHAR(250)
);

CREATE TABLE kitaplar (
    id SERIAL PRIMARY KEY,
    isbn VARCHAR(20) NOT NULL UNIQUE,
    ad VARCHAR(200) NOT NULL,
    yazar_id INT NOT NULL REFERENCES yazarlar(id),
    kategori_id INT NOT NULL REFERENCES kategoriler(id),
    yayinevi_id INT NOT NULL REFERENCES yayinevleri(id),
    basim_yili INT,
    sayfa_sayisi INT,
    stok INT NOT NULL DEFAULT 1,
    mevcut INT NOT NULL DEFAULT 1,
    raf_no VARCHAR(20)
);

CREATE TABLE uyeler (
    id SERIAL PRIMARY KEY,
    tc_no VARCHAR(11) NOT NULL UNIQUE,
    ad VARCHAR(100) NOT NULL,
    soyad VARCHAR(100) NOT NULL,
    telefon VARCHAR(20),
    eposta VARCHAR(150),
    adres VARCHAR(250),
    kayit_tarihi DATE NOT NULL DEFAULT CURRENT_DATE,
    aktif BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE personeller (
    id SERIAL PRIMARY KEY,
    tc_no VARCHAR(11) NOT NULL UNIQUE,
    ad VARCHAR(100) NOT NULL,
    soyad VARCHAR(100) NOT NULL,
    telefon VARCHAR(20),
    eposta VARCHAR(150),
    kullanici_adi VARCHAR(50) NOT NULL UNIQUE,
    sifre VARCHAR(50) NOT NULL,
    rol VARCHAR(20) NOT NULL
);

CREATE TABLE odunc_islemleri (
    id SERIAL PRIMARY KEY,
    kitap_id INT NOT NULL REFERENCES kitaplar(id),
    uye_id INT NOT NULL REFERENCES uyeler(id),
    personel_id INT NOT NULL REFERENCES personeller(id),
    odunc_tarihi DATE NOT NULL,
    son_teslim_tarihi DATE NOT NULL,
    iade_tarihi DATE
);

CREATE TABLE cezalar (
    id SERIAL PRIMARY KEY,
    odunc_id INT NOT NULL REFERENCES odunc_islemleri(id),
    uye_id INT NOT NULL REFERENCES uyeler(id),
    gecikme_gunu INT NOT NULL,
    tutar NUMERIC(10,2) NOT NULL,
    odendi BOOLEAN NOT NULL DEFAULT FALSE,
    odeme_tarihi DATE
);

INSERT INTO kategoriler (ad, aciklama) VALUES
('Roman', 'Yerli ve yabancı romanlar'),
('Bilim', 'Bilimsel ve popüler bilim kitapları'),
('Tarih', 'Tarih kitapları'),
('Şiir', 'Şiir kitapları'),
('Yazılım', 'Bilgisayar ve programlama kitapları');

INSERT INTO yazarlar (ad, soyad, ulke, dogum_yili) VALUES
('Orhan', 'Pamuk', 'Türkiye', 1952),
('Sabahattin', 'Ali', 'Türkiye', 1907),
('George', 'Orwell', 'İngiltere', 1903),
('Stephen', 'Hawking', 'İngiltere', 1942),
('Robert', 'Martin', 'ABD', 1952);

INSERT INTO yayinevleri (ad, telefon, adres) VALUES
('Yapı Kredi Yayınları', '02122522700', 'İstanbul'),
('Can Yayınları', '02122525675', 'İstanbul'),
('Prentice Hall', '18005550000', 'New Jersey');

INSERT INTO kitaplar (isbn, ad, yazar_id, kategori_id, yayinevi_id, basim_yili, sayfa_sayisi, stok, mevcut, raf_no) VALUES
('9789750800010', 'Kürk Mantolu Madonna', 2, 1, 1, 1943, 160, 3, 2, 'A-01'),
('9789750800027', 'İçimizdeki Şeytan', 2, 1, 1, 1940, 272, 2, 2, 'A-02'),
('9789750800034', 'Masumiyet Müzesi', 1, 1, 1, 2008, 592, 2, 2, 'A-03'),
('9789750800041', 'Benim Adım Kırmızı', 1, 1, 1, 1998, 472, 1, 1, 'A-04'),
('9789750800058', '1984', 3, 1, 2, 1949, 352, 4, 3, 'B-01'),
('9789750800065', 'Hayvan Çiftliği', 3, 1, 2, 1945, 152, 2, 2, 'B-02'),
('9789750800072', 'Zamanın Kısa Tarihi', 4, 2, 2, 1988, 256, 2, 2, 'C-01'),
('9780132350884', 'Clean Code', 5, 5, 3, 2008, 464, 2, 2, 'D-01');

INSERT INTO uyeler (tc_no, ad, soyad, telefon, eposta, adres, kayit_tarihi, aktif) VALUES
('11111111111', 'Ahmet', 'Yılmaz', '05321111111', 'ahmet@mail.com', 'Ankara', CURRENT_DATE - 100, TRUE),
('22222222222', 'Ayşe', 'Demir', '05322222222', 'ayse@mail.com', 'İstanbul', CURRENT_DATE - 60, TRUE),
('33333333333', 'Mehmet', 'Kaya', '05323333333', 'mehmet@mail.com', 'İzmir', CURRENT_DATE - 30, TRUE),
('44444444444', 'Zeynep', 'Çelik', '05324444444', 'zeynep@mail.com', 'Bursa', CURRENT_DATE - 10, FALSE);

INSERT INTO personeller (tc_no, ad, soyad, telefon, eposta, kullanici_adi, sifre, rol) VALUES
('55555555555', 'Ali', 'Veli', '05325555555', 'admin@kutuphane.com', 'admin', 'admin123', 'Yönetici'),
('66666666666', 'Fatma', 'Şahin', '05326666666', 'fatma@kutuphane.com', 'gorevli', '1234', 'Görevli');

INSERT INTO odunc_islemleri (kitap_id, uye_id, personel_id, odunc_tarihi, son_teslim_tarihi, iade_tarihi) VALUES
(1, 1, 1, CURRENT_DATE - 5, CURRENT_DATE + 10, NULL),
(5, 2, 2, CURRENT_DATE - 20, CURRENT_DATE - 5, NULL),
(7, 3, 1, CURRENT_DATE - 40, CURRENT_DATE - 25, CURRENT_DATE - 20);

INSERT INTO cezalar (odunc_id, uye_id, gecikme_gunu, tutar, odendi, odeme_tarihi) VALUES
(3, 3, 5, 25.00, TRUE, CURRENT_DATE - 20);
