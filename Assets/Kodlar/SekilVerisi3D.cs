using UnityEngine;
using System.Collections.Generic;

// X Ekseni (5 Sütun)
[System.Serializable]
public struct Satir
{
    public bool[] sutunlar;
}

// Z Ekseni (5 Satır) ve Y Ekseni İsmilendirmesi
[System.Serializable]
public struct Katman
{
    public string katmanAdi; // Inspector'da hangi yükseklikte olduğumuzu görmek için
    public Satir[] satirlar;
}

// Unity menüsüne veri oluşturma seçeneği ekliyoruz
[CreateAssetMenu(fileName = "Yeni3DSekil", menuName = "BuildBattle/3D Sekil Modeli")]
public class SekilVerisi3D : ScriptableObject
{
    public string sekilAdi = "Yeni 3D Model";
    
    [Header("Yükseklik Katmanları (Aşağıdan Yukarıya)")]
    // Yüksekliğin sınırı yok, liste olarak tutuyoruz
    public List<Katman> katmanlar = new List<Katman>();

    // SİHİRLİ KISIM: Inspector'da sağ tıklayıp otomatik 5x5 katman eklememizi sağlar
    [ContextMenu("Yeni Yükseklik Katmanı Ekle (5x5)")]
    public void YeniKatmanEkle()
    {
        Katman yeniKatman = new Katman();
        yeniKatman.katmanAdi = "Yükseklik Katmanı " + katmanlar.Count;
        yeniKatman.satirlar = new Satir[5];
        
        for (int i = 0; i < 5; i++)
        {
            yeniKatman.satirlar[i].sutunlar = new bool[5]; // 5 kutucuklu sütun
        }
        
        katmanlar.Add(yeniKatman);
    }

    // Oyun içinde (X, Y, Z) koordinatına göre orada blok olup olmadığını soran fonksiyon
    public bool BlokVarMi(int x, int y, int z)
    {
        // Önce yüksekliği (Y) kontrol et
        if (y < 0 || y >= katmanlar.Count) return false;
        
        // Sonra 5x5 taban sınırlarını (X ve Z) kontrol et
        if (z < 0 || z >= 5 || x < 0 || x >= 5) return false;
        
        return katmanlar[y].satirlar[z].sutunlar[x];
    }
}