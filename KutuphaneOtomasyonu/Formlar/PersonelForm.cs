using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class PersonelForm : KayitForm
{
    private readonly PersonelRepository repository = new PersonelRepository();
    private TextBox txtTcNo;
    private TextBox txtAd;
    private TextBox txtSoyad;
    private TextBox txtTelefon;
    private TextBox txtEposta;
    private TextBox txtKullaniciAdi;
    private TextBox txtSifre;
    private ComboBox cmbRol;

    public PersonelForm() : base("Personel İşlemleri")
    {
        txtTcNo = MetinAlaniEkle("TC No *");
        txtTcNo.MaxLength = 11;
        txtAd = MetinAlaniEkle("Ad *");
        txtSoyad = MetinAlaniEkle("Soyad *");
        txtTelefon = MetinAlaniEkle("Telefon");
        txtEposta = MetinAlaniEkle("E-posta");
        txtKullaniciAdi = MetinAlaniEkle("Kullanıcı Adı *");
        txtSifre = MetinAlaniEkle("Şifre *");
        txtSifre.UseSystemPasswordChar = true;
        cmbRol = SecimAlaniEkle("Rol *");
        cmbRol.Items.AddRange(new object[] { "Yönetici", "Görevli" });
    }

    protected override void Listele(string arama)
    {
        TabloyaYukle(repository.Ara(arama), "Sifre");
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
        if (Servisler.GirisServisi.AktifPersonel.Id == id)
        {
            throw new Exception("Giriş yapan personel kendini silemez.");
        }
        repository.Sil(id);
    }

    protected override void KayitYukle(int id)
    {
        var personel = repository.Getir(id);
        txtTcNo.Text = personel.TcNo;
        txtAd.Text = personel.Ad;
        txtSoyad.Text = personel.Soyad;
        txtTelefon.Text = personel.Telefon;
        txtEposta.Text = personel.Eposta;
        txtKullaniciAdi.Text = personel.KullaniciAdi;
        txtSifre.Text = personel.Sifre;
        cmbRol.SelectedItem = personel.Rol;
    }

    private void Kontrol()
    {
        ZorunluAlanKontrol(txtTcNo, txtAd, txtSoyad, txtKullaniciAdi, txtSifre);
        if (cmbRol.SelectedItem == null)
        {
            throw new Exception("Rol seçin.");
        }
    }

    private Personel FormdanAl()
    {
        return new Personel
        {
            Id = seciliId,
            TcNo = txtTcNo.Text.Trim(),
            Ad = txtAd.Text.Trim(),
            Soyad = txtSoyad.Text.Trim(),
            Telefon = txtTelefon.Text.Trim(),
            Eposta = txtEposta.Text.Trim(),
            KullaniciAdi = txtKullaniciAdi.Text.Trim(),
            Sifre = txtSifre.Text,
            Rol = cmbRol.SelectedItem.ToString()
        };
    }
}
