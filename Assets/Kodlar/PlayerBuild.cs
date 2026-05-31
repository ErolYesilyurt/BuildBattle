using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuild : MonoBehaviour
{
    public enum BuildMode { Add, Delete }
    
    [Header("Aktif İnşa Modu")]
    public BuildMode currentMode = BuildMode.Add; 

    public GridManager gridManager;
    public OrbitCamera orbitCamera;
    public RoundManager roundManager; 
    public int playerID = 1; 

    void Update()
    {
        if (roundManager != null && roundManager.currentState == RoundManager.GameState.Build)
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                if (IsClickInMyScreenHalf(mousePos))
                {
                    if (orbitCamera != null && !orbitCamera.IsDragging())
                    {
                        Camera cam = orbitCamera != null ? orbitCamera.GetComponent<Camera>() : Camera.main;
                        Ray ray = cam.ScreenPointToRay(mousePos);

                        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                        {
                            if (currentMode == BuildMode.Add)
                            {
                                Vector3 newCubePos = hit.point + (hit.normal * 0.1f);
                                int x = Mathf.RoundToInt(newCubePos.x);
                                int y = Mathf.RoundToInt(newCubePos.y);
                                int z = Mathf.RoundToInt(newCubePos.z);

                                // OYUNCUNUN SEÇTİĞİ BLOK İNDEKSİNİ AL (0, 1, 2 veya 3)
                                int seciliIndeks = (playerID == 1) ? roundManager.p1SelectedBlockIndex : roundManager.p2SelectedBlockIndex;

                                // Seçili indeksi GridManager'a gönder
                                gridManager.AddCube(new Vector3(x, y, z), seciliIndeks);
                            }
                            else if (currentMode == BuildMode.Delete)
                            {
                                if (hit.collider.gameObject.CompareTag("Cube"))
                                {
                                    int x = Mathf.RoundToInt(hit.collider.gameObject.transform.position.x);
                                    int y = Mathf.RoundToInt(hit.collider.gameObject.transform.position.y);
                                    int z = Mathf.RoundToInt(hit.collider.gameObject.transform.position.z);

                                    gridManager.RemoveCube(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void ToggleMode()
    {
        if (currentMode == BuildMode.Add) currentMode = BuildMode.Delete;
        else if (currentMode == BuildMode.Delete) currentMode = BuildMode.Add;
    }
    
    public void SetAddMode()
    {
        currentMode = BuildMode.Add;
        Debug.Log($"Player {playerID}: Ekleme Moduna Geçti.");
    }

    public void SetDeleteMode()
    {
        currentMode = BuildMode.Delete;
        Debug.Log($"Player {playerID}: Silme Moduna Geçti.");
    }

    bool IsClickInMyScreenHalf(Vector2 clickPos)
    {
        if (roundManager != null && roundManager.isSoloMode && playerID == 1) 
            return true;

        float halfScreenWidth = Screen.width / 2f;

        if (playerID == 1) return clickPos.x < halfScreenWidth; 
        else return clickPos.x >= halfScreenWidth; 
    }
}