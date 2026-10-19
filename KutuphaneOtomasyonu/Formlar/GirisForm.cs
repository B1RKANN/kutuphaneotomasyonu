using KutuphaneOtomasyonu.Servisler;

namespace KutuphaneOtomasyonu.Formlar;

public class GirisForm : Form
{
    private readonly GirisServisi girisServisi = new GirisServisi();
    private TextBox txtKullaniciAdi = new TextBox { Width = 220 };
    private TextBox txtSifre = new TextBox { Width = 220, UseSystemPasswordChar = true };

    public GirisForm()
    {
        Text = "Kütüphane Otomasyonu - Giriş";
        Size = new Size(420, 300);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        Font = new Font("Segoe UI", 10);

        var lblBaslik = new Label { Text = "KÜTÜPHANE OTOMASYONU", Font = new Font("Segoe UI", 14, FontStyle.Bold), AutoSize = false, Width = 400, Height = 40, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 15) };
        var lblKullanici = new Label { Text = "Kullanıcı Adı:", AutoSize = true, Location = new Point(30, 85) };
        var lblSifre = new Label { Text = "Şifre:", AutoSize = true, Location = new Point(30, 125) };
        txtKullaniciAdi.Location = new Point(140, 82);
        txtSifre.Location = new Point(140, 122);

        var btnGiris = new Button { Text = "Giriş Yap", Width = 220, Height = 38, Location = new Point(140, 170), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnGiris.Click += BtnGiris_Click;
        AcceptButton = btnGiris;

        Controls.AddRange(new Control[] { lblBaslik, lblKullanici, lblSifre, txtKullaniciAdi, txtSifre, btnGiris });
    }

    private void BtnGiris_Click(object sender, EventArgs e)
    {
        try
        {
            if (girisServisi.GirisYap(txtKullaniciAdi.Text.Trim(), txtSifre.Text))
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre hatalı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSifre.Clear();
                txtSifre.Focus();
            }
        }
        catch (Exception hata)
        {
            MessageBox.Show(hata.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
