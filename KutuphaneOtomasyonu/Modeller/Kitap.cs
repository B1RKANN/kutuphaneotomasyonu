namespace KutuphaneOtomasyonu.Modeller;

public class Kitap
{
    public int Id { get; set; }
    public string Isbn { get; set; }
    public string Ad { get; set; }
    public int YazarId { get; set; }
    public int KategoriId { get; set; }
    public int YayineviId { get; set; }
    public int BasimYili { get; set; }
    public int SayfaSayisi { get; set; }
    public int Stok { get; set; }
    public int Mevcut { get; set; }
    public string RafNo { get; set; }
    public string YazarAdi { get; set; }
    public string KategoriAdi { get; set; }
    public string YayineviAdi { get; set; }

    public bool OduncVerilebilirMi()
    {
        return Mevcut > 0;
    }

    public override string ToString()
    {
        return Ad + " (" + Mevcut + " adet)";
    }
}
