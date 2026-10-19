using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class UyeForm : KayitForm
{
    private readonly UyeRepository repository = new UyeRepository();
    private TextBox txtTcNo;
    private TextBox txtAd;
    private TextBox txtSoyad;
    private TextBox txtTelefon;
    private TextBox txtEposta;
    private TextBox txtAdres;
    private CheckBox chkAktif;

    public UyeForm() : base("Üye İşlemleri")
    {
        txtTcNo = MetinAlaniEkle("TC No *");
        txtTcNo.MaxLength = 11;
        txtAd = MetinAlaniEkle("Ad *");
        txtSoyad = MetinAlaniEkle("Soyad *");
        txtTelefon = MetinAlaniEkle("Telefon");
        txtEposta = MetinAlaniEkle("E-posta");
        txtAdres = MetinAlaniEkle("Adres");
        chkAktif = OnayAlaniEkle("Üyelik aktif");
        chkAktif.Checked = true;
    }

    protected override void Listele(string arama)
    {
        TabloyaYukle(repository.Ara(arama));
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
        var uye = repository.Getir(id);
        txtTcNo.Text = uye.TcNo;
        txtAd.Text = uye.Ad;
        txtSoyad.Text = uye.Soyad;
        txtTelefon.Text = uye.Telefon;
        txtEposta.Text = uye.Eposta;
        txtAdres.Text = uye.Adres;
        chkAktif.Checked = uye.Aktif;
    }

    protected override void Temizle()
    {
        base.Temizle();
        chkAktif.Checked = true;
    }

    private void Kontrol()
    {
        ZorunluAlanKontrol(txtTcNo, txtAd, txtSoyad);
        if (txtTcNo.Text.Trim().Length != 11 || !txtTcNo.Text.Trim().All(char.IsDigit))
        {
            throw new Exception("TC No 11 haneli rakam olmalıdır.");
        }
    }

    private Uye FormdanAl()
    {
        return new Uye
        {
            Id = seciliId,
            TcNo = txtTcNo.Text.Trim(),
            Ad = txtAd.Text.Trim(),
            Soyad = txtSoyad.Text.Trim(),
            Telefon = txtTelefon.Text.Trim(),
            Eposta = txtEposta.Text.Trim(),
            Adres = txtAdres.Text.Trim(),
            Aktif = chkAktif.Checked
        };
    }
}
