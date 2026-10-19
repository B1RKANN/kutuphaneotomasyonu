using KutuphaneOtomasyonu.Modeller;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu.Formlar;

public class CezaForm : Form
{
    private readonly CezaRepository cezaRepository = new CezaRepository();
    private TextBox txtAra = new TextBox { Width = 250 };
    private CheckBox chkOdenmemis = new CheckBox { Text = "Sadece ödenmemişler", AutoSize = true, Checked = true };
    private Label lblToplam = new Label { AutoSize = true, Margin = new Padding(20, 6, 3, 3), ForeColor = Color.DarkRed };
    private DataGridView tablo = new DataGridView();

    public CezaForm()
    {
        Text = "Ceza İşlemleri";
        Size = new Size(1000, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);

        var btnOde = new Button { Text = "Ödendi Olarak İşaretle", Width = 190, Height = 32, BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnOde.Click += BtnOde_Click;

        var ustPanel = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(8) };
        ustPanel.Controls.Add(new Label { Text = "Ara:", AutoSize = true, Margin = new Padding(3, 6, 3, 3) });
        ustPanel.Controls.Add(txtAra);
        ustPanel.Controls.Add(chkOdenmemis);
        ustPanel.Controls.Add(btnOde);
        ustPanel.Controls.Add(lblToplam);

        tablo.Dock = DockStyle.Fill;
        tablo.ReadOnly = true;
        tablo.AllowUserToAddRows = false;
        tablo.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        tablo.MultiSelect = false;
        tablo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        tablo.BackgroundColor = Color.White;

        Controls.Add(tablo);
        Controls.Add(ustPanel);

        txtAra.TextChanged += (s, e) => Listele();
        chkOdenmemis.CheckedChanged += (s, e) => Listele();
        Load += (s, e) => Listele();
    }

    private void Listele()
    {
        try
        {
            var liste = cezaRepository.Ara(txtAra.Text.Trim());
            if (chkOdenmemis.Checked)
            {
                liste = liste.Where(c => !c.Odendi).ToList();
            }
            tablo.DataSource = liste;
            tablo.Columns["OduncId"].Visible = false;
            tablo.Columns["UyeId"].Visible = false;
            lblToplam.Text = "Ödenmemiş toplam: " + cezaRepository.OdenmemisCezalar().Sum(c => c.Tutar).ToString("C2");
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnOde_Click(object sender, EventArgs e)
    {
        if (tablo.CurrentRow == null)
        {
            MessageBox.Show("Tablodan bir ceza seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var ceza = (Ceza)tablo.CurrentRow.DataBoundItem;
        if (ceza.Odendi)
        {
            MessageBox.Show("Bu ceza zaten ödenmiş.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        cezaRepository.Ode(ceza.Id);
        MessageBox.Show(ceza.UyeAdi + " adlı üyenin " + ceza.Tutar.ToString("C2") + " cezası ödendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        Listele();
    }
}
