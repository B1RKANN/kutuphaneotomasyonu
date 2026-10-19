using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class YazarForm : KayitForm
{
    private readonly YazarRepository repository = new YazarRepository();
    private TextBox txtAd;
    private TextBox txtSoyad;
    private TextBox txtUlke;
    private NumericUpDown numDogumYili;

    public YazarForm() : base("Yazar İşlemleri")
    {
        txtAd = MetinAlaniEkle("Ad *");
        txtSoyad = MetinAlaniEkle("Soyad *");
        txtUlke = MetinAlaniEkle("Ülke");
        numDogumYili = SayiAlaniEkle("Doğum Yılı", 0, 2100);
    }

    protected override void Listele(string arama)
    {
        TabloyaYukle(repository.Ara(arama));
    }

    protected override void Ekle()
    {
        ZorunluAlanKontrol(txtAd, txtSoyad);
        repository.Ekle(FormdanAl());
    }

    protected override void Guncelle()
    {
        ZorunluAlanKontrol(txtAd, txtSoyad);
        repository.Guncelle(FormdanAl());
    }

    protected override void Sil(int id)
    {
        repository.Sil(id);
    }

    protected override void KayitYukle(int id)
    {
        var yazar = repository.Getir(id);
        txtAd.Text = yazar.Ad;
        txtSoyad.Text = yazar.Soyad;
        txtUlke.Text = yazar.Ulke;
        numDogumYili.Value = yazar.DogumYili;
    }

    private Yazar FormdanAl()
    {
        return new Yazar
        {
            Id = seciliId,
            Ad = txtAd.Text.Trim(),
            Soyad = txtSoyad.Text.Trim(),
            Ulke = txtUlke.Text.Trim(),
            DogumYili = (int)numDogumYili.Value
        };
    }
}
