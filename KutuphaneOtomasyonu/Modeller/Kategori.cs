namespace KutuphaneOtomasyonu.Modeller;

public class Kategori
{
    public int Id { get; set; }
    public string Ad { get; set; }
    public string Aciklama { get; set; }

    public override string ToString()
    {
        return Ad;
    }
}
