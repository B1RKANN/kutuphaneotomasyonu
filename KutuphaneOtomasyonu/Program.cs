using KutuphaneOtomasyonu.Formlar;
using KutuphaneOtomasyonu.Veri;

namespace KutuphaneOtomasyonu;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!Veritabani.BaglantiTest())
        {
            MessageBox.Show("Veritabanına bağlanılamadı.\nÖnce 'docker compose up -d' komutu ile PostgreSQL'i başlatın.",
                "Bağlantı Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var girisForm = new GirisForm();
        if (girisForm.ShowDialog() == DialogResult.OK)
        {
            Application.Run(new AnaForm());
        }
    }
}
