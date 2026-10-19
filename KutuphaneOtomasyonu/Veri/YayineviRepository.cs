using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class YayineviRepository : IRepository<Yayinevi>
{
    public void Ekle(Yayinevi yayinevi)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("INSERT INTO yayinevleri (ad, telefon, adres) VALUES (@ad, @telefon, @adres)", baglanti);
        ParametreEkle(komut, yayinevi);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Yayinevi yayinevi)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE yayinevleri SET ad = @ad, telefon = @telefon, adres = @adres WHERE id = @id", baglanti);
        ParametreEkle(komut, yayinevi);
        komut.Parameters.AddWithValue("id", yayinevi.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM yayinevleri WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Yayinevi Getir(int id)
    {
        return TumunuGetir().FirstOrDefault(y => y.Id == id);
    }

    public List<Yayinevi> TumunuGetir()
    {
        return Ara("");
    }

    public List<Yayinevi> Ara(string metin)
    {
        var liste = new List<Yayinevi>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("SELECT id, ad, telefon, adres FROM yayinevleri WHERE ad ILIKE @metin ORDER BY ad", baglanti);
        komut.Parameters.AddWithValue("metin", "%" + metin + "%");
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Yayinevi
            {
                Id = okuyucu.GetInt32(0),
                Ad = okuyucu.GetString(1),
                Telefon = okuyucu.IsDBNull(2) ? "" : okuyucu.GetString(2),
                Adres = okuyucu.IsDBNull(3) ? "" : okuyucu.GetString(3)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Yayinevi yayinevi)
    {
        komut.Parameters.AddWithValue("ad", yayinevi.Ad);
        komut.Parameters.AddWithValue("telefon", yayinevi.Telefon);
        komut.Parameters.AddWithValue("adres", yayinevi.Adres);
    }
}
