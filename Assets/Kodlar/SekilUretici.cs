using UnityEngine;
using UnityEditor;
using System.IO;

public class SekilUretici
{
    [MenuItem("BuildBattle/Hazır Şekilleri Üret!")]
    public static void OtomatikSekilleriOlustur()
    {
        // Yolu Resources klasörü olarak güncelledik
        string klasorYolu = "Assets/Resources/HazirSekiller";

        // Resources klasörü yoksa oluştur
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        // HazirSekiller klasörü yoksa oluştur
        if (!AssetDatabase.IsValidFolder("Assets/Resources/HazirSekiller"))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "HazirSekiller");
        }

        // --- ŞEKİL 1: 5 BLOKLUK MERKEZ KULE ---
        SekilVerisi3D kule = ScriptableObject.CreateInstance<SekilVerisi3D>();
        kule.sekilAdi = "Merkez Kule";
        for (int y = 0; y < 5; y++) 
        {
            kule.YeniKatmanEkle();
            kule.katmanlar[y].satirlar[2].sutunlar[2] = true; 
        }
        AssetDatabase.CreateAsset(kule, $"{klasorYolu}/Kule.asset");

        // --- ŞEKİL 2: 3 KATLI PİRAMİT ---
        SekilVerisi3D piramit = ScriptableObject.CreateInstance<SekilVerisi3D>();
        piramit.sekilAdi = "Piramit";
        
        piramit.YeniKatmanEkle();
        for(int z=0; z<5; z++) for(int x=0; x<5; x++) piramit.katmanlar[0].satirlar[z].sutunlar[x] = true;
        
        piramit.YeniKatmanEkle();
        for(int z=1; z<4; z++) for(int x=1; x<4; x++) piramit.katmanlar[1].satirlar[z].sutunlar[x] = true;

        piramit.YeniKatmanEkle();
        piramit.katmanlar[2].satirlar[2].sutunlar[2] = true;
        
        AssetDatabase.CreateAsset(piramit, $"{klasorYolu}/Piramit.asset");

        // --- ŞEKİL 3: SAVUNMA DUVARI (5 Genişlik, 3 Yükseklik) ---
        SekilVerisi3D duvar = ScriptableObject.CreateInstance<SekilVerisi3D>();
        duvar.sekilAdi = "Savunma Duvarı";
        for (int y = 0; y < 3; y++)
        {
            duvar.YeniKatmanEkle();
            for (int x = 0; x < 5; x++)
            {
                duvar.katmanlar[y].satirlar[0].sutunlar[x] = true; 
            }
        }
        AssetDatabase.CreateAsset(duvar, $"{klasorYolu}/Duvar.asset");

        // --- ŞEKİL 4: L MERDİVEN ---
        SekilVerisi3D merdiven = ScriptableObject.CreateInstance<SekilVerisi3D>();
        merdiven.sekilAdi = "Basamaklı Merdiven";
        for (int y = 0; y < 3; y++)
        {
            merdiven.YeniKatmanEkle();
            merdiven.katmanlar[y].satirlar[2].sutunlar[y] = true; 
        }
        AssetDatabase.CreateAsset(merdiven, $"{klasorYolu}/Merdiven.asset");

        // --- ŞEKİL 5: U ŞEKLİ (SİPER) ---
        SekilVerisi3D siper = ScriptableObject.CreateInstance<SekilVerisi3D>();
        siper.sekilAdi = "Siper (U Şekli)";
        siper.YeniKatmanEkle(); 
        for (int z = 0; z < 3; z++)
        {
            siper.katmanlar[0].satirlar[z].sutunlar[0] = true; 
            siper.katmanlar[0].satirlar[z].sutunlar[4] = true; 
        }
        for (int x = 0; x < 5; x++) siper.katmanlar[0].satirlar[0].sutunlar[x] = true; 
        AssetDatabase.CreateAsset(siper, $"{klasorYolu}/Siper.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Şekiller Assets/Resources/HazirSekiller konumuna üretildi!");
    }
}