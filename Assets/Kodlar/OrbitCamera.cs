using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour
{
    public Transform target; 
    public float rotationSpeed = 0.05f; 
    public float distance = 12f; 
    public float dragThreshold = 5f; 
    public RoundManager roundManager;
    // --- YENİ AYARLAR ---
    public int playerID = 1; // Müfettişten (Inspector) 1 veya 2 yapacaksın
    
    private float x = 45.0f;
    private float y = 30.0f;
    private bool isDragging = false;
    private Vector2 startMousePos;

    void Start()
    {
        if (target == null) {
            target = new GameObject("CamTarget").transform;
            target.position = transform.position + transform.forward * distance;
        }
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            
            // Dokunulan yer bu oyuncunun ekran yarısına uyuyorsa sürüklemeyi başlat
            if (IsTouchInMyScreenHalf(mousePos))
            {
                startMousePos = mousePos;
                isDragging = false;
            }
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            
            // Sadece kendi ekran yarısındaysa döndürmeye izin ver
            if (IsTouchInMyScreenHalf(currentMousePos))
            {
                float distanceMoved = Vector2.Distance(startMousePos, currentMousePos);

                if (distanceMoved > dragThreshold)
                {
                    isDragging = true;
                    Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                    
                    x += mouseDelta.x * rotationSpeed;
                    y -= mouseDelta.y * rotationSpeed;
                    y = Mathf.Clamp(y, 5, 85);
                    UpdateCameraPosition();
                }
            }
        }
    }

    // Dokunmanın hangi oyuncunun ekranında olduğunu bulan yardımcı fonksiyon
    bool IsTouchInMyScreenHalf(Vector2 touchPos)
    {
        // Küçük 'r' ile roundManager objesini kontrol ediyoruz
        if (roundManager != null && roundManager.isSoloMode && playerID == 1) 
        {
            return true;
        }

        float halfScreenWidth = Screen.width / 2f;

        if (playerID == 1)
            return touchPos.x < halfScreenWidth;
        else
            return touchPos.x >= halfScreenWidth;
    }

    public bool IsDragging() => isDragging; 

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }
}