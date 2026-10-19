using Npgsql;

namespace KutuphaneOtomasyonu.Formlar;

public abstract class KayitForm : Form
{
    protected int seciliId;
    protected DataGridView tablo = new DataGridView();
    protected TextBox txtAra = new TextBox();
    private TableLayoutPanel alanPaneli = new TableLayoutPanel();
    private FlowLayoutPanel butonPaneli = new FlowLayoutPanel();

    protected KayitForm(string baslik)
    {
        Text = baslik;
        Size = new Size(1100, 620);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        tablo.Dock = DockStyle.Fill;
        tablo.ReadOnly = true;
        tablo.AllowUserToAddRows = false;
        tablo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tablo.MultiSelect = false;
        tablo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tablo.BackgroundColor = Color.White;
        tablo.CellClick += Tablo_CellClick;

        var solPanel = new Panel { Dock = DockStyle.Left, Width = 360, Padding = new Padding(10) };
        alanPaneli.Dock = DockStyle.Top;
        alanPaneli.AutoSize = true;
        alanPaneli.ColumnCount = 2;
        alanPaneli.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        alanPaneli.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        butonPaneli.Dock = DockStyle.Top;
        butonPaneli.Height = 90;
        butonPaneli.Padding = new Padding(0, 10, 0, 0);
        ButonEkle("Ekle", Color.SeaGreen, (s, e) => Islem(Ekle, "Kayıt eklendi."));
        ButonEkle("Güncelle", Color.SteelBlue, (s, e) => SeciliIslem(Guncelle, "Kayıt güncellendi."));
        ButonEkle("Sil", Color.IndianRed, (s, e) => SilOnay());
        ButonEkle("Temizle", Color.Gray, (s, e) => Temizle());

        solPanel.Controls.Add(butonPaneli);
        solPanel.Controls.Add(alanPaneli);

        var aramaPaneli = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 45, Padding = new Padding(8) };
        txtAra.Width = 300;
        txtAra.TextChanged += (s, e) => Yenile();
        aramaPaneli.Controls.Add(new Label { Text = "Ara:", AutoSize = true, Margin = new Padding(3, 6, 3, 3) });
        aramaPaneli.Controls.Add(txtAra);

        Controls.Add(tablo);
        Controls.Add(solPanel);
        Controls.Add(aramaPaneli);

        Load += (s, e) => Yenile();
    }

    protected abstract void Listele(string arama);
    protected abstract void Ekle();
    protected abstract void Guncelle();
    protected abstract void Sil(int id);
    protected abstract void KayitYukle(int id);

    protected TextBox MetinAlaniEkle(string etiket)
    {
        var kutu = new TextBox();
        AlanEkle(etiket, kutu);
        return kutu;
    }

    protected ComboBox SecimAlaniEkle(string etiket)
    {
        var kutu = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        AlanEkle(etiket, kutu);
        return kutu;
    }

    protected NumericUpDown SayiAlaniEkle(string etiket, int enKucuk, int enBuyuk)
    {
        var kutu = new NumericUpDown { Minimum = enKucuk, Maximum = enBuyuk };
        AlanEkle(etiket, kutu);
        return kutu;
    }

    protected CheckBox OnayAlaniEkle(string etiket)
    {
        var kutu = new CheckBox { Text = etiket, AutoSize = true };
        AlanEkle("", kutu);
        return kutu;
    }

    protected void TabloyaYukle(object liste, params string[] gizlenecekKolonlar)
    {
        tablo.DataSource = liste;
        foreach (var kolon in gizlenecekKolonlar)
        {
            if (tablo.Columns.Contains(kolon))
            {
                tablo.Columns[kolon].Visible = false;
            }
        }
    }

    protected void ZorunluAlanKontrol(params TextBox[] kutular)
    {
        if (kutular.Any(k => string.IsNullOrWhiteSpace(k.Text)))
        {
            throw new Exception("Lütfen (*) ile işaretli zorunlu alanları doldurun.");
        }
    }

    protected virtual void Temizle()
    {
        seciliId = 0;
        foreach (Control kontrol in alanPaneli.Controls)
        {
            if (kontrol is TextBox metin) metin.Clear();
            if (kontrol is ComboBox secim) secim.SelectedIndex = -1;
            if (kontrol is NumericUpDown sayi) sayi.Value = sayi.Minimum;
            if (kontrol is CheckBox onay) onay.Checked = false;
        }
    }

    protected void Yenile()
    {
        try
        {
            Listele(txtAra.Text.Trim());
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void AlanEkle(string etiket, Control kontrol)
    {
        int satir = alanPaneli.RowCount;
        alanPaneli.RowCount++;
        kontrol.Dock = DockStyle.Fill;
        alanPaneli.Controls.Add(new Label { Text = etiket, AutoSize = true, Margin = new Padding(3, 6, 3, 3) }, 0, satir);
        alanPaneli.Controls.Add(kontrol, 1, satir);
    }

    private void ButonEkle(string yazi, Color renk, EventHandler tiklama)
    {
        var buton = new Button
        {
            Text = yazi,
            Width = 80,
            Height = 35,
            BackColor = renk,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        buton.Click += tiklama;
        butonPaneli.Controls.Add(buton);
    }

    private void Islem(Action islem, string mesaj)
    {
        try
        {
            islem();
            MessageBox.Show(mesaj, "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Temizle();
            Yenile();
        }
        catch (PostgresException hata) when (hata.SqlState == "23503")
        {
            MessageBox.Show("Bu kayıt başka kayıtlarda kullanıldığı için silinemez.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (PostgresException hata) when (hata.SqlState == "23505")
        {
            MessageBox.Show("Bu bilgilerle kayıtlı başka bir kayıt zaten var.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SeciliIslem(Action islem, string mesaj)
    {
        if (seciliId == 0)
        {
            MessageBox.Show("Önce tablodan bir kayıt seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        Islem(islem, mesaj);
    }

    private void SilOnay()
    {
        if (seciliId == 0)
        {
            MessageBox.Show("Önce tablodan bir kayıt seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Seçili kayıt silinsin mi?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            int id = seciliId;
            Islem(() => Sil(id), "Kayıt silindi.");
        }
    }

    private void Tablo_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || !tablo.Columns.Contains("Id"))
        {
            return;
        }
        seciliId = (int)tablo.Rows[e.RowIndex].Cells["Id"].Value;
        KayitYukle(seciliId);
    }
}
