namespace KutuphaneOtomasyonu.Modeller;

public class OduncIslemi
{
    public int Id { get; set; }
    public int KitapId { get; set; }
    public int UyeId { get; set; }
    public int PersonelId { get; set; }
    public DateTime OduncTarihi { get; set; }
    public DateTime SonTeslimTarihi { get; set; }
    public DateTime? IadeTarihi { get; set; }
    public string KitapAdi { get; set; }
    public string UyeAdi { get; set; }
    public string PersonelAdi { get; set; }

    public bool IadeEdildiMi => IadeTarihi != null;

    public int GecikmeGunu(DateTime tarih)
    {
        int gun = (tarih.Date - SonTeslimTarihi.Date).Days;
        return gun > 0 ? gun : 0;
    }
}
