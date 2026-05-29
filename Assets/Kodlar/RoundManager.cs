using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI; // UI elementleri (Toggle vb.) için gerekli

public class RoundManager : MonoBehaviour
{
    public enum GameState { MainMenu, Observe, Build, RoundEnd } 
    public GameState currentState;

    [Header("Oyun Modu Ayarları")]
    public bool isSoloMode;
    public bool isRandomModeOn; // Random modun açık/kapalı durumunu tutar

    [Header("UI ve Kamera Ayarları")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public TextMeshProUGUI infoText; 
    public Camera camP1;
    public Camera camP2;

    [Header("Buton Grupları (Add/Delete)")]
    public GameObject p1ButtonContainer; 
    public GameObject p2ButtonContainer;

    [Header("Süre Ayarları")]
    public float observeTime = 15f; 
    public float buildTime = 60f;   
    private float _timer;

    [Header("Kontrolcüler")]
    public GameObject player1Controller; 
    public GameObject player2Controller; 

    [Header("Oyuncu ve Skor Ayarları")]
    public GridManager player1Grid;
    public GridManager player2Grid;
    public int p1Wins;
    public int p2Wins;
    public int winTarget = 3; // Sadece Duo modda geçerli olacak

    [Header("Örnek Şekil Gösterim Ayarları")]
    public GameObject previewCubePrefab; 
    public Transform player1Base; 
    public Transform player2Base; 
    
    private List<GameObject> _spawnedPreviewCubes = new List<GameObject>(); 
    private List<List<Vector3>> _allShapesPool = new List<List<Vector3>>();
    
    public List<Vector3> targetShape = new List<Vector3>();

    void Start()
    {
        InitializeShapePool();
        ReturnToMainMenu(); // Oyun başlarken direkt menü kurulumunu yapsın
    }

    // --- MENÜ VE ÇIKIŞ FONKSİYONLARI ---

    // Oyunu tamamen kapatır (Sadece Build alındığında (EXE/APK) çalışır, editörde görünmez)
    public void QuitGame()
    {
        Debug.Log("Oyundan Çıkılıyor...");
        Application.Quit();
    }

    // Oyun içinden Ana Menüye dönerken her şeyi sıfırlar
    public void ReturnToMainMenu()
    {
        currentState = GameState.MainMenu;
        p1Wins = 0;
        p2Wins = 0;
        
        HidePreviewShape();
        ClearPlayerGrid(player1Grid);
        ClearPlayerGrid(player2Grid);

        if (player1Controller != null) player1Controller.SetActive(true);
    
        // Menüdeyken P1 kamerasını tam ekran yapıyoruz
        if (camP1 != null) camP1.rect = new Rect(0, 0, 1, 1); 

        // Sadece P2'yi kapatıyoruz
        if (player2Controller != null) player2Controller.SetActive(false);

        if (menuPanel != null) menuPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
    }

    // Random Modu UI'dan (Toggle/Checkbox) açıp kapatmak için
    public void SetRandomMode(bool isOn)
    {
        isRandomModeOn = isOn;
        Debug.Log("Random Mod: " + (isOn ? "AÇIK" : "KAPALI"));
    }

    // --- MOD SEÇİMLERİ ---

    public void SelectSoloMode()
    {
        isSoloMode = true;
        camP1.rect = new Rect(0, 0, 1, 1); 
        
        if (camP2 != null) camP2.gameObject.SetActive(false); 
        if (player2Base != null) player2Base.gameObject.SetActive(false); 
        if (player2Controller != null) player2Controller.SetActive(false); 
        if (player1Controller != null) player1Controller.SetActive(true);

        if (p1ButtonContainer != null) p1ButtonContainer.SetActive(true);
        if (p2ButtonContainer != null) p2ButtonContainer.SetActive(false);

        if (menuPanel != null) menuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        StartNewRound();
    }

    public void SelectDuoMode()
    {
        isSoloMode = false;
        camP1.rect = new Rect(0, 0, 0.5f, 1); 
        
        if (camP2 != null) 
        {
            camP2.rect = new Rect(0.5f, 0, 0.5f, 1); 
            camP2.gameObject.SetActive(true);
        }
        
        if (player2Base != null) player2Base.gameObject.SetActive(true);
        if (player1Controller != null) player1Controller.SetActive(true);
        if (player2Controller != null) player2Controller.SetActive(true);

        if (p1ButtonContainer != null) p1ButtonContainer.SetActive(true);
        if (p2ButtonContainer != null) p2ButtonContainer.SetActive(true);

        if (menuPanel != null) menuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        StartNewRound();
    }

    void InitializeShapePool()
    {
        List<Vector3> koltuk = new List<Vector3> {
            new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0),
            new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(2,0,1),
            new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(2,1,0)
        };
        _allShapesPool.Add(koltuk);

        List<Vector3> piramit = new List<Vector3> {
            new Vector3(0,0,0), new Vector3(1,0,0),
            new Vector3(0,0,1), new Vector3(1,0,1),
            new Vector3(0,1,0)
        };
        _allShapesPool.Add(piramit);

        List<Vector3> kare = new List<Vector3> {
            new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(2,0,0),
            new Vector3(0,0,1),                     new Vector3(2,0,1),
            new Vector3(0,0,2), new Vector3(1,0,2), new Vector3(2,0,2)
        };
        _allShapesPool.Add(kare);
    }

    public void StartNewRound()
    {
        ClearPlayerGrid(player1Grid);
        if (!isSoloMode) ClearPlayerGrid(player2Grid); 

        // RASTGELE MOD AÇIKSA PROCEDURAL ÜRET, KAPALIYSA HAVUZDAN SEÇ
        if (isRandomModeOn)
        {
            targetShape = GenerateProceduralShape();
        }
        else
        {
            int randomIndex = Random.Range(0, _allShapesPool.Count);
            targetShape = _allShapesPool[randomIndex];
        }

        SpawnPreviewShape(player1Base);
        if (!isSoloMode) SpawnPreviewShape(player2Base); 

        currentState = GameState.Observe;
        _timer = observeTime;
    }

    // YENİ: KENDİ KENDİNE YEPYENİ ŞEKİLLER ÜRETEN ALGORİTMA
    List<Vector3> GenerateProceduralShape()
    {
        List<Vector3> randomShape = new List<Vector3>();
        int cubeCount = Random.Range(4, 9); // 4 ile 8 arası küpten oluşan şekil
        
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 randomPos;
            int antiCrash = 0;
            do
            {
                // 3x3x3 boyutlarında rastgele bir koordinat seç
                randomPos = new Vector3(Random.Range(0, 3), Random.Range(0, 3), Random.Range(0, 3));
                antiCrash++;
                if(antiCrash > 50) break; // Sonsuz döngü koruması
                
            } while (randomShape.Contains(randomPos)); 
            
            randomShape.Add(randomPos);
        }
        return randomShape;
    }

    void Update()
    {
        if (currentState == GameState.Observe)
        {
            _timer -= Time.deltaTime;
            UpdateUI($"ŞEKLİ EZBERLE!\nKalan Süre: {Mathf.CeilToInt(_timer)}");

            if (_timer <= 0)
            {
                HidePreviewShape();
                currentState = GameState.Build;
                _timer = buildTime;
            }
        }
        else if (currentState == GameState.Build)
        {
            _timer -= Time.deltaTime;
            UpdateUI($"İNŞA ET!\nKalan Süre: {Mathf.CeilToInt(_timer)}");
            CheckEarlyWin();

            if (_timer <= 0) EndRound();
        }
    }

    void UpdateUI(string message)
    {
        if (infoText != null) infoText.text = message;
    }

    void SpawnPreviewShape(Transform baseTransform)
    {
        if (baseTransform == null || previewCubePrefab == null) return;

        int baseX = Mathf.RoundToInt(baseTransform.position.x);
        int baseY = Mathf.RoundToInt(baseTransform.position.y);
        int baseZ = Mathf.RoundToInt(baseTransform.position.z);

        foreach (Vector3 localPos in targetShape)
        {
            float spawnX = baseX + localPos.x;
            float spawnY = baseY + localPos.y + 1f; 
            float spawnZ = baseZ + localPos.z;

            Vector3 finalSpawnPos = new Vector3(spawnX, spawnY, spawnZ);
            GameObject cube = Instantiate(previewCubePrefab, finalSpawnPos, Quaternion.identity);
            cube.tag = "Cube"; 
            _spawnedPreviewCubes.Add(cube);
        }
    }

    void HidePreviewShape()
    {
        foreach (GameObject cube in _spawnedPreviewCubes)
        {
            if (cube != null) Destroy(cube);
        }
        _spawnedPreviewCubes.Clear();
    }

    void ClearPlayerGrid(GridManager grid)
    {
        if (grid == null) return;

        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes)
        {
            Destroy(cube);
        }
        grid.placedCubes.Clear();
    }

    void CheckEarlyWin()
    {
        bool p1Finished = IsShapePerfect(player1Grid.placedCubes);
        
        if (isSoloMode)
        {
            if (p1Finished) 
            { 
                p1Wins += CalculateScore(player1Grid.placedCubes); // Solo modda kazanma sayısı değil, total puan tutulur
                EndRound(true); 
            }
            return; 
        }

        bool p2Finished = IsShapePerfect(player2Grid.placedCubes);

        if (p1Finished)
        {
            p1Wins++;
            EndRound(true);
        }
        else if (p2Finished)
        {
            p2Wins++;
            EndRound(true);
        }
    }

    bool IsShapePerfect(List<Vector3> playerCubes)
    {
        if (playerCubes.Count != targetShape.Count) return false;

        foreach (Vector3 pos in playerCubes)
        {
            if (!targetShape.Contains(pos)) return false;
        }
        return true;
    }

    void EndRound(bool isEarlyWin = false)
    {
        currentState = GameState.RoundEnd;

        if (!isEarlyWin)
        {
            int p1Score = CalculateScore(player1Grid.placedCubes);
            
            if (isSoloMode)
            {
                p1Wins += p1Score; // Toplam puana ekle
                UpdateUI($"TUR BİTTİ!\nBu Tur Puanın: {p1Score}\nToplam Puanın: {p1Wins}");
            }
            else
            {
                int p2Score = CalculateScore(player2Grid.placedCubes);
                if (p1Score > p2Score) p1Wins++;
                else if (p2Score > p1Score) p2Wins++;
                UpdateUI($"TUR BİTTİ!\nP1 Puan: {p1Score} | P2 Puan: {p2Score}");
            }
        }
        else 
        {
            if (isSoloMode) UpdateUI($"MÜKEMMEL EŞLEŞME!\nToplam Puanın: {p1Wins}");
            else UpdateUI("MÜKEMMEL EŞLEŞME!\nTur Kazanıldı!");
        }

        // SOLO MOD İSE SONSUZ DÖNGÜ (winTarget kontrolü yapılmaz)
        if (isSoloMode)
        {
            Invoke(nameof(StartNewRound), 3f);
        }
        // DUO MOD İSE 3 YAPAN KAZANIR (OYUN BİTER)
        else
        {
            if (p1Wins >= winTarget) 
            {
                UpdateUI("OYUN BİTTİ!\nOYUNCU 1 KAZANDI!");
            }
            else if (p2Wins >= winTarget) 
            {
                UpdateUI("OYUN BİTTİ!\nOYUNCU 2 KAZANDI!");
            }
            else
            {
                Invoke(nameof(StartNewRound), 3f);
            }
        }
    }

    int CalculateScore(List<Vector3> playerCubes)
    {
        int score = 0;
        foreach (Vector3 pos in playerCubes)
        {
            if (targetShape.Contains(pos)) score += 10;
            else score -= 5; // Yanlış yere koyulan her blok -5 puan
        }
        return score;
    }
}