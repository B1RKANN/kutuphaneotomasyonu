using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Servisler;

public class GirisServisi
{
    public static Personel AktifPersonel { get; private set; }

    private readonly PersonelRepository personelRepository = new PersonelRepository();

    public bool GirisYap(string kullaniciAdi, string sifre)
    {
        var personel = personelRepository.KullaniciAdiIleGetir(kullaniciAdi);
        if (personel == null || personel.Sifre != sifre)
        {
            return false;
        }
        AktifPersonel = personel;
        return true;
    }

    public void CikisYap()
    {
        AktifPersonel = null;
    }
}
