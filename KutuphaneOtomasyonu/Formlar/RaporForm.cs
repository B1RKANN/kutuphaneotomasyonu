using KutuphaneOtomasyonu.Servisler;

namespace KutuphaneOtomasyonu.Formlar;

public class RaporForm : Form
{
    private readonly RaporServisi raporServisi = new RaporServisi();
    private DataGridView tabloGeciken = TabloOlustur();
    private DataGridView tabloEnCok = TabloOlustur();
    private Label lblOzet = new Label { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 13), Padding = new Padding(20) };

    public RaporForm()
    {
        Text = "Raporlar";
        Size = new Size(1000, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        var sekmeler = new TabControl { Dock = DockStyle.Fill };
        var sekmeOzet = new TabPage("Genel Özet");
        var sekmeGeciken = new TabPage("Geciken Kitaplar");
        var sekmeEnCok = new TabPage("En Çok Okunan Kitaplar");
        sekmeOzet.Controls.Add(lblOzet);
        sekmeGeciken.Controls.Add(tabloGeciken);
        sekmeEnCok.Controls.Add(tabloEnCok);
        sekmeler.TabPages.Add(sekmeOzet);
        sekmeler.TabPages.Add(sekmeGeciken);
        sekmeler.TabPages.Add(sekmeEnCok);
        Controls.Add(sekmeler);

        Load += (s, e) => RaporlariYukle();
    }

    private static DataGridView TabloOlustur()
    {
        return new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Color.White
        };
    }

    private void RaporlariYukle()
    {
        try
        {
            lblOzet.Text =
                "Toplam kitap (kopya): " + raporServisi.ToplamKitapSayisi() + "\n\n" +
                "Toplam üye: " + raporServisi.ToplamUyeSayisi() + "\n\n" +
                "Ödünçteki kitap: " + raporServisi.OduncteKitapSayisi() + "\n\n" +
                "Geciken kitap: " + raporServisi.GecikenKitapSayisi() + "\n\n" +
                "Ödenmemiş ceza toplamı: " + raporServisi.OdenmemisCezaToplami().ToString("C2");

            tabloGeciken.DataSource = raporServisi.GecikenKitaplar()
                .Select(o => new
                {
                    Kitap = o.KitapAdi,
                    Uye = o.UyeAdi,
                    o.OduncTarihi,
                    o.SonTeslimTarihi,
                    GecikmeGunu = o.GecikmeGunu(DateTime.Today),
                    TahminiCeza = o.GecikmeGunu(DateTime.Today) * OduncServisi.GunlukCezaTutari
                })
                .ToList();

            tabloEnCok.DataSource = raporServisi.EnCokOkunanKitaplar();
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
