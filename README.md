# Kütüphane Otomasyonu (C# + Windows Forms + PostgreSQL)

## Çalıştırma
1. Docker Desktop açık olsun.
2. Bu klasörde: `docker compose up -d`  (PostgreSQL **5433** portunda açılır, tablolar ve örnek veriler otomatik yüklenir)
3. `cd KutuphaneOtomasyonu` → `dotnet run`  (veya `.csproj` dosyasını Visual Studio ile açıp F5)

## Giriş bilgileri
| Kullanıcı | Şifre | Rol |
|---|---|---|
| admin | admin123 | Yönetici (personel ekranı açık) |
| gorevli | 1234 | Görevli |

## Kurallar
- Bir üye en fazla 3 kitap alabilir, süre 15 gün.
- Gecikme cezası günlük 5 TL; ödenmemiş cezası olan veya pasif üye kitap alamaz.

## Veritabanını sıfırlama
`docker compose down -v` ardından `docker compose up -d`

## Klasörler
- `KutuphaneOtomasyonu/` – C# kaynak kodu (Modeller, Arayuzler, Veri, Servisler, Formlar)
- `veritabani/init.sql` – tablolar + örnek veri
- `diyagramlar/` – Sınıf, Nesne, Dağıtım ve ER diyagramları (.puml kaynak + .png)
- `SINIFLAR.md` – her sınıfın özellik ve metot açıklamaları
