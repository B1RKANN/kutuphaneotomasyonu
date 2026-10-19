using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class YayineviForm : KayitForm
{
    private readonly YayineviRepository repository = new YayineviRepository();
    private TextBox txtAd;
    private TextBox txtTelefon;
    private TextBox txtAdres;

    public YayineviForm() : base("Yayınevi İşlemleri")
    {
        txtAd = MetinAlaniEkle("Yayınevi Adı *");
        txtTelefon = MetinAlaniEkle("Telefon");
        txtAdres = MetinAlaniEkle("Adres");
    }

    protected override void Listele(string arama)
    {
        TabloyaYukle(repository.Ara(arama));
    }

    protected override void Ekle()
    {
        ZorunluAlanKontrol(txtAd);
        repository.Ekle(FormdanAl());
    }

    protected override void Guncelle()
    {
        ZorunluAlanKontrol(txtAd);
        repository.Guncelle(FormdanAl());
    }

    protected override void Sil(int id)
    {
        repository.Sil(id);
    }

    protected override void KayitYukle(int id)
    {
        var yayinevi = repository.Getir(id);
        txtAd.Text = yayinevi.Ad;
        txtTelefon.Text = yayinevi.Telefon;
        txtAdres.Text = yayinevi.Adres;
    }

    private Yayinevi FormdanAl()
    {
        return new Yayinevi
        {
            Id = seciliId,
            Ad = txtAd.Text.Trim(),
            Telefon = txtTelefon.Text.Trim(),
            Adres = txtAdres.Text.Trim()
        };
    }
}
