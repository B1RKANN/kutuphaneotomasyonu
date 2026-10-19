using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;
using NpgsqlTypes;

namespace KutuphaneOtomasyonu.Veri;

public class OduncRepository : IRepository<OduncIslemi>
{
    private const string SecimSorgusu =
        "SELECT o.id, o.kitap_id, o.uye_id, o.personel_id, o.odunc_tarihi, o.son_teslim_tarihi, o.iade_tarihi, " +
        "k.ad, u.ad || ' ' || u.soyad, p.ad || ' ' || p.soyad " +
        "FROM odunc_islemleri o " +
        "JOIN kitaplar k ON k.id = o.kitap_id " +
        "JOIN uyeler u ON u.id = o.uye_id " +
        "JOIN personeller p ON p.id = o.personel_id ";

    public void Ekle(OduncIslemi odunc)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "INSERT INTO odunc_islemleri (kitap_id, uye_id, personel_id, odunc_tarihi, son_teslim_tarihi, iade_tarihi) " +
            "VALUES (@kitapId, @uyeId, @personelId, @oduncTarihi, @sonTeslimTarihi, @iadeTarihi)", baglanti);
        ParametreEkle(komut, odunc);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(OduncIslemi odunc)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "UPDATE odunc_islemleri SET kitap_id = @kitapId, uye_id = @uyeId, personel_id = @personelId, odunc_tarihi = @oduncTarihi, " +
            "son_teslim_tarihi = @sonTeslimTarihi, iade_tarihi = @iadeTarihi WHERE id = @id", baglanti);
        ParametreEkle(komut, odunc);
        komut.Parameters.AddWithValue("id", odunc.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM odunc_islemleri WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public OduncIslemi Getir(int id)
    {
        return Listele(SecimSorgusu + "WHERE o.id = @deger", id).FirstOrDefault();
    }

    public List<OduncIslemi> TumunuGetir()
    {
        return Ara("");
    }

    public List<OduncIslemi> Ara(string metin)
    {
        return Listele(SecimSorgusu + "WHERE k.ad || ' ' || u.ad || ' ' || u.soyad ILIKE @deger ORDER BY o.odunc_tarihi DESC, o.id DESC", "%" + metin + "%");
    }

    public List<OduncIslemi> AktifOduncler()
    {
        return TumunuGetir().Where(o => !o.IadeEdildiMi).ToList();
    }

    public List<OduncIslemi> GecikenOduncler()
    {
        return AktifOduncler().Where(o => o.GecikmeGunu(DateTime.Today) > 0).ToList();
    }

    public int UyeninAktifOduncSayisi(int uyeId)
    {
        return AktifOduncler().Count(o => o.UyeId == uyeId);
    }

    public void IadeEt(int id, DateTime iadeTarihi)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE odunc_islemleri SET iade_tarihi = @iadeTarihi WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("iadeTarihi", NpgsqlDbType.Date, iadeTarihi);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public List<KitapOkunma> EnCokOkunanlar(int adet)
    {
        var liste = new List<KitapOkunma>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "SELECT k.ad, y.ad || ' ' || y.soyad, COUNT(o.id) FROM odunc_islemleri o " +
            "JOIN kitaplar k ON k.id = o.kitap_id JOIN yazarlar y ON y.id = k.yazar_id " +
            "GROUP BY k.ad, y.ad, y.soyad ORDER BY COUNT(o.id) DESC LIMIT @adet", baglanti);
        komut.Parameters.AddWithValue("adet", adet);
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new KitapOkunma
            {
                KitapAdi = okuyucu.GetString(0),
                YazarAdi = okuyucu.GetString(1),
                OkunmaSayisi = Convert.ToInt32(okuyucu.GetInt64(2))
            });
        }
        return liste;
    }

    private List<OduncIslemi> Listele(string sql, object deger)
    {
        var liste = new List<OduncIslemi>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(sql, baglanti);
        komut.Parameters.AddWithValue("deger", deger);
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new OduncIslemi
            {
                Id = okuyucu.GetInt32(0),
                KitapId = okuyucu.GetInt32(1),
                UyeId = okuyucu.GetInt32(2),
                PersonelId = okuyucu.GetInt32(3),
                OduncTarihi = okuyucu.GetDateTime(4),
                SonTeslimTarihi = okuyucu.GetDateTime(5),
                IadeTarihi = okuyucu.IsDBNull(6) ? null : okuyucu.GetDateTime(6),
                KitapAdi = okuyucu.GetString(7),
                UyeAdi = okuyucu.GetString(8),
                PersonelAdi = okuyucu.GetString(9)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, OduncIslemi odunc)
    {
        komut.Parameters.AddWithValue("kitapId", odunc.KitapId);
        komut.Parameters.AddWithValue("uyeId", odunc.UyeId);
        komut.Parameters.AddWithValue("personelId", odunc.PersonelId);
        komut.Parameters.AddWithValue("oduncTarihi", NpgsqlDbType.Date, odunc.OduncTarihi);
        komut.Parameters.AddWithValue("sonTeslimTarihi", NpgsqlDbType.Date, odunc.SonTeslimTarihi);
        komut.Parameters.AddWithValue("iadeTarihi", NpgsqlDbType.Date, odunc.IadeTarihi.HasValue ? odunc.IadeTarihi.Value : DBNull.Value);
    }
}
