namespace KutuphaneOtomasyonu.Modeller;

public class Uye : Kisi
{
    public string Adres { get; set; }
    public DateTime KayitTarihi { get; set; }
    public bool Aktif { get; set; }

    public override string KisiTuru()
    {
        return "Üye";
    }
}
