using UnityEngine;
using UnityEditor;

public class SekilUretici
{
#if UNITY_EDITOR
    [MenuItem("BuildBattle/Hazır Şekilleri Üret (Devasa Paket)!")]
    public static void OtomatikSekilleriOlustur()
    {
        string klasorYolu = "Assets/Resources/HazirSekiller";

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        if (!AssetDatabase.IsValidFolder("Assets/Resources/HazirSekiller"))
            AssetDatabase.CreateFolder("Assets/Resources", "HazirSekiller");

        // ================= 1. BÖLÜM: SENİN KLASİK ŞEKİLLERİN =================

        // --- ŞEKİL 1: 5 BLOKLUK MERKEZ KULE ---
        SekilVerisi3D kule = YeniSekilOlustur("Merkez Kule", 5);
        for (int y = 0; y < 5; y++) BlokKoy(kule, 2, y, 2, (y == 0) ? 2 : 3);
        Kaydet(kule, "Kule", klasorYolu);

        // --- ŞEKİL 2: 3 KATLI PİRAMİT ---
        SekilVerisi3D piramit = YeniSekilOlustur("Piramit", 3);
        for(int z=0; z<5; z++) for(int x=0; x<5; x++) BlokKoy(piramit, x, 0, z, 2); // Zemin
        for(int z=1; z<4; z++) for(int x=1; x<4; x++) BlokKoy(piramit, x, 1, z, 1); // Orta
        BlokKoy(piramit, 2, 2, 2, 0); // Tepe
        Kaydet(piramit, "Piramit", klasorYolu);

        // --- ŞEKİL 3: SAVUNMA DUVARI ---
        SekilVerisi3D duvar = YeniSekilOlustur("Savunma Duvarı", 3);
        for (int y = 0; y < 3; y++)
            for (int x = 0; x < 5; x++)
                BlokKoy(duvar, x, y, 0, (y == 2) ? 3 : 2);
        Kaydet(duvar, "Duvar", klasorYolu);

        // --- ŞEKİL 4: L MERDİVEN ---
        SekilVerisi3D merdiven = YeniSekilOlustur("Basamaklı Merdiven", 3);
        for (int y = 0; y < 3; y++) BlokKoy(merdiven, y, y, 2, 0);
        Kaydet(merdiven, "Merdiven", klasorYolu);

        // --- ŞEKİL 5: U ŞEKLİ SİPER ---
        SekilVerisi3D siper = YeniSekilOlustur("Siper (U Şekli)", 1);
        for (int z = 0; z < 3; z++) { BlokKoy(siper, 0, 0, z, 3); BlokKoy(siper, 4, 0, z, 3); }
        for (int x = 0; x < 5; x++) BlokKoy(siper, x, 0, 0, 3);
        Kaydet(siper, "Siper", klasorYolu);


        // ================= 2. BÖLÜM: YENİ EFSANE ŞEKİLLER =================

        // --- ŞEKİL 6: TAŞ KÖPRÜ ---
        SekilVerisi3D kopru = YeniSekilOlustur("Taş Köprü", 3);
        for (int z = 0; z < 5; z++) { BlokKoy(kopru, 1, 0, z, 2); BlokKoy(kopru, 3, 0, z, 2); } // Ayaklar
        for (int z = 0; z < 5; z++) BlokKoy(kopru, 2, 1, z, 0); // Yürüyüş yolu
        for (int z = 0; z < 5; z++) { BlokKoy(kopru, 1, 2, z, 1); BlokKoy(kopru, 3, 2, z, 1); } // Korkuluklar
        Kaydet(kopru, "TasKopru", klasorYolu);

        // --- ŞEKİL 7: TUĞLA ŞÖMİNE ---
        SekilVerisi3D somine = YeniSekilOlustur("Tuğla Şömine", 4);
        for (int x = 1; x <= 3; x++) for (int z = 2; z <= 4; z++) BlokKoy(somine, x, 0, z, 3);
        BlokKoy(somine, 1, 1, 3, 3); BlokKoy(somine, 3, 1, 3, 3);
        BlokKoy(somine, 1, 1, 4, 3); BlokKoy(somine, 2, 1, 4, 3); BlokKoy(somine, 3, 1, 4, 3);
        for (int x = 1; x <= 3; x++) for (int z = 3; z <= 4; z++) BlokKoy(somine, x, 2, z, 3);
        BlokKoy(somine, 2, 3, 4, 3); // Baca
        Kaydet(somine, "Somine", klasorYolu);

        // --- ŞEKİL 8: DEV HAZİNE SANDIĞI ---
        SekilVerisi3D sandik = YeniSekilOlustur("Hazine Sandığı", 3);
        for (int y = 0; y <= 2; y++)
        {
            for (int x = 1; x <= 3; x++)
            {
                for (int z = 1; z <= 3; z++)
                {
                    if (y == 1 && x == 2 && z == 2) continue; // İçi boş
                    if ((x == 1 || x == 3) && (z == 1 || z == 3)) BlokKoy(sandik, x, y, z, 1); // Köşeler Meşe
                    else BlokKoy(sandik, x, y, z, 0); // Geri kalan Tahta
                }
            }
        }
        Kaydet(sandik, "HazineSandigi", klasorYolu);

        // --- ŞEKİL 9: PİKNİK MASASI ---
        SekilVerisi3D masa = YeniSekilOlustur("Piknik Masası", 2);
        BlokKoy(masa, 1, 0, 1, 1); BlokKoy(masa, 3, 0, 1, 1); // Ayaklar
        BlokKoy(masa, 1, 0, 3, 1); BlokKoy(masa, 3, 0, 3, 1);
        for (int x = 0; x <= 4; x++) for (int z = 1; z <= 3; z++) BlokKoy(masa, x, 1, z, 0); // Tabla
        Kaydet(masa, "PiknikMasasi", klasorYolu);

        // --- ŞEKİL 10: KORSAN GEMİSİ ---
        SekilVerisi3D gemi = YeniSekilOlustur("Korsan Gemisi", 4);
        for (int z = 0; z <= 4; z++) BlokKoy(gemi, 2, 0, z, 1); // Omurga
        for (int z = 1; z <= 3; z++) { BlokKoy(gemi, 1, 1, z, 0); BlokKoy(gemi, 3, 1, z, 0); } // Gövde
        BlokKoy(gemi, 2, 1, 0, 0); BlokKoy(gemi, 2, 1, 4, 0);
        BlokKoy(gemi, 2, 1, 2, 1); BlokKoy(gemi, 2, 2, 2, 1); BlokKoy(gemi, 2, 3, 2, 1); // Direk
        Kaydet(gemi, "KorsanGemisi", klasorYolu);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("<color=cyan><b>İŞLEM TAMAM!</b></color> Hem eski klasik şekillerin hem de yeni devasa şekiller 'HazirSekiller' konumuna üretildi!");
    }

    // ================= YARDIMCI FONKSİYONLAR =================
    // Senin kodunun altyapısına özel tasarlandı!

    static SekilVerisi3D YeniSekilOlustur(string isim, int katmanSayisi)
    {
        SekilVerisi3D sekil = ScriptableObject.CreateInstance<SekilVerisi3D>();
        sekil.sekilAdi = isim;
        for (int i = 0; i < katmanSayisi; i++)
        {
            sekil.YeniKatmanEkle();
        }
        return sekil;
    }

    // Uzun uzun satirlar[].sutunlar[] yazmamak için güvenli atama fonksiyonu
    static void BlokKoy(SekilVerisi3D sekil, int x, int y, int z, int tip)
    {
        if (y >= 0 && y < sekil.katmanlar.Count && z >= 0 && z < 5 && x >= 0 && x < 5)
        {
            sekil.katmanlar[y].satirlar[z].sutunlar[x] = tip;
        }
    }

    static void Kaydet(SekilVerisi3D sekil, string dosyaAdi, string yol)
    {
        AssetDatabase.CreateAsset(sekil, $"{yol}/{dosyaAdi}.asset");
    }
#endif
}