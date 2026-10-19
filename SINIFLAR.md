# Kütüphane Otomasyonu – Sınıflar ve Özellikleri

Mimari 4 katmandan oluşur: **Modeller** (veri nesneleri) → **Veri** (PostgreSQL erişimi) → **Servisler** (iş kuralları) → **Formlar** (Windows Forms arayüzü).

---

## 1. Modeller

### Kisi (abstract)
Üye ve personelin ortak üst sınıfı.
| Üye | Tür | Açıklama |
|---|---|---|
| Id | int | Birincil anahtar |
| TcNo | string | 11 haneli TC kimlik no |
| Ad, Soyad | string | Ad ve soyad |
| Telefon, Eposta | string | İletişim bilgileri |
| AdSoyad | string (salt okunur) | Ad + " " + Soyad |
| KisiTuru() | abstract string | Alt sınıf kendi türünü döndürür |
| ToString() | string | AdSoyad döndürür (ComboBox'larda görünür) |

### Uye : Kisi
| Üye | Tür | Açıklama |
|---|---|---|
| Adres | string | Üyenin adresi |
| KayitTarihi | DateTime | Kütüphaneye kayıt tarihi |
| Aktif | bool | Pasif üye kitap alamaz |
| KisiTuru() | string | "Üye" döndürür |

### Personel : Kisi
| Üye | Tür | Açıklama |
|---|---|---|
| KullaniciAdi | string | Giriş için kullanıcı adı |
| Sifre | string | Giriş şifresi |
| Rol | string | "Yönetici" veya "Görevli" |
| YoneticiMi | bool (salt okunur) | Personel menüsü sadece yöneticiye açılır |
| KisiTuru() | string | Rol döndürür |

### Kategori
Id, Ad, Aciklama, ToString() → Ad.

### Yazar
Id, Ad, Soyad, Ulke, DogumYili, AdSoyad, ToString() → AdSoyad.

### Yayinevi
Id, Ad, Telefon, Adres, ToString() → Ad.

### Kitap
| Üye | Tür | Açıklama |
|---|---|---|
| Id, Isbn, Ad | int, string, string | Kimlik, ISBN (benzersiz), kitap adı |
| YazarId, KategoriId, YayineviId | int | Yabancı anahtarlar |
| BasimYili, SayfaSayisi | int | Basım yılı, sayfa sayısı |
| Stok | int | Kütüphanedeki toplam kopya |
| Mevcut | int | Rafta duran (ödünç verilmemiş) kopya |
| RafNo | string | Raf konumu |
| YazarAdi, KategoriAdi, YayineviAdi | string | Listelemede gösterilen isimler |
| OduncVerilebilirMi() | bool | Mevcut > 0 ise true |
| ToString() | string | "Ad (Mevcut adet)" |

### OduncIslemi
| Üye | Tür | Açıklama |
|---|---|---|
| Id, KitapId, UyeId, PersonelId | int | Kimlik ve ilişkiler |
| OduncTarihi | DateTime | Kitabın verildiği gün |
| SonTeslimTarihi | DateTime | Ödünç tarihi + 15 gün |
| IadeTarihi | DateTime? | İade edilmediyse null |
| KitapAdi, UyeAdi, PersonelAdi | string | Listelemede gösterilen isimler |
| IadeEdildiMi | bool | IadeTarihi != null |
| GecikmeGunu(tarih) | int | Son teslimden sonra geçen gün (en az 0) |

### Ceza
Id, OduncId, UyeId, GecikmeGunu, Tutar (decimal), Odendi (bool), OdemeTarihi (DateTime?), UyeAdi, KitapAdi.

### KitapOkunma
Rapor satırı: KitapAdi, YazarAdi, OkunmaSayisi.

---

## 2. Arayüzler

### IRepository&lt;T&gt;
Tüm repository sınıflarının uyduğu sözleşme:
`Ekle(T)`, `Guncelle(T)`, `Sil(int id)`, `Getir(int id) : T`, `TumunuGetir() : List<T>`, `Ara(string metin) : List<T>`.

---

## 3. Veri Katmanı

### Veritabani (static)
| Üye | Açıklama |
|---|---|
| BaglantiCumlesi | PostgreSQL bağlantı bilgisi (localhost:5433, kutuphane) |
| BaglantiAc() | Açık bir NpgsqlConnection döndürür |
| BaglantiTest() | Program açılırken veritabanına erişilebiliyor mu kontrol eder |
| SayiGetir(sql) | Tek sayı döndüren sorguları çalıştırır (rapor sayıları) |

### KategoriRepository, YazarRepository, YayineviRepository
IRepository'nin 6 metodunu ilgili tablo için uygular. Arama ad (yazar için ad+soyad) üzerinde ILIKE ile yapılır.

### KitapRepository
IRepository metodlarına ek olarak:
- `MevcutKitaplar()` – rafta kopyası olan kitaplar
- `MevcutDegistir(kitapId, miktar)` – ödünçte −1, iadede +1
- Güncellemede stok değişirse mevcut da aynı farkla değişir.

### UyeRepository
Ek: `AktifUyeler()` – sadece aktif üyeler (ödünç ekranında kullanılır).

### PersonelRepository
Ek: `KullaniciAdiIleGetir(kullaniciAdi)` – giriş kontrolü için.

### OduncRepository
Ek: `AktifOduncler()`, `GecikenOduncler()`, `UyeninAktifOduncSayisi(uyeId)`, `IadeEt(id, tarih)`, `EnCokOkunanlar(adet)`.

### CezaRepository
Ek: `OdenmemisCezalar()`, `UyeninOdenmemisCezasi(uyeId)`, `Ode(id)`.

---

## 4. Servisler

### GirisServisi
- `AktifPersonel` (static) – sisteme giriş yapan personel
- `GirisYap(kullaniciAdi, sifre)` – doğruysa AktifPersonel'i ayarlar, true döner
- `CikisYap()`

### OduncServisi
Sabitler: `MaksimumKitapSayisi = 3`, `OduncGunSayisi = 15`, `GunlukCezaTutari = 5 TL`.
- `OduncVer(kitapId, uyeId, personelId)` – kontroller: üye aktif mi, kitap rafta var mı, üyede 3'ten az kitap var mı, ödenmemiş cezası yok mu. Uygunsa kayıt açar ve mevcut −1.
- `IadeAl(oduncId)` – iade tarihini yazar, mevcut +1, gecikme varsa Ceza kaydı oluşturur, ceza tutarını döndürür.
- `CezaHesapla(odunc, tarih)` – gecikme günü × 5 TL.

### RaporServisi
`ToplamKitapSayisi()`, `ToplamUyeSayisi()`, `OduncteKitapSayisi()`, `GecikenKitapSayisi()`, `OdenmemisCezaToplami()`, `GecikenKitaplar()`, `EnCokOkunanKitaplar()`.

---

## 5. Formlar (Arayüz)

### KayitForm (abstract, Form'dan türer)
Ekle/Güncelle/Sil/Temizle butonları, arama kutusu ve tabloyu hazır veren ortak taban form.
- Alt sınıfların doldurması gereken: `Listele`, `Ekle`, `Guncelle`, `Sil`, `KayitYukle`
- Yardımcılar: `MetinAlaniEkle`, `SecimAlaniEkle`, `SayiAlaniEkle`, `OnayAlaniEkle`, `TabloyaYukle`, `ZorunluAlanKontrol`, `Temizle`, `Yenile`
- Veritabanı hatalarını anlaşılır mesaja çevirir (silinemeyen kayıt, tekrar eden kayıt).

### KayitForm'dan türeyenler
| Form | Alanlar |
|---|---|
| KategoriForm | Ad, Açıklama |
| YazarForm | Ad, Soyad, Ülke, Doğum Yılı |
| YayineviForm | Ad, Telefon, Adres |
| KitapForm | ISBN, Ad, Yazar, Kategori, Yayınevi, Basım Yılı, Sayfa, Stok, Raf No |
| UyeForm | TC No (11 hane kontrolü), Ad, Soyad, Telefon, E-posta, Adres, Aktif |
| PersonelForm | TC No, Ad, Soyad, Telefon, E-posta, Kullanıcı Adı, Şifre, Rol (sadece yönetici açabilir) |

### Diğer formlar
| Form | Görevi |
|---|---|
| GirisForm | Kullanıcı adı/şifre ile giriş |
| AnaForm | Menü, özet kutuları (kitap, üye, ödünçteki, geciken), tüm formlara geçiş |
| OduncForm | Üye + kitap seçip ödünç verme, seçili kaydı iade alma; gecikenler kırmızı |
| CezaForm | Cezaları listeleme, ödendi olarak işaretleme, ödenmemiş toplam |
| RaporForm | Genel özet, geciken kitaplar, en çok okunan kitaplar sekmeleri |

### Program
Veritabanı bağlantısını test eder, GirisForm'u açar, giriş başarılıysa AnaForm'u çalıştırır.
