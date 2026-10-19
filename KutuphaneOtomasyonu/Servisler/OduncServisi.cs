using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Servisler;

public class OduncServisi
{
    public const int MaksimumKitapSayisi = 3;
    public const int OduncGunSayisi = 15;
    public const decimal GunlukCezaTutari = 5;

    private readonly KitapRepository kitapRepository = new KitapRepository();
    private readonly UyeRepository uyeRepository = new UyeRepository();
    private readonly OduncRepository oduncRepository = new OduncRepository();
    private readonly CezaRepository cezaRepository = new CezaRepository();

    public void OduncVer(int kitapId, int uyeId, int personelId)
    {
        var kitap = kitapRepository.Getir(kitapId);
        var uye = uyeRepository.Getir(uyeId);

        if (kitap == null || uye == null)
        {
            throw new Exception("Kitap veya üye bulunamadı.");
        }
        if (!uye.Aktif)
        {
            throw new Exception("Üyelik aktif değil.");
        }
        if (!kitap.OduncVerilebilirMi())
        {
            throw new Exception("Bu kitabın rafta kopyası kalmadı.");
        }
        if (oduncRepository.UyeninAktifOduncSayisi(uyeId) >= MaksimumKitapSayisi)
        {
            throw new Exception("Üye en fazla " + MaksimumKitapSayisi + " kitap alabilir.");
        }
        if (cezaRepository.UyeninOdenmemisCezasi(uyeId) > 0)
        {
            throw new Exception("Üyenin ödenmemiş cezası var.");
        }

        var odunc = new OduncIslemi
        {
            KitapId = kitapId,
            UyeId = uyeId,
            PersonelId = personelId,
            OduncTarihi = DateTime.Today,
            SonTeslimTarihi = DateTime.Today.AddDays(OduncGunSayisi)
        };
        oduncRepository.Ekle(odunc);
        kitapRepository.MevcutDegistir(kitapId, -1);
    }

    public decimal IadeAl(int oduncId)
    {
        var odunc = oduncRepository.Getir(oduncId);
        if (odunc == null)
        {
            throw new Exception("Ödünç kaydı bulunamadı.");
        }
        if (odunc.IadeEdildiMi)
        {
            throw new Exception("Bu kitap zaten iade edilmiş.");
        }

        oduncRepository.IadeEt(oduncId, DateTime.Today);
        kitapRepository.MevcutDegistir(odunc.KitapId, 1);

        decimal tutar = CezaHesapla(odunc, DateTime.Today);
        if (tutar > 0)
        {
            cezaRepository.Ekle(new Ceza
            {
                OduncId = odunc.Id,
                UyeId = odunc.UyeId,
                GecikmeGunu = odunc.GecikmeGunu(DateTime.Today),
                Tutar = tutar,
                Odendi = false
            });
        }
        return tutar;
    }

    public decimal CezaHesapla(OduncIslemi odunc, DateTime tarih)
    {
        return odunc.GecikmeGunu(tarih) * GunlukCezaTutari;
    }
}
