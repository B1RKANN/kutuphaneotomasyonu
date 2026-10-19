using KutuphaneOtomasyonu.Arayuzler;
using KutuphaneOtomasyonu.Modeller;
using Npgsql;
using NpgsqlTypes;

namespace KutuphaneOtomasyonu.Veri;

public class CezaRepository : IRepository<Ceza>
{
    private const string SecimSorgusu =
        "SELECT c.id, c.odunc_id, c.uye_id, c.gecikme_gunu, c.tutar, c.odendi, c.odeme_tarihi, u.ad || ' ' || u.soyad, k.ad " +
        "FROM cezalar c " +
        "JOIN uyeler u ON u.id = c.uye_id " +
        "JOIN odunc_islemleri o ON o.id = c.odunc_id " +
        "JOIN kitaplar k ON k.id = o.kitap_id ";

    public void Ekle(Ceza ceza)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "INSERT INTO cezalar (odunc_id, uye_id, gecikme_gunu, tutar, odendi, odeme_tarihi) " +
            "VALUES (@oduncId, @uyeId, @gecikmeGunu, @tutar, @odendi, @odemeTarihi)", baglanti);
        ParametreEkle(komut, ceza);
        komut.ExecuteNonQuery();
    }

    public void Guncelle(Ceza ceza)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(
            "UPDATE cezalar SET odunc_id = @oduncId, uye_id = @uyeId, gecikme_gunu = @gecikmeGunu, tutar = @tutar, " +
            "odendi = @odendi, odeme_tarihi = @odemeTarihi WHERE id = @id", baglanti);
        ParametreEkle(komut, ceza);
        komut.Parameters.AddWithValue("id", ceza.Id);
        komut.ExecuteNonQuery();
    }

    public void Sil(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("DELETE FROM cezalar WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    public Ceza Getir(int id)
    {
        return Listele(SecimSorgusu + "WHERE c.id = @deger", id).FirstOrDefault();
    }

    public List<Ceza> TumunuGetir()
    {
        return Ara("");
    }

    public List<Ceza> Ara(string metin)
    {
        return Listele(SecimSorgusu + "WHERE u.ad || ' ' || u.soyad || ' ' || k.ad ILIKE @deger ORDER BY c.odendi, c.id DESC", "%" + metin + "%");
    }

    public List<Ceza> OdenmemisCezalar()
    {
        return TumunuGetir().Where(c => !c.Odendi).ToList();
    }

    public decimal UyeninOdenmemisCezasi(int uyeId)
    {
        return OdenmemisCezalar().Where(c => c.UyeId == uyeId).Sum(c => c.Tutar);
    }

    public void Ode(int id)
    {
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand("UPDATE cezalar SET odendi = TRUE, odeme_tarihi = CURRENT_DATE WHERE id = @id", baglanti);
        komut.Parameters.AddWithValue("id", id);
        komut.ExecuteNonQuery();
    }

    private List<Ceza> Listele(string sql, object deger)
    {
        var liste = new List<Ceza>();
        using var baglanti = Veritabani.BaglantiAc();
        using var komut = new NpgsqlCommand(sql, baglanti);
        komut.Parameters.AddWithValue("deger", deger);
        using var okuyucu = komut.ExecuteReader();
        while (okuyucu.Read())
        {
            liste.Add(new Ceza
            {
                Id = okuyucu.GetInt32(0),
                OduncId = okuyucu.GetInt32(1),
                UyeId = okuyucu.GetInt32(2),
                GecikmeGunu = okuyucu.GetInt32(3),
                Tutar = okuyucu.GetDecimal(4),
                Odendi = okuyucu.GetBoolean(5),
                OdemeTarihi = okuyucu.IsDBNull(6) ? null : okuyucu.GetDateTime(6),
                UyeAdi = okuyucu.GetString(7),
                KitapAdi = okuyucu.GetString(8)
            });
        }
        return liste;
    }

    private void ParametreEkle(NpgsqlCommand komut, Ceza ceza)
    {
        komut.Parameters.AddWithValue("oduncId", ceza.OduncId);
        komut.Parameters.AddWithValue("uyeId", ceza.UyeId);
        komut.Parameters.AddWithValue("gecikmeGunu", ceza.GecikmeGunu);
        komut.Parameters.AddWithValue("tutar", ceza.Tutar);
        komut.Parameters.AddWithValue("odendi", ceza.Odendi);
        komut.Parameters.AddWithValue("odemeTarihi", NpgsqlDbType.Date, ceza.OdemeTarihi.HasValue ? ceza.OdemeTarihi.Value : DBNull.Value);
    }
}
