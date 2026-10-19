namespace KutuphaneOtomasyonu.Modeller;

public class Yazar
{
    public int Id { get; set; }
    public string Ad { get; set; }
    public string Soyad { get; set; }
    public string Ulke { get; set; }
    public int DogumYili { get; set; }

    public string AdSoyad => Ad + " " + Soyad;

    public override string ToString()
    {
        return AdSoyad;
    }
}
