namespace KutuphaneOtomasyonu.Modeller;

public class Personel : Kisi
{
    public string KullaniciAdi { get; set; }
    public string Sifre { get; set; }
    public string Rol { get; set; }

    public bool YoneticiMi => Rol == "Yönetici";

    public override string KisiTuru()
    {
        return Rol;
    }
}
