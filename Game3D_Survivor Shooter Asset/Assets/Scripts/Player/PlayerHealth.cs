using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageImage;
    public AudioClip deathClip;
    public float flashSpeed = 5f;
    public Color flashColour = new Color(1f, 0f, 0f, 0.1f);


    Animator anim;
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    //PlayerShooting playerShooting;
    bool isDead;
    bool damaged;


    void Awake ()
    {
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
        playerMovement = GetComponent <PlayerMovement> ();
        //playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = startingHealth;
            healthSlider.value = currentHealth;
        }
    }


    void Update ()
    {
        if (damageImage == null) return;
        if(damaged)
        {
            damageImage.color = flashColour;
        }
        else
        {
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;
    }


    public void TakeDamage (int amount)
    {
        if (isDead || amount <= 0) return;
        damaged = true;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (healthSlider != null) healthSlider.value = currentHealth;

        if (playerAudio != null) playerAudio.Play ();

        if(currentHealth <= 0 && !isDead)
        {
            Death ();
        }
    }


    void Death ()
    {
        isDead = true;

        //playerShooting.DisableEffects ();

        if (anim != null) anim.SetTrigger ("Die");

        if (playerAudio != null && deathClip != null)
        {
            playerAudio.clip = deathClip;
            playerAudio.Play ();
        }

        if (playerMovement != null) playerMovement.enabled = false;
        foreach (PlayerShooting shooting in GetComponentsInChildren<PlayerShooting>())
            shooting.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void OnGUI ()
    {
        if (!isDead) return;

        Rect panel = new Rect((Screen.width - 300) / 2f, (Screen.height - 140) / 2f, 300, 140);
        GUI.Box(panel, "Bạn đã thua");
        if (GUI.Button(new Rect(panel.x + 50, panel.y + 65, 200, 45), "Chơi lại"))
            RestartLevel();
    }

    public void RestartLevel ()
    {
        SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);
    }
}
