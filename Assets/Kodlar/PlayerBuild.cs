using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBuild : MonoBehaviour
{
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
                            // Oyuncunun seçtiği güncel blok/envanter indeksini alıyoruz (0, 1, 2, 3 veya 4)
                            int seciliIndeks = (playerID == 1) ? roundManager.p1SelectedBlockIndex : roundManager.p2SelectedBlockIndex;

                            // SİHİRLİ KISIM: Eğer 5. slot olan Silgi (indeks 4) seçiliyse direkt silme fonksiyonunu çalıştır
                            if (seciliIndeks == 4)
                            {
                                if (hit.collider.gameObject.CompareTag("Cube"))
                                {
                                    int x = Mathf.RoundToInt(hit.collider.gameObject.transform.position.x);
                                    int y = Mathf.RoundToInt(hit.collider.gameObject.transform.position.y);
                                    int z = Mathf.RoundToInt(hit.collider.gameObject.transform.position.z);

                                    gridManager.RemoveCube(new Vector3(x, y, z));
                                }
                            }
                            // Eğer 0, 1, 2, 3 indekslerinden biri seçiliyse normal şekilde ilgili bloğu ekle
                            else
                            {
                                Vector3 newCubePos = hit.point + (hit.normal * 0.1f);
                                int x = Mathf.RoundToInt(newCubePos.x);
                                int y = Mathf.RoundToInt(newCubePos.y);
                                int z = Mathf.RoundToInt(newCubePos.z);

                                gridManager.AddCube(new Vector3(x, y, z), seciliIndeks);
                            }
                        }
                    }
                }
            }
        }
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