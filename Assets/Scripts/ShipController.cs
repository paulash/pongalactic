using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPlayerDeath(ShipController controller);

public class ShipController : MonoBehaviour
{
    public Transform paddle;
    public float speed = 3;
    public Animator paddleAnimator;

    CircleCollider2D shipCollider;
    Camera mainCamera;
    Rigidbody2D rigidbody2d;
    Ship ship;

    public Collider2D paddleCollider;

    public AudioClip bonkSound;
    public AudioClip hitSound;
    public AudioClip deathSound;

    public OnPlayerDeath onPlayerDeath;


    private void Awake()
    {
        mainCamera = Camera.main;
        rigidbody2d = GetComponent<Rigidbody2D>();
        shipCollider = GetComponent<CircleCollider2D>();
        ship = GetComponent<Ship>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ship.dead) return;

        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 paddleForward = (worldPosition - (Vector2)transform.position).normalized;
        paddle.up = Vector3.Lerp(paddle.up, paddleForward, Time.deltaTime*12);

        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rigidbody2d.AddForce(movement * speed);
    }

    void OnDeath()
    {
        GameObject.Destroy(gameObject);
        if (onPlayerDeath != null)
            onPlayerDeath(this);
    }

    void OnStartDeath()
    {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == paddleCollider)
        {
            paddleAnimator.Play("fire");
            AudioSource.PlayClipAtPoint(bonkSound, Camera.main.transform.position);
        }
    }
    
    void OnHit()
    {
        AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
    }

    void OnPowerUp(PowerUpType powerType)
    {
        switch(powerType)
        {
            case PowerUpType.Heal:
                ship.UpdateHealth(3);
                break;
            case PowerUpType.Shield:
                ship.UpdateMaxHealth(1);
                break;
        }
    }
}
