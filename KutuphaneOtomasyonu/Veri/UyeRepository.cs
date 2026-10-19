using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class UyeRepository : IRepository<Uye>
{
    public void Ekle(Uye uye)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "INSERT INTO uyeler (tc_no, ad, soyad, telefon, eposta, adres, aktif) VALUES (@tcNo, @ad, @soyad, @telefon, @eposta, @adres, @aktif)", baglanti);
        ParametreEkle(komut, uye);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Uye uye)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "UPDATE uyeler SET tc_no = @tcNo, ad = @ad, soyad = @soyad, telefon = @telefon, eposta = @eposta, adres = @adres, aktif = @aktif WHERE id = @id", baglanti);
        ParametreEkle(komut, uye);
        komut.Parameters.AddWithValue("id", uye.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM uyeler WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Uye Getir(int id)
    {
        return TumunuGetir().FirstOrDefault(u => u.Id == id);
    }

    public List<Uye> TumunuGetir()
    {
        return Ara("");
    }

    public List<Uye> AktifUyeler()
    {
        return TumunuGetir().Where(u => u.Aktif).ToList();
    }

    public List<Uye> Ara(string metin)
    {
        var liste = new List<Uye>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "SELECT id, tc_no, ad, soyad, telefon, eposta, adres, kayit_tarihi, aktif FROM uyeler " +
            "WHERE ad || ' ' || soyad || ' ' || tc_no ILIKE @metin ORDER BY ad, soyad", baglanti);
        komut.Parameters.AddWithValue("metin", "%" + metin + "%");
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Uye
            {
                Id = okuyucu.GetInt32(0),
                TcNo = okuyucu.GetString(1),
                Ad = okuyucu.GetString(2),
                Soyad = okuyucu.GetString(3),
                Telefon = okuyucu.IsDBNull(4) ? "" : okuyucu.GetString(4),
                Eposta = okuyucu.IsDBNull(5) ? "" : okuyucu.GetString(5),
                Adres = okuyucu.IsDBNull(6) ? "" : okuyucu.GetString(6),
                KayitTarihi = okuyucu.GetDateTime(7),
                Aktif = okuyucu.GetBoolean(8)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Uye uye)
    {
        komut.Parameters.AddWithValue("tcNo", uye.TcNo);
        komut.Parameters.AddWithValue("ad", uye.Ad);
        komut.Parameters.AddWithValue("soyad", uye.Soyad);
        komut.Parameters.AddWithValue("telefon", uye.Telefon);
        komut.Parameters.AddWithValue("eposta", uye.Eposta);
        komut.Parameters.AddWithValue("adres", uye.Adres);
        komut.Parameters.AddWithValue("aktif", uye.Aktif);
    }
}
