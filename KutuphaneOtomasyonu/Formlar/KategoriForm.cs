using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class KategoriForm : KayitForm
{
    private readonly KategoriRepository repository = new KategoriRepository();
    private TextBox txtAd;
    private TextBox txtAciklama;

    public KategoriForm() : base("Kategori İşlemleri")
    {
        txtAd = MetinAlaniEkle("Kategori Adı *");
        txtAciklama = MetinAlaniEkle("Açıklama");
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
        var kategori = repository.Getir(id);
        txtAd.Text = kategori.Ad;
        txtAciklama.Text = kategori.Aciklama;
    }

    private Kategori FormdanAl()
    {
        return new Kategori
        {
            Id = seciliId,
            Ad = txtAd.Text.Trim(),
            Aciklama = txtAciklama.Text.Trim()
        };
    }
}
