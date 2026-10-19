using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class YazarRepository : IRepository<Yazar>
{
    public void Ekle(Yazar yazar)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("INSERT INTO yazarlar (ad, soyad, ulke, dogum_yili) VALUES (@ad, @soyad, @ulke, @dogumYili)", baglanti);
        ParametreEkle(komut, yazar);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Yazar yazar)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE yazarlar SET ad = @ad, soyad = @soyad, ulke = @ulke, dogum_yili = @dogumYili WHERE id = @id", baglanti);
        ParametreEkle(komut, yazar);
        komut.Parameters.AddWithValue("id", yazar.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM yazarlar WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Yazar Getir(int id)
    {
        return TumunuGetir().FirstOrDefault(y => y.Id == id);
    }

    public List<Yazar> TumunuGetir()
    {
        return Ara("");
    }

    public List<Yazar> Ara(string metin)
    {
        var liste = new List<Yazar>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("SELECT id, ad, soyad, ulke, dogum_yili FROM yazarlar WHERE ad || ' ' || soyad ILIKE @metin ORDER BY ad, soyad", baglanti);
        komut.Parameters.AddWithValue("metin", "%" + metin + "%");
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Yazar
            {
                Id = okuyucu.GetInt32(0),
                Ad = okuyucu.GetString(1),
                Soyad = okuyucu.GetString(2),
                Ulke = okuyucu.IsDBNull(3) ? "" : okuyucu.GetString(3),
                DogumYili = okuyucu.IsDBNull(4) ? 0 : okuyucu.GetInt32(4)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Yazar yazar)
    {
        komut.Parameters.AddWithValue("ad", yazar.Ad);
        komut.Parameters.AddWithValue("soyad", yazar.Soyad);
        komut.Parameters.AddWithValue("ulke", yazar.Ulke);
        komut.Parameters.AddWithValue("dogumYili", yazar.DogumYili);
    }
}
