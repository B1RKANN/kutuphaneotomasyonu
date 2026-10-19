namespace KutuphaneOtomasyonu.Modeller;

public class Ceza
{
    public int Id { get; set; }
    public int OduncId { get; set; }
    public int UyeId { get; set; }
    public int GecikmeGunu { get; set; }
    public decimal Tutar { get; set; }
    public bool Odendi { get; set; }
    public DateTime? OdemeTarihi { get; set; }
    public string UyeAdi { get; set; }
    public string KitapAdi { get; set; }
}
