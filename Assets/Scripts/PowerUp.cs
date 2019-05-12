using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
    Heal,
    Shield,
    Sticky
}


public class PowerUp : MonoBehaviour
{
    public PowerUpType powerUpType = PowerUpType.Heal;

    Rigidbody2D rigidbody2d;

    public AudioClip takenSound;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        rigidbody2d.velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-0.2f, 0.2f)) * 4;
    }

    private void Update()
    {
        // TODO: if player is near enough, change velocity towards player.
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            AudioSource.PlayClipAtPoint(takenSound, Camera.main.transform.position);

            GameObject.Destroy(gameObject);
            collision.gameObject.SendMessage("OnPowerUp", powerUpType);
        }
    }
}
