using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    [Header("Thông số bắn")]
    public int damagePerShot = 100;
    public float timeBetweenBullets = 0.3f;
    public float range = 100f;

    [Header("Đường đạn")]
    public float bulletLineWidth = 0.09f;
    public Color bulletColor = new Color(1f, 0.95f, 0.45f, 1f);

    float timer;
    Ray shootRay;
    RaycastHit shootHit;
    int shootableMask;
    LineRenderer gunLine;
    AudioSource gunAudio;
    Camera mainCamera;
    float lineDisplayTimer = 0f;
    public float lineDisplayDuration = 0.14f;


    void Awake()
    {
        shootableMask = LayerMask.GetMask("Shootable");
        gunAudio  = GetComponent<AudioSource>();
        mainCamera = Camera.main;

        // --- Tạo LineRenderer ---
        gunLine = GetComponent<LineRenderer>();
        if (gunLine == null)
            gunLine = gameObject.AddComponent<LineRenderer>();

        gunLine.positionCount = 2;
        gunLine.useWorldSpace = true;
        gunLine.alignment = LineAlignment.View;
        gunLine.startWidth    = bulletLineWidth;
        gunLine.endWidth      = bulletLineWidth * 0.8f;
        gunLine.material      = new Material(Shader.Find("Sprites/Default"));
        gunLine.startColor    = new Color(bulletColor.r, bulletColor.g, bulletColor.b, 1f);
        gunLine.endColor      = new Color(bulletColor.r, bulletColor.g, bulletColor.b, 1f);

        // *** Fix lỗi thanh trắng: đặt cả 2 điểm ở vị trí hiện tại ***
        gunLine.SetPosition(0, transform.position);
        gunLine.SetPosition(1, transform.position);
        gunLine.enabled = false;

        // --- Tạo Crosshair (+) ở giữa màn hình ---
        CreateCrosshair();
    }


    void CreateCrosshair()
    {
        // Tạo Canvas nếu chưa có
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("HUDCanvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Tạo Text "+" ở giữa màn hình làm crosshair
        GameObject crosshairGO = new GameObject("Crosshair");
        crosshairGO.transform.SetParent(canvas.transform, false);

        Text txt = crosshairGO.AddComponent<Text>();
        txt.text      = "+";
        txt.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.fontSize  = 40;
        txt.color     = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;

        RectTransform rt = crosshairGO.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta        = new Vector2(50f, 50f);
    }


    void Update()
    {
        timer            += Time.deltaTime;
        lineDisplayTimer += Time.deltaTime;

        // Chuột PHẢI để bắn
        if (Input.GetMouseButton(1) && timer >= timeBetweenBullets && Time.timeScale != 0)
        {
            Shoot();
        }

        // Ẩn đường đạn
        if (lineDisplayTimer >= lineDisplayDuration)
        {
            gunLine.enabled = false;
        }
    }


    void OnDisable()
    {
        if (gunLine != null) gunLine.enabled = false;
    }

    void Shoot()
    {
        timer            = 0f;
        lineDisplayTimer = 0f;

        if (gunAudio != null) gunAudio.Play();

        // Đường đạn bắt đầu từ vị trí object này
        gunLine.enabled = true;
        gunLine.SetPosition(0, transform.position);

        // Tia bắn từ tâm camera
        shootRay.origin    = mainCamera.transform.position;
        shootRay.direction = mainCamera.transform.forward;

        Debug.Log("Bắn! Mask: " + shootableMask);

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Trúng: " + shootHit.collider.gameObject.name
                      + " | Layer: " + LayerMask.LayerToName(shootHit.collider.gameObject.layer));

            EnemyHealth enemyHealth = shootHit.collider.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
                enemyHealth.TakeDamage(damagePerShot, shootHit.point);

            gunLine.SetPosition(1, shootHit.point);
        }
        else
        {
            Debug.Log("Không trúng gì - kiểm tra Layer Shootable của Enemy!");
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
    }
}
