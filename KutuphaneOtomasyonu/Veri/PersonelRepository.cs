using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class PersonelRepository : IRepository<Personel>
{
    public void Ekle(Personel personel)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "INSERT INTO personeller (tc_no, ad, soyad, telefon, eposta, kullanici_adi, sifre, rol) " +
            "VALUES (@tcNo, @ad, @soyad, @telefon, @eposta, @kullaniciAdi, @sifre, @rol)", baglanti);
        ParametreEkle(komut, personel);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Personel personel)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "UPDATE personeller SET tc_no = @tcNo, ad = @ad, soyad = @soyad, telefon = @telefon, eposta = @eposta, " +
            "kullanici_adi = @kullaniciAdi, sifre = @sifre, rol = @rol WHERE id = @id", baglanti);
        ParametreEkle(komut, personel);
        komut.Parameters.AddWithValue("id", personel.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM personeller WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Personel Getir(int id)
    {
        return TumunuGetir().FirstOrDefault(p => p.Id == id);
    }

    public List<Personel> TumunuGetir()
    {
        return Ara("");
    }

    public Personel KullaniciAdiIleGetir(string kullaniciAdi)
    {
        return TumunuGetir().FirstOrDefault(p => p.KullaniciAdi == kullaniciAdi);
    }

    public List<Personel> Ara(string metin)
    {
        var liste = new List<Personel>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "SELECT id, tc_no, ad, soyad, telefon, eposta, kullanici_adi, sifre, rol FROM personeller " +
            "WHERE ad || ' ' || soyad || ' ' || kullanici_adi ILIKE @metin ORDER BY ad, soyad", baglanti);
        komut.Parameters.AddWithValue("metin", "%" + metin + "%");
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Personel
            {
                Id = okuyucu.GetInt32(0),
                TcNo = okuyucu.GetString(1),
                Ad = okuyucu.GetString(2),
                Soyad = okuyucu.GetString(3),
                Telefon = okuyucu.IsDBNull(4) ? "" : okuyucu.GetString(4),
                Eposta = okuyucu.IsDBNull(5) ? "" : okuyucu.GetString(5),
                KullaniciAdi = okuyucu.GetString(6),
                Sifre = okuyucu.GetString(7),
                Rol = okuyucu.GetString(8)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Personel personel)
    {
        komut.Parameters.AddWithValue("tcNo", personel.TcNo);
        komut.Parameters.AddWithValue("ad", personel.Ad);
        komut.Parameters.AddWithValue("soyad", personel.Soyad);
        komut.Parameters.AddWithValue("telefon", personel.Telefon);
        komut.Parameters.AddWithValue("eposta", personel.Eposta);
        komut.Parameters.AddWithValue("kullaniciAdi", personel.KullaniciAdi);
        komut.Parameters.AddWithValue("sifre", personel.Sifre);
        komut.Parameters.AddWithValue("rol", personel.Rol);
    }
}
