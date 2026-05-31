using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

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
    public int winTarget = 3;

    [Header("Örnek Şekil Gösterim Ayarları")]
    public GameObject previewCubePrefab;
    public Transform player1Base;
    public Transform player2Base;

    private List<GameObject> _spawnedPreviewCubes = new List<GameObject>();

    [Header("3D Şekil Kütüphanesi")]
    public SekilVerisi3D[] seviyeSekilleri;

    [HideInInspector] public List<Vector3> targetShape = new List<Vector3>();

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

    public void SetRandomMode(bool isOn)
    {
        isRandomModeOn = isOn;
    }

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
            duoScoreText.text = $"{p1Wins} - {p2Wins}";
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

        if (isRandomModeOn) targetShape = GenerateProceduralShape();
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
        
        UpdateUI("ŞEKLİ EZBERLE!");
    }

    List<Vector3> Yukle3DSekilKoordinatlari(SekilVerisi3D veri)
    {
        List<Vector3> koordinatlar = new List<Vector3>();
        for (int y = 0; y < veri.katmanlar.Count; y++)
            for (int z = 0; z < 5; z++)
                for (int x = 0; x < 5; x++)
                    if (veri.BlokVarMi(x, y, z))
                        koordinatlar.Add(new Vector3(x, y, z));
        return koordinatlar;
    }

    List<Vector3> GenerateProceduralShape()
    {
        List<Vector3> randomShape = new List<Vector3>();
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
            } while (randomShape.Contains(randomPos));
            randomShape.Add(randomPos);
        }
        return randomShape;
    }

    void Update()
    {
        if (currentState == GameState.Observe || currentState == GameState.Build)
        {
            _timer -= Time.deltaTime;
            
            // Süreyi sadece rakam olarak Text objelerine yansıtıyoruz
            string timeString = Mathf.CeilToInt(_timer).ToString();
            if (p1TimerText != null) p1TimerText.text = timeString;
            if (p2TimerText != null && !isSoloMode) p2TimerText.text = timeString;

            if (currentState == GameState.Observe && _timer <= 0)
            {
                HidePreviewShape();
                currentState = GameState.Build;
                _timer = buildTime;
                UpdateUI("İNŞA ET!");
            }
            else if (currentState == GameState.Build)
            {
                CheckEarlyWin();
                if (_timer <= 0) EndRound();
            }
        }
    }

    void UpdateUI(string message)
    {
        if (infoText != null) infoText.text = message;
    }

    void SpawnPreviewShape(Transform baseTransform)
    {
        if (baseTransform == null || previewCubePrefab == null) return;
        float baseX = baseTransform.position.x;
        float baseY = baseTransform.position.y;
        float baseZ = baseTransform.position.z;

        foreach (Vector3 localPos in targetShape)
        {
            float spawnX = baseX + (localPos.x - 2f);
            float spawnY = baseY + localPos.y + 1f;
            float spawnZ = baseZ + (localPos.z - 2f);
            Vector3 finalSpawnPos = new Vector3(spawnX, spawnY, spawnZ);
            GameObject cube = Instantiate(previewCubePrefab, finalSpawnPos, Quaternion.identity);
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
        grid.placedCubes.Clear();
    }

    void CheckEarlyWin()
    {
        bool p1Finished = IsShapePerfect(player1Grid.placedCubes, player1Base);
        
        if (isSoloMode)
        {
            if (p1Finished) EndRound(true);
            return;
        }

        bool p2Finished = IsShapePerfect(player2Grid.placedCubes, player2Base);

        if (p1Finished || p2Finished) EndRound(true);
    }

    bool IsShapePerfect(List<Vector3> playerCubes, Transform playerBase)
    {
        if (playerCubes.Count != targetShape.Count) return false;

        float baseX = playerBase.position.x;
        float baseY = playerBase.position.y;
        float baseZ = playerBase.position.z;

        foreach (Vector3 pos in playerCubes)
        {
            int gridX = Mathf.RoundToInt(pos.x - baseX + 2f);
            int gridY = Mathf.RoundToInt(pos.y - baseY - 1f);
            int gridZ = Mathf.RoundToInt(pos.z - baseZ + 2f);
            Vector3 normalizedPos = new Vector3(gridX, gridY, gridZ);

            if (!targetShape.Contains(normalizedPos)) return false;
        }
        return true;
    }

    void EndRound(bool isEarlyWin = false)
    {
        currentState = GameState.RoundEnd;

        float p1Percentage = CalculateSuccessPercentage(player1Grid.placedCubes, player1Base);
        
        if (isSoloMode)
        {
            if (isEarlyWin) p1Percentage = 100f;
            
            UpdateUI($"TUR BİTTİ!\nDoğruluk Oranı: %{p1Percentage:F1}");
            if (p1ScoreText != null) p1ScoreText.text = $"%{p1Percentage:F1}";

            Invoke(nameof(StartNewRound), 3f);
        }
        else
        {
            float p2Percentage = CalculateSuccessPercentage(player2Grid.placedCubes, player2Base);
            
            if (isEarlyWin)
            {
                if (IsShapePerfect(player1Grid.placedCubes, player1Base)) p1Percentage = 100f;
                if (IsShapePerfect(player2Grid.placedCubes, player2Base)) p2Percentage = 100f;
            }

            if (p1Percentage > p2Percentage) p1Wins++;
            else if (p2Percentage > p1Percentage) p2Wins++;

            if (p1ScoreText != null) p1ScoreText.text = $"%{p1Percentage:F1}";
            if (p2ScoreText != null) p2ScoreText.text = $"%{p2Percentage:F1}";

            if (duoScoreText != null) duoScoreText.text = $"{p1Wins} - {p2Wins}";

            if (p1Wins >= winTarget) 
            {
                UpdateUI("OYUN BİTTİ!\nOYUNCU 1 KAZANDI!");
                if (duoScoreText != null) duoScoreText.color = Color.green;
            }
            else if (p2Wins >= winTarget) 
            {
                UpdateUI("OYUN BİTTİ!\nOYUNCU 2 KAZANDI!");
                if (duoScoreText != null) duoScoreText.color = Color.green;
            }
            else
            {
                UpdateUI("TUR BİTTİ!");
                Invoke(nameof(StartNewRound), 3f);
            }
        }
    }

    float CalculateSuccessPercentage(List<Vector3> playerCubes, Transform playerBase)
    {
        if (targetShape.Count == 0) return 0f;

        float totalTargetBlocks = targetShape.Count;
        int correctCount = 0;
        int wrongCount = 0;
        
        float baseX = playerBase.position.x;
        float baseY = playerBase.position.y;
        float baseZ = playerBase.position.z;

        foreach (Vector3 pos in playerCubes)
        {
            int gridX = Mathf.RoundToInt(pos.x - baseX + 2f);
            int gridY = Mathf.RoundToInt(pos.y - baseY - 1f);
            int gridZ = Mathf.RoundToInt(pos.z - baseZ + 2f);
            
            Vector3 normalizedPos = new Vector3(gridX, gridY, gridZ);

            if (targetShape.Contains(normalizedPos)) correctCount++;
            else wrongCount++;
        }

        float correctPercentage = (correctCount / totalTargetBlocks) * 100f;
        float wrongPercentage = (wrongCount / totalTargetBlocks) * 100f;
        float finalPercentage = correctPercentage - wrongPercentage;

        return Mathf.Clamp(finalPercentage, 0f, 100f);
    }
}