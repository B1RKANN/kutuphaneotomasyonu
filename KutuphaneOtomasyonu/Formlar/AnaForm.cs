using KutuphaneOtomasyonu.Servisler;

namespace KutuphaneOtomasyonu.Formlar;

public class AnaForm : Form
{
    private readonly RaporServisi raporServisi = new RaporServisi();
    private Label lblKitap = OzetKutusu(Color.SteelBlue);
    private Label lblUye = OzetKutusu(Color.SeaGreen);
    private Label lblOdunc = OzetKutusu(Color.DarkOrange);
    private Label lblGeciken = OzetKutusu(Color.IndianRed);

    public AnaForm()
    {
        var personel = GirisServisi.AktifPersonel;
        Text = "Kütüphane Otomasyonu - " + personel.AdSoyad + " (" + personel.Rol + ")";
        Size = new Size(900, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        var menu = new MenuStrip();
        var tanimlar = new ToolStripMenuItem("Tanımlar");
        tanimlar.DropDownItems.Add("Kategoriler", null, (s, e) => FormAc(new KategoriForm()));
        tanimlar.DropDownItems.Add("Yazarlar", null, (s, e) => FormAc(new YazarForm()));
        tanimlar.DropDownItems.Add("Yayınevleri", null, (s, e) => FormAc(new YayineviForm()));
        menu.Items.Add(tanimlar);
        menu.Items.Add("Kitaplar", null, (s, e) => FormAc(new KitapForm()));
        menu.Items.Add("Üyeler", null, (s, e) => FormAc(new UyeForm()));
        menu.Items.Add("Ödünç / İade", null, (s, e) => FormAc(new OduncForm()));
        menu.Items.Add("Cezalar", null, (s, e) => FormAc(new CezaForm()));
        menu.Items.Add("Raporlar", null, (s, e) => FormAc(new RaporForm()));
        var personelMenu = menu.Items.Add("Personeller", null, (s, e) => FormAc(new PersonelForm()));
        personelMenu.Enabled = personel.YoneticiMi;
        menu.Items.Add("Çıkış", null, (s, e) => Close());

        var ozetPaneli = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 140, Padding = new Padding(20) };
        ozetPaneli.Controls.AddRange(new Control[] { lblKitap, lblUye, lblOdunc, lblGeciken });

        var butonPaneli = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        butonPaneli.Controls.Add(MenuButonu("Kitaplar", () => new KitapForm()));
        butonPaneli.Controls.Add(MenuButonu("Üyeler", () => new UyeForm()));
        butonPaneli.Controls.Add(MenuButonu("Ödünç / İade", () => new OduncForm()));
        butonPaneli.Controls.Add(MenuButonu("Cezalar", () => new CezaForm()));
        butonPaneli.Controls.Add(MenuButonu("Yazarlar", () => new YazarForm()));
        butonPaneli.Controls.Add(MenuButonu("Kategoriler", () => new KategoriForm()));
        butonPaneli.Controls.Add(MenuButonu("Yayınevleri", () => new YayineviForm()));
        butonPaneli.Controls.Add(MenuButonu("Raporlar", () => new RaporForm()));

        Controls.Add(butonPaneli);
        Controls.Add(ozetPaneli);
        Controls.Add(menu);
        MainMenuStrip = menu;

        Load += (s, e) => OzetiYenile();
    }

    private static Label OzetKutusu(Color renk)
    {
        return new Label
        {
            Width = 190,
            Height = 90,
            Margin = new Padding(5),
            BackColor = renk,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
    }

    private Button MenuButonu(string yazi, Func<Form> formOlustur)
    {
        var buton = new Button
        {
            Text = yazi,
            Width = 190,
            Height = 70,
            Margin = new Padding(5),
            BackColor = Color.WhiteSmoke,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 11, FontStyle.Bold)
        };
        buton.Click += (s, e) => FormAc(formOlustur());
        return buton;
    }

    private void FormAc(Form form)
    {
        form.ShowDialog(this);
        OzetiYenile();
    }

    private void OzetiYenile()
    {
        try
        {
            lblKitap.Text = "Toplam Kitap\n" + raporServisi.ToplamKitapSayisi();
            lblUye.Text = "Toplam Üye\n" + raporServisi.ToplamUyeSayisi();
            lblOdunc.Text = "Ödünçteki Kitap\n" + raporServisi.OduncteKitapSayisi();
            lblGeciken.Text = "Geciken Kitap\n" + raporServisi.GecikenKitapSayisi();
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
