using Npgsql;

namespace KutuphaneOtomasyonu.Veri;

public static class Veritabani
{
    public static string BaglantiCumlesi = "Host=localhost;Port=5433;Database=kutuphane;Username=kutuphane;Password=kutuphane123";

    public static NpgsqlConnection BaglantiAc()
    {
        var baglanti = new NpgsqlConnection(BaglantiCumlesi);
        baglanti.Open();
        return baglanti;
    }

    public static bool BaglantiTest()
    {
        try
        {
            using var baglanti = BaglantiAc();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static int SayiGetir(string sql)
    {
        using var baglanti = BaglantiAc();
        using var komut = new NpgsqlCommand(sql, baglanti);
        return Convert.ToInt32(komut.ExecuteScalar());
    }
}
