using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public float sinkSpeed = 2.5f;
    public int scoreValue = 10;
    public AudioClip deathClip;


    Animator anim;
    AudioSource enemyAudio;
    ParticleSystem hitParticles;
    CapsuleCollider capsuleCollider;
    bool isDead;
    bool isSinking;
    Vector3 sinkPosition;


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        enemyAudio = GetComponent <AudioSource> ();
        hitParticles = GetComponentInChildren <ParticleSystem> ();
        capsuleCollider = GetComponent <CapsuleCollider> ();
        FitBodyCollider();

        currentHealth = startingHealth;
    }


    // Use the mesh bounds in enemy-root space, without changing model scale.
    void FitBodyCollider()
    {
        if (capsuleCollider == null) return;
        var body = GetComponentInChildren<SkinnedMeshRenderer>();
        if (body == null) return;

        Bounds meshBounds = body.localBounds;
        Bounds rootBounds = new Bounds();
        for (int i = 0; i < 8; i++)
        {
            Vector3 corner = meshBounds.center + Vector3.Scale(meshBounds.extents,
                new Vector3((i & 1) == 0 ? -1 : 1,
                            (i & 2) == 0 ? -1 : 1,
                            (i & 4) == 0 ? -1 : 1));
            Vector3 point = transform.InverseTransformPoint(body.transform.TransformPoint(corner));
            if (i == 0) rootBounds = new Bounds(point, Vector3.zero);
            else rootBounds.Encapsulate(point);
        }

        capsuleCollider.direction = 1;
        capsuleCollider.center = rootBounds.center;
        capsuleCollider.radius = Mathf.Min(rootBounds.extents.x, rootBounds.extents.z,
            rootBounds.extents.y) * 0.95f;
        capsuleCollider.height = rootBounds.size.y;
    }

    void LateUpdate ()
    {
        if(isSinking)
        {
            sinkPosition += Vector3.down * sinkSpeed * Time.deltaTime;
            transform.position = sinkPosition;
        }
    }


    public void TakeDamage (int amount, Vector3 hitPoint)
    {
        if(isDead)
            return;

        // Phát âm thanh bị thương (nếu có)
        if (enemyAudio != null)
            enemyAudio.Play ();

        currentHealth -= amount;
        Debug.Log(name + " nhận " + amount + " sát thương, máu còn: " + currentHealth, this);

        // Hiệu ứng trúng đạn (nếu có)
        if (hitParticles != null)
        {
            hitParticles.transform.position = hitPoint;
            hitParticles.Play();
        }

        if(currentHealth <= 0)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        var movement = GetComponent<EnemyMovement>();
        if (movement != null) movement.enabled = false;
        var attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.enabled = false;
        foreach (Collider hitCollider in GetComponentsInChildren<Collider>())
            hitCollider.enabled = false;

        // Chạy animation chết (nếu có)
        if (anim != null)
            anim.SetTrigger ("Dead");

        // Phát âm thanh chết (nếu có clip)
        if (enemyAudio != null && deathClip != null)
        {
            enemyAudio.clip = deathClip;
            enemyAudio.Play ();
        }

        // Phản hồi ngay khi hết máu, không tiếp tục đuổi người chơi.
        StartSinking();
    }


    public void StartSinking ()
    {
        if (isSinking) return;
        var nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (nav != null) nav.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Freeze the pose so animation cannot move the root back while sinking.
        if (anim != null)
        {
            anim.applyRootMotion = false;
            anim.enabled = false;
        }
        sinkPosition = transform.position;
        isSinking = true;
        Destroy(gameObject, 2f);
    }
}
