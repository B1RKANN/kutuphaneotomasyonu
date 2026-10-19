using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class KategoriRepository : IRepository<Kategori>
{
    public void Ekle(Kategori kategori)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("INSERT INTO kategoriler (ad, aciklama) VALUES (@ad, @aciklama)", baglanti);
        komut.Parameters.AddWithValue("ad", kategori.Ad);
        komut.Parameters.AddWithValue("aciklama", kategori.Aciklama);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Kategori kategori)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE kategoriler SET ad = @ad, aciklama = @aciklama WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("ad", kategori.Ad);
        komut.Parameters.AddWithValue("aciklama", kategori.Aciklama);
        komut.Parameters.AddWithValue("id", kategori.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM kategoriler WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Kategori Getir(int id)
    {
        return TumunuGetir().FirstOrDefault(k => k.Id == id);
    }

    public List<Kategori> TumunuGetir()
    {
        return Ara("");
    }

    public List<Kategori> Ara(string metin)
    {
        var liste = new List<Kategori>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("SELECT id, ad, aciklama FROM kategoriler WHERE ad ILIKE @metin ORDER BY ad", baglanti);
        komut.Parameters.AddWithValue("metin", "%" + metin + "%");
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Kategori
            {
                Id = okuyucu.GetInt32(0),
                Ad = okuyucu.GetString(1),
                Aciklama = okuyucu.IsDBNull(2) ? "" : okuyucu.GetString(2)
            });
        }
        return liste;
    }
}
