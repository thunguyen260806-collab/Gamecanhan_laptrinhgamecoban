using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Di chuyển")]
    public float speed = 6f;                        // Tốc độ di chuyển

    [Header("Camera & Chuột")]
    public float mouseSensitivity = 3f;             // Độ nhạy chuột
    public float verticalLookMin = -20f;            // Góc nhìn xuống tối đa
    public float verticalLookMax = 50f;             // Góc nhìn lên tối đa
    public Vector3 cameraOffset = new Vector3(1.5f, 1.6f, -3f); // Vị trí camera so với player

    public float cameraYawOffset = -10f; // Góc nhìn chéo qua vai phải

    float verticalRotation = 0f;                    // Góc pitch của camera
    Animator anim;
    Rigidbody playerRigidbody;
    Camera mainCamera;


    void Awake()
    {
        anim = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

        // Khoá và ẩn con trỏ chuột (như PUBG/Valo)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        // Đọc input chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Mouse X → xoay player theo chiều ngang (Y axis)
        transform.Rotate(0f, mouseX, 0f);

        // Mouse Y → nghiêng camera lên/xuống (pitch), giới hạn góc
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, verticalLookMin, verticalLookMax);
    }


    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Move(h, v);
        Animating(h, v);
    }


    // Camera cập nhật sau khi mọi vật lý chạy xong (tránh giật)
    void LateUpdate()
    {
        // Tính vị trí camera: offset theo hướng player (local → world)
        Vector3 worldOffset = transform.rotation * cameraOffset;
        mainCamera.transform.position = transform.position + worldOffset;

        // Camera nhìn theo Y của player + pitch lên/xuống
        mainCamera.transform.rotation = Quaternion.Euler(
            verticalRotation,
            transform.eulerAngles.y + cameraYawOffset,
            0f
        );
    }


    void Move(float h, float v)
    {
        // Di chuyển theo hướng player đang nhìn (forward/right)
        Vector3 movement = (transform.forward * v + transform.right * h).normalized
                           * speed * Time.deltaTime;

        playerRigidbody.MovePosition(transform.position + movement);
    }


    void Animating(float h, float v)
    {
        bool walking = h != 0f || v != 0f;
        anim.SetBool("IsWalking", walking);
    }
}

