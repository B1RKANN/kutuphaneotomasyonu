using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Servisler;

public class RaporServisi
{
    private readonly OduncRepository oduncRepository = new OduncRepository();
    private readonly CezaRepository cezaRepository = new CezaRepository();

    public int ToplamKitapSayisi()
    {
        return Veritabani.SayiGetir("SELECT COALESCE(SUM(stok), 0) FROM kitaplar");
    }

    public int ToplamUyeSayisi()
    {
        return Veritabani.SayiGetir("SELECT COUNT(*) FROM uyeler");
    }

    public int OduncteKitapSayisi()
    {
        return oduncRepository.AktifOduncler().Count;
    }

    public int GecikenKitapSayisi()
    {
        return oduncRepository.GecikenOduncler().Count;
    }

    public decimal OdenmemisCezaToplami()
    {
        return cezaRepository.OdenmemisCezalar().Sum(c => c.Tutar);
    }

    public List<OduncIslemi> GecikenKitaplar()
    {
        return oduncRepository.GecikenOduncler();
    }

    public List<KitapOkunma> EnCokOkunanKitaplar()
    {
        return oduncRepository.EnCokOkunanlar(10);
    }
}
