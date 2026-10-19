using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Servisler;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class OduncForm : Form
{
    private readonly OduncServisi oduncServisi = new OduncServisi();
    private readonly OduncRepository oduncRepository = new OduncRepository();
    private readonly UyeRepository uyeRepository = new UyeRepository();
    private readonly KitapRepository kitapRepository = new KitapRepository();
    private ComboBox cmbUye = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 220 };
    private ComboBox cmbKitap = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 280 };
    private TextBox txtAra = new TextBox { Width = 200 };
    private CheckBox chkTumu = new CheckBox { Text = "İade edilenleri de göster", AutoSize = true };
    private DataGridView tablo = new DataGridView();

    public OduncForm()
    {
        Text = "Ödünç Verme / İade Alma";
        Size = new Size(1150, 620);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        var btnOduncVer = new Button { Text = "Ödünç Ver", Width = 110, Height = 32, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        var btnIadeAl = new Button { Text = "Seçili Kitabı İade Al", Width = 180, Height = 32, BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnOduncVer.Click += BtnOduncVer_Click;
        btnIadeAl.Click += BtnIadeAl_Click;

        var ustPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };
        ustPanel.Controls.Add(Etiket("Üye:"));
        ustPanel.Controls.Add(cmbUye);
        ustPanel.Controls.Add(Etiket("Kitap:"));
        ustPanel.Controls.Add(cmbKitap);
        ustPanel.Controls.Add(btnOduncVer);

        var altPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };
        altPanel.Controls.Add(Etiket("Ara:"));
        altPanel.Controls.Add(txtAra);
        altPanel.Controls.Add(chkTumu);
        altPanel.Controls.Add(btnIadeAl);

        tablo.Dock = DockStyle.Fill;
        tablo.ReadOnly = true;
        tablo.AllowUserToAddRows = false;
        tablo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tablo.MultiSelect = false;
        tablo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tablo.BackgroundColor = Color.White;
        tablo.CellFormatting += Tablo_CellFormatting;

        Controls.Add(tablo);
        Controls.Add(altPanel);
        Controls.Add(ustPanel);

        txtAra.TextChanged += (s, e) => Listele();
        chkTumu.CheckedChanged += (s, e) => Listele();
        Load += (s, e) => Yenile();
    }

    private Label Etiket(string yazi)
    {
        return new Label { Text = yazi, AutoSize = true, Margin = new Padding(3, 6, 3, 3) };
    }

    private void Yenile()
    {
        try
        {
            cmbUye.DataSource = uyeRepository.AktifUyeler();
            cmbKitap.DataSource = kitapRepository.MevcutKitaplar();
            Listele();
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Listele()
    {
        var liste = oduncRepository.Ara(txtAra.Text.Trim());
        if (!chkTumu.Checked)
        {
            liste = liste.Where(o => !o.IadeEdildiMi).ToList();
        }
        tablo.DataSource = liste;
        foreach (var kolon in new[] { "KitapId", "UyeId", "PersonelId" })
        {
            tablo.Columns[kolon].Visible = false;
        }
    }

    private void BtnOduncVer_Click(object sender, EventArgs e)
    {
        if (cmbUye.SelectedItem == null || cmbKitap.SelectedItem == null)
        {
            MessageBox.Show("Üye ve kitap seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            var uye = (Uye)cmbUye.SelectedItem;
            var kitap = (Kitap)cmbKitap.SelectedItem;
            oduncServisi.OduncVer(kitap.Id, uye.Id, GirisServisi.AktifPersonel.Id);
            MessageBox.Show(kitap.Ad + " kitabı " + uye.AdSoyad + " adlı üyeye verildi.\nSon teslim: " +
                DateTime.Today.AddDays(OduncServisi.OduncGunSayisi).ToShortDateString(), "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Yenile();
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnIadeAl_Click(object sender, EventArgs e)
    {
        if (tablo.CurrentRow == null)
        {
            MessageBox.Show("Tablodan bir ödünç kaydı seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            var odunc = (OduncIslemi)tablo.CurrentRow.DataBoundItem;
            decimal ceza = oduncServisi.IadeAl(odunc.Id);
            string mesaj = odunc.KitapAdi + " iade alındı.";
            if (ceza > 0)
            {
                mesaj += "\nGecikme cezası: " + ceza.ToString("C2");
            }
            MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Yenile();
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Tablo_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        var odunc = (OduncIslemi)tablo.Rows[e.RowIndex].DataBoundItem;
        if (!odunc.IadeEdildiMi && odunc.GecikmeGunu(DateTime.Today) > 0)
        {
            e.CellStyle.BackColor = Color.MistyRose;
        }
        else if (odunc.IadeEdildiMi)
        {
            e.CellStyle.ForeColor = Color.Gray;
        }
    }
}
