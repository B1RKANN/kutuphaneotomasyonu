namespace KutuphaneOtomasyonu.Modeller;

public abstract class Kisi
{
    public int Id { get; set; }
    public string TcNo { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Telefon { get; set; }
    public string Eposta { get; set; }

    public string AdSoyad => Ad + " " + Soyad;

    public abstract string KisiTuru();

    public override string ToString()
    {
        return AdSoyad;
    }
}
