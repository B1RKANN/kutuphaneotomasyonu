using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public class KitapRepository : IRepository<Kitap>
{
    private const string SecimSorgusu =
        "SELECT k.id, k.isbn, k.ad, k.yazar_id, k.kategori_id, k.yayinevi_id, k.basim_yili, k.sayfa_sayisi, " +
        "k.stok, k.mevcut, k.raf_no, y.ad || ' ' || y.soyad, c.ad, p.ad " +
        "FROM kitaplar k " +
        "JOIN yazarlar y ON y.id = k.yazar_id " +
        "JOIN kategoriler c ON c.id = k.kategori_id " +
        "JOIN yayinevleri p ON p.id = k.yayinevi_id ";

    public void Ekle(Kitap kitap)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "INSERT INTO kitaplar (isbn, ad, yazar_id, kategori_id, yayinevi_id, basim_yili, sayfa_sayisi, stok, mevcut, raf_no) " +
            "VALUES (@isbn, @ad, @yazarId, @kategoriId, @yayineviId, @basimYili, @sayfaSayisi, @stok, @stok, @rafNo)", baglanti);
        ParametreEkle(komut, kitap);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Kitap kitap)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "UPDATE kitaplar SET isbn = @isbn, ad = @ad, yazar_id = @yazarId, kategori_id = @kategoriId, yayinevi_id = @yayineviId, " +
            "basim_yili = @basimYili, sayfa_sayisi = @sayfaSayisi, mevcut = mevcut + (@stok - stok), stok = @stok, raf_no = @rafNo WHERE id = @id", baglanti);
        ParametreEkle(komut, kitap);
        komut.Parameters.AddWithValue("id", kitap.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM kitaplar WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Kitap Getir(int id)
    {
        return Listele(SecimSorgusu + "WHERE k.id = @deger", id).FirstOrDefault();
    }

    public List<Kitap> TumunuGetir()
    {
        return Ara("");
    }

    public List<Kitap> Ara(string metin)
    {
        return Listele(SecimSorgusu + "WHERE k.ad || ' ' || k.isbn || ' ' || y.ad || ' ' || y.soyad ILIKE @deger ORDER BY k.ad", "%" + metin + "%");
    }

    public List<Kitap> MevcutKitaplar()
    {
        return TumunuGetir().Where(k => k.OduncVerilebilirMi()).ToList();
    }

    public void MevcutDegistir(int kitapId, int miktar)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE kitaplar SET mevcut = mevcut + @miktar WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("miktar", miktar);
        komut.Parameters.AddWithValue("id", kitapId);
        komut.ExecuteNonQuery();
    }

    private List<Kitap> Listele(string sql, object deger)
    {
        var liste = new List<Kitap>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(sql, baglanti);
        komut.Parameters.AddWithValue("deger", deger);
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Kitap
            {
                Id = okuyucu.GetInt32(0),
                Isbn = okuyucu.GetString(1),
                Ad = okuyucu.GetString(2),
                YazarId = okuyucu.GetInt32(3),
                KategoriId = okuyucu.GetInt32(4),
                YayineviId = okuyucu.GetInt32(5),
                BasimYili = okuyucu.IsDBNull(6) ? 0 : okuyucu.GetInt32(6),
                SayfaSayisi = okuyucu.IsDBNull(7) ? 0 : okuyucu.GetInt32(7),
                Stok = okuyucu.GetInt32(8),
                Mevcut = okuyucu.GetInt32(9),
                RafNo = okuyucu.IsDBNull(10) ? "" : okuyucu.GetString(10),
                YazarAdi = okuyucu.GetString(11),
                KategoriAdi = okuyucu.GetString(12),
                YayineviAdi = okuyucu.GetString(13)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Kitap kitap)
    {
        komut.Parameters.AddWithValue("isbn", kitap.Isbn);
        komut.Parameters.AddWithValue("ad", kitap.Ad);
        komut.Parameters.AddWithValue("yazarId", kitap.YazarId);
        komut.Parameters.AddWithValue("kategoriId", kitap.KategoriId);
        komut.Parameters.AddWithValue("yayineviId", kitap.YayineviId);
        komut.Parameters.AddWithValue("basimYili", kitap.BasimYili);
        komut.Parameters.AddWithValue("sayfaSayisi", kitap.SayfaSayisi);
        komut.Parameters.AddWithValue("stok", kitap.Stok);
        komut.Parameters.AddWithValue("rafNo", kitap.RafNo);
    }
}
