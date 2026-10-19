namespace KutuphaneOtomasyonu.Modeller;

public class Yayinevi
{
    public int Id { get; set; }
    public string Ad { get; set; }
    public string Telefon { get; set; }
    public string Adres { get; set; }

    public override string ToString()
    {
        return Ad;
    }
}
