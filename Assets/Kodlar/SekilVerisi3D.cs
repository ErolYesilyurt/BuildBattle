using UnityEngine;
using System.Collections.Generic;

// struct yerine class yaptık!
[System.Serializable]
public class Satir 
{
    // -1 = Boş | 0 = Plank | 1 = Oak | 2 = Cobblestone | 3 = Brick
    public int[] sutunlar; 
}

// struct yerine class yaptık!
[System.Serializable]
public class Katman 
{
    public string katmanAdi; 
    public Satir[] satirlar;
}

[CreateAssetMenu(fileName = "Yeni3DSekil", menuName = "BuildBattle/3D Sekil Modeli")]
public class SekilVerisi3D : ScriptableObject
{
    public string sekilAdi = "Yeni 3D Model";
    
    [Header("Yükseklik Katmanları (Aşağıdan Yukarıya)")]
    public List<Katman> katmanlar = new List<Katman>();

    [ContextMenu("Yeni Yükseklik Katmanı Ekle (5x5)")]
    public void YeniKatmanEkle()
    {
        Katman yeniKatman = new Katman();
        yeniKatman.katmanAdi = "Yükseklik Katmanı " + katmanlar.Count;
        yeniKatman.satirlar = new Satir[5];
        
        for (int z = 0; z < 5; z++)
        {
            yeniKatman.satirlar[z] = new Satir(); // Class olduğu için bunu eklememiz gerekti
            yeniKatman.satirlar[z].sutunlar = new int[5]; 
            
            for (int x = 0; x < 5; x++)
            {
                yeniKatman.satirlar[z].sutunlar[x] = -1;
            }
        }
        
        katmanlar.Add(yeniKatman);
    }

    public int BlokTipiniGetir(int x, int y, int z)
    {
        if (y < 0 || y >= katmanlar.Count) return -1;
        if (katmanlar[y].satirlar == null || katmanlar[y].satirlar.Length <= z) return -1;
        if (katmanlar[y].satirlar[z].sutunlar == null || katmanlar[y].satirlar[z].sutunlar.Length <= x) return -1;
        
        return katmanlar[y].satirlar[z].sutunlar[x];
    }
}