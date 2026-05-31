using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public enum GameState { MainMenu, Observe, Build, RoundEnd }
    public GameState currentState;

    [Header("Oyun Modu Ayarları")]
    public bool isSoloMode;
    public bool isRandomModeOn;

    [Header("Ana UI ve Kamera Ayarları")]
    public GameObject menuPanel;
    public GameObject gamePanel;
    public TextMeshProUGUI infoText; 
    public Camera camP1;
    public Camera camP2;

    [Header("Yeni Şık UI (Skor ve Süre)")]
    public TextMeshProUGUI duoScoreText; 
    public TextMeshProUGUI p1TimerText;  
    public TextMeshProUGUI p2TimerText;  
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;

    [Header("Blok Envanter Ayarları")]
    public int p1SelectedBlockIndex; 
    public int p2SelectedBlockIndex; 

    [Header("Buton Grupları (Add/Delete)")]
    public GameObject p1ButtonContainer;
    public GameObject p2ButtonContainer;

    [Header("Süre Ayarları")]
    public float observeTime = 15f;
    public float buildTime = 60f;
    private float _timer;

    [Header("Speed Builders Zaman Takibi")]
    private bool _p1FinishedRound;
    private bool _p2FinishedRound;
    private float _p1FinishTimeLeft;
    private float _p2FinishTimeLeft;

    [Header("Kontrolcüler")]
    public GameObject player1Controller;
    public GameObject player2Controller;

    [Header("Oyuncu ve Skor Ayarları")]
    public GridManager player1Grid;
    public GridManager player2Grid;
    public int p1Wins;
    public int p2Wins;
    public int winTarget = 3;

    [Header("Blok Ayarları")]
    public GameObject[] blockPrefabs; 
    
    [Header("Örnek Şekil Gösterim Ayarları")]
    public Transform player1Base;
    public Transform player2Base;
    private readonly List<GameObject> _spawnedPreviewCubes = new List<GameObject>();

    [Header("3D Şekil Kütüphanesi")]
    public SekilVerisi3D[] seviyeSekilleri;

    public Dictionary<Vector3, int> targetShape = new Dictionary<Vector3, int>();

    void Start()
    {
        seviyeSekilleri = Resources.LoadAll<SekilVerisi3D>("HazirSekiller");
        ReturnToMainMenu();
    }

    public void QuitGame() 
    { 
        Application.Quit(); 
    }

    public void ReturnToMainMenu()
    {
        currentState = GameState.MainMenu;
        p1Wins = 0; 
        p2Wins = 0;
        p1SelectedBlockIndex = 0; 
        p2SelectedBlockIndex = 0;

        HidePreviewShape();
        ClearPlayerGrid(player1Grid);
        ClearPlayerGrid(player2Grid);

        if (p1ScoreText != null) p1ScoreText.text = "";
        if (p2ScoreText != null) p2ScoreText.text = "";
        if (duoScoreText != null) duoScoreText.gameObject.SetActive(false);
        if (p1TimerText != null) p1TimerText.gameObject.SetActive(false);
        if (p2TimerText != null) p2TimerText.gameObject.SetActive(false);

        if (player1Controller != null) player1Controller.SetActive(true);
        if (camP1 != null) camP1.rect = new Rect(0, 0, 1, 1);
        if (player2Controller != null) player2Controller.SetActive(false);

        if (menuPanel != null) menuPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
    }

    public void SetP1SelectedBlock(int index) { p1SelectedBlockIndex = index; }
    public void SetP2SelectedBlock(int index) { p2SelectedBlockIndex = index; }
    public void SetRandomMode(bool isOn) { isRandomModeOn = isOn; }

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

        if (duoScoreText != null) duoScoreText.gameObject.SetActive(false);
        if (p2TimerText != null) p2TimerText.gameObject.SetActive(false);
        if (p1TimerText != null) p1TimerText.gameObject.SetActive(true);

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

        if (duoScoreText != null) 
        {
            duoScoreText.gameObject.SetActive(true);
            duoScoreText.text = $"<color=#EAB308>{p1Wins} - {p2Wins}</color>";
        }
        if (p1TimerText != null) p1TimerText.gameObject.SetActive(true);
        if (p2TimerText != null) p2TimerText.gameObject.SetActive(true);

        if (menuPanel != null) menuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        StartNewRound();
    }

    public void StartNewRound()
    {
        ClearPlayerGrid(player1Grid);
        if (!isSoloMode) ClearPlayerGrid(player2Grid);

        if (p1ScoreText != null) p1ScoreText.text = "";
        if (p2ScoreText != null) p2ScoreText.text = "";

        _p1FinishedRound = false;
        _p2FinishedRound = false;
        _p1FinishTimeLeft = 0f;
        _p2FinishTimeLeft = 0f;

        if (isRandomModeOn) 
        {
            targetShape = GenerateProceduralShape();
        }
        else
        {
            if (seviyeSekilleri != null && seviyeSekilleri.Length > 0)
                targetShape = Yukle3DSekilKoordinatlari(seviyeSekilleri[Random.Range(0, seviyeSekilleri.Length)]);
            else
                targetShape = GenerateProceduralShape();
        }

        SpawnPreviewShape(player1Base);
        if (!isSoloMode) SpawnPreviewShape(player2Base);

        currentState = GameState.Observe;
        _timer = observeTime;
        
        UpdateUI("<color=#38BDF8>ŞEKLİ İNCELE!</color>");
    }

    Dictionary<Vector3, int> Yukle3DSekilKoordinatlari(SekilVerisi3D veri)
    {
        Dictionary<Vector3, int> koordinatlar = new Dictionary<Vector3, int>();
        for (int y = 0; y < veri.katmanlar.Count; y++)
        {
            for (int z = 0; z < 5; z++)
            {
                for (int x = 0; x < 5; x++)
                {
                    int blokTipi = veri.BlokTipiniGetir(x, y, z);
                    if (blokTipi != -1) 
                    {
                        koordinatlar.Add(new Vector3(x, y, z), blokTipi);
                    }
                }
            }
        }
        return koordinatlar;
    }

    Dictionary<Vector3, int> GenerateProceduralShape()
    {
        Dictionary<Vector3, int> randomShape = new Dictionary<Vector3, int>();
        int cubeCount = Random.Range(4, 9);
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 randomPos;
            int antiCrash = 0;
            do
            {
                randomPos = new Vector3(Random.Range(0, 5), Random.Range(0, 4), Random.Range(0, 5));
                antiCrash++;
                if (antiCrash > 50) break;
            } while (randomShape.ContainsKey(randomPos));
            
            randomShape.Add(randomPos, Random.Range(0, 4));
        }
        return randomShape;
    }

    void Update()
    {
        if (currentState == GameState.Observe || currentState == GameState.Build)
        {
            _timer -= Time.deltaTime;
            
            if (currentState == GameState.Observe)
            {
                string timeString = Mathf.CeilToInt(_timer).ToString();
                if (p1TimerText != null) p1TimerText.text = timeString;
                if (p2TimerText != null && !isSoloMode) p2TimerText.text = timeString;

                if (_timer <= 0)
                {
                    HidePreviewShape();
                    currentState = GameState.Build;
                    _timer = buildTime;
                    UpdateUI("<color=#22C55E>İNŞA ET!</color>");
                }
            }
            else if (currentState == GameState.Build)
            {
                // Speed Builders: Birisi mükemmel yaptığında anında yakala
                if (!_p1FinishedRound && IsShapePerfect(player1Grid.placedBlocks, player1Base))
                {
                    _p1FinishedRound = true;
                    _p1FinishTimeLeft = _timer; 
                }

                if (!isSoloMode && !_p2FinishedRound && IsShapePerfect(player2Grid.placedBlocks, player2Base))
                {
                    _p2FinishedRound = true;
                    _p2FinishTimeLeft = _timer; 
                }

                string currentGlobalTime = Mathf.CeilToInt(_timer).ToString();
                if (p1TimerText != null) p1TimerText.text = currentGlobalTime;
                if (!isSoloMode && p2TimerText != null) p2TimerText.text = currentGlobalTime;

                // TAMAM yazısını bekletmeden, biri bitirdiği an veya süre bittiğinde turu bitir
                if (_p1FinishedRound || _p2FinishedRound || _timer <= 0)
                {
                    EndRound();
                }
            }
        }
    }

    void UpdateUI(string message) 
    { 
        if (infoText != null) infoText.text = message; 
    }

    void SpawnPreviewShape(Transform baseTransform)
    {
        if (baseTransform == null || blockPrefabs.Length == 0) return;
        float baseX = baseTransform.position.x;
        float baseY = baseTransform.position.y;
        float baseZ = baseTransform.position.z;

        foreach (KeyValuePair<Vector3, int> kvp in targetShape)
        {
            Vector3 localPos = kvp.Key;
            int blockType = kvp.Value; 

            float spawnX = baseX + (localPos.x - 2f);
            float spawnY = baseY + localPos.y + 1f;
            float spawnZ = baseZ + (localPos.z - 2f);
            Vector3 finalSpawnPos = new Vector3(spawnX, spawnY, spawnZ);
            
            GameObject cube = Instantiate(blockPrefabs[blockType], finalSpawnPos, Quaternion.identity);
            cube.tag = "Cube";
            _spawnedPreviewCubes.Add(cube);
        }
    }

    void HidePreviewShape()
    {
        foreach (GameObject cube in _spawnedPreviewCubes) if (cube != null) Destroy(cube);
        _spawnedPreviewCubes.Clear();
    }

    void ClearPlayerGrid(GridManager grid)
    {
        if (grid == null) return;
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Cube");
        foreach (GameObject cube in cubes) Destroy(cube);
        grid.placedBlocks.Clear();
    }

    bool IsShapePerfect(Dictionary<Vector3, int> playerCubes, Transform playerBase)
    {
        if (playerCubes.Count != targetShape.Count) return false;

        float baseX = playerBase.position.x;
        float baseY = playerBase.position.y;
        float baseZ = playerBase.position.z;

        foreach (KeyValuePair<Vector3, int> kvp in playerCubes)
        {
            Vector3 pos = kvp.Key;
            int playerBlockType = kvp.Value;

            int gridX = Mathf.RoundToInt(pos.x - baseX + 2f);
            int gridY = Mathf.RoundToInt(pos.y - baseY - 1f);
            int gridZ = Mathf.RoundToInt(pos.z - baseZ + 2f);
            Vector3 normalizedPos = new Vector3(gridX, gridY, gridZ);

            if (!targetShape.TryGetValue(normalizedPos, out int targetBlockType) || targetBlockType != playerBlockType) 
                return false;
        }
        return true;
    }

    // Yüzdeye göre modern renk kodunu veren sistem
    string GetColorHex(float percentage)
    {
        if (percentage >= 100f) return "#22C55E"; // Yeşil (Mükemmel)
        if (percentage >= 70f)  return "#EAB308"; // Sarı (İyi)
        if (percentage >= 40f)  return "#F97316"; // Turuncu (Orta)
        return "#EF4444"; // Kırmızı (Kötü)
    }

    void EndRound()
    {
        currentState = GameState.RoundEnd;

        float p1Percentage = _p1FinishedRound ? 100f : CalculateSuccessPercentage(player1Grid.placedBlocks, player1Base);
        float p2Percentage = isSoloMode ? 0f : (_p2FinishedRound ? 100f : CalculateSuccessPercentage(player2Grid.placedBlocks, player2Base));
        
        float p1Duration = _p1FinishedRound ? (buildTime - _p1FinishTimeLeft) : buildTime;
        float p2Duration = _p2FinishedRound ? (buildTime - _p2FinishTimeLeft) : buildTime;

        string p1Color = GetColorHex(p1Percentage);
        string p2Color = GetColorHex(p2Percentage);

        // Bitiremeyen oyuncunun altına "Süre Bitti" YAZMA. Sadece süresi yeteni göster.
        string p1TimeDisplay = _p1FinishedRound ? $"\n<size=65%><color=#EAB308>⏱ {p1Duration:F1}sn</color></size>" : "";
        string p2TimeDisplay = _p2FinishedRound ? $"\n<size=65%><color=#EAB308>⏱ {p2Duration:F1}sn</color></size>" : "";

        if (isSoloMode)
        {
            UpdateUI($"<color=#FFFFFF>TUR BİTTİ!</color>\n<size=70%>Doğruluk: <color={p1Color}>%{p1Percentage:F1}</color></size>");
            if (p1ScoreText != null) p1ScoreText.text = $"<b><color={p1Color}>%{p1Percentage:F1}</color></b>{p1TimeDisplay}";

            Invoke(nameof(StartNewRound), 3.5f);
        }
        else
        {
            bool p1Kazandi = false;
            bool p2Kazandi = false;

            if (p1Percentage > p2Percentage)
            {
                p1Kazandi = true;
            }
            else if (p2Percentage > p1Percentage)
            {
                p2Kazandi = true;
            }
            else 
            {
                if (_p1FinishTimeLeft > _p2FinishTimeLeft) p1Kazandi = true;
                else if (_p2FinishTimeLeft > _p1FinishTimeLeft) p2Kazandi = true;
            }

            if (p1Kazandi) p1Wins++;
            else if (p2Kazandi) p2Wins++;

            if (p1ScoreText != null) p1ScoreText.text = $"<b><color={p1Color}>%{p1Percentage:F1}</color></b>{p1TimeDisplay}";
            if (p2ScoreText != null) p2ScoreText.text = $"<b><color={p2Color}>%{p2Percentage:F1}</color></b>{p2TimeDisplay}";
            
            if (duoScoreText != null) duoScoreText.text = $"<color=#EAB308>{p1Wins} - {p2Wins}</color>";

            if (p1Wins >= winTarget) 
            {
                UpdateUI("<color=#22C55E>OYUN BİTTİ!\n1. OYUNCU KAZANDI!</color>");
            }
            else if (p2Wins >= winTarget) 
            {
                UpdateUI("<color=#22C55E>OYUN BİTTİ!\n2. OYUNCU KAZANDI!</color>");
            }
            else
            {
                string roundWinnerTxt = p1Kazandi ? "<color=#38BDF8>1. OYUNCU TURU ALDI!</color>" : 
                                       (p2Kazandi ? "<color=#F87171>2. OYUNCU TURU ALDI!</color>" : "<color=#A1A1AA>BERABERE!</color>");
                UpdateUI(roundWinnerTxt);
                
                Invoke(nameof(StartNewRound), 3.5f);
            }
        }
    }

    float CalculateSuccessPercentage(Dictionary<Vector3, int> playerCubes, Transform playerBase)
    {
        if (targetShape.Count == 0) return 0f;

        float totalTargetBlocks = targetShape.Count;
        int correctCount = 0;
        int wrongCount = 0;
        
        float baseX = playerBase.position.x;
        float baseY = playerBase.position.y;
        float baseZ = playerBase.position.z;

        foreach (KeyValuePair<Vector3, int> kvp in playerCubes)
        {
            Vector3 pos = kvp.Key;
            int playerBlockType = kvp.Value; 

            int gridX = Mathf.RoundToInt(pos.x - baseX + 2f);
            int gridY = Mathf.RoundToInt(pos.y - baseY - 1f);
            int gridZ = Mathf.RoundToInt(pos.z - baseZ + 2f);
            Vector3 normalizedPos = new Vector3(gridX, gridY, gridZ);

            if (targetShape.TryGetValue(normalizedPos, out int targetBlockType))
            {
                if (targetBlockType == playerBlockType) correctCount++;
                else wrongCount++;
            }
            else wrongCount++;
        }

        float correctPercentage = (correctCount / totalTargetBlocks) * 100f;
        float wrongPercentage = (wrongCount / totalTargetBlocks) * 100f;
        float finalPercentage = correctPercentage - wrongPercentage;

        return Mathf.Clamp(finalPercentage, 0f, 100f);
    }
}