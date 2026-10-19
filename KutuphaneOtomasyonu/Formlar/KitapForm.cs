using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class KitapForm : KayitForm
{
    private readonly KitapRepository repository = new KitapRepository();
    private readonly YazarRepository yazarRepository = new YazarRepository();
    private readonly KategoriRepository kategoriRepository = new KategoriRepository();
    private readonly YayineviRepository yayineviRepository = new YayineviRepository();
    private TextBox txtIsbn;
    private TextBox txtAd;
    private ComboBox cmbYazar;
    private ComboBox cmbKategori;
    private ComboBox cmbYayinevi;
    private NumericUpDown numBasimYili;
    private NumericUpDown numSayfa;
    private NumericUpDown numStok;
    private TextBox txtRafNo;

    public KitapForm() : base("Kitap İşlemleri")
    {
        txtIsbn = MetinAlaniEkle("ISBN *");
        txtAd = MetinAlaniEkle("Kitap Adı *");
        cmbYazar = SecimAlaniEkle("Yazar *");
        cmbKategori = SecimAlaniEkle("Kategori *");
        cmbYayinevi = SecimAlaniEkle("Yayınevi *");
        numBasimYili = SayiAlaniEkle("Basım Yılı", 0, 2100);
        numSayfa = SayiAlaniEkle("Sayfa Sayısı", 0, 10000);
        numStok = SayiAlaniEkle("Stok", 1, 1000);
        txtRafNo = MetinAlaniEkle("Raf No");

        Load += (s, e) => SecimleriDoldur();
    }

    private void SecimleriDoldur()
    {
        cmbYazar.DataSource = yazarRepository.TumunuGetir();
        cmbKategori.DataSource = kategoriRepository.TumunuGetir();
        cmbYayinevi.DataSource = yayineviRepository.TumunuGetir();
        Temizle();
    }

    protected override void Listele(string arama)
    {
        TabloyaYukle(repository.Ara(arama), "YazarId", "KategoriId", "YayineviId");
    }

    protected override void Ekle()
    {
        Kontrol();
        repository.Ekle(FormdanAl());
    }

    protected override void Guncelle()
    {
        Kontrol();
        repository.Guncelle(FormdanAl());
    }

    protected override void Sil(int id)
    {
        repository.Sil(id);
    }

    protected override void KayitYukle(int id)
    {
        var kitap = repository.Getir(id);
        txtIsbn.Text = kitap.Isbn;
        txtAd.Text = kitap.Ad;
        cmbYazar.SelectedItem = ((List<Yazar>)cmbYazar.DataSource).First(y => y.Id == kitap.YazarId);
        cmbKategori.SelectedItem = ((List<Kategori>)cmbKategori.DataSource).First(k => k.Id == kitap.KategoriId);
        cmbYayinevi.SelectedItem = ((List<Yayinevi>)cmbYayinevi.DataSource).First(y => y.Id == kitap.YayineviId);
        numBasimYili.Value = kitap.BasimYili;
        numSayfa.Value = kitap.SayfaSayisi;
        numStok.Value = kitap.Stok;
        txtRafNo.Text = kitap.RafNo;
    }

    private void Kontrol()
    {
        ZorunluAlanKontrol(txtIsbn, txtAd);
        if (cmbYazar.SelectedItem == null || cmbKategori.SelectedItem == null || cmbYayinevi.SelectedItem == null)
        {
            throw new Exception("Yazar, kategori ve yayınevi seçin.");
        }
    }

    private Kitap FormdanAl()
    {
        return new Kitap
        {
            Id = seciliId,
            Isbn = txtIsbn.Text.Trim(),
            Ad = txtAd.Text.Trim(),
            YazarId = ((Yazar)cmbYazar.SelectedItem).Id,
            KategoriId = ((Kategori)cmbKategori.SelectedItem).Id,
            YayineviId = ((Yayinevi)cmbYayinevi.SelectedItem).Id,
            BasimYili = (int)numBasimYili.Value,
            SayfaSayisi = (int)numSayfa.Value,
            Stok = (int)numStok.Value,
            RafNo = txtRafNo.Text.Trim()
        };
    }
}
