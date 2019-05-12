using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool faceVelocity = true;
    Rigidbody2D rigidbody2d;

    SpriteRenderer spriteRenderer;
    Material material;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        material = spriteRenderer.material;

        GameObject.Destroy(gameObject, 30);
    }

    // Update is called once per frame
    void Update()
    {
        if (faceVelocity)
            transform.up = -rigidbody2d.velocity;

        if (rigidbody2d.velocity.magnitude < 5)
            rigidbody2d.velocity = rigidbody2d.velocity * 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            material.color = new Color(204 / 255f, 105 / 255f, 228 / 255f);
            gameObject.layer = LayerMask.NameToLayer("AllHit");
        }
    }
}
