namespace KutuphaneOtomasyonu.Arayuzler;

public interface IRepository<T>
{
    void Ekle(T nesne);
    void Guncelle(T nesne);
    void Sil(int id);
    T Getir(int id);
    List<T> TumunuGetir();
    List<T> Ara(string metin);
}
