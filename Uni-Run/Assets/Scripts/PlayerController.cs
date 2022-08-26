using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AudioClip deathClip;
    public float jumpForce = 700f;

    private bool isDead = false;
    private Rigidbody2D playerRigidbody;
    private Animator animator;
    private AudioSource playerAudio;

    public GameObject restartBtn;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();
        restartBtn.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;

        }

        if(Input.GetMouseButtonDown(0))
        {
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
            playerAudio.Play();
        }
        else if(Input.GetMouseButtonUp(0) && playerRigidbody.velocity.y > 0)
        {
            playerRigidbody.velocity = playerRigidbody.velocity * 0.5f;
        }

    }

    void Die()
    {
        playerAudio.clip = deathClip;
        playerAudio.Play();

        playerRigidbody.velocity = Vector2.zero;
        isDead = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Dead" && !isDead)
        {
            Die();
            ShowRestart();
        }
    }

    void ShowRestart()
    {
        restartBtn.SetActive(true);
        Highscore.Instance.ShowLeaderboard();
    }
}
