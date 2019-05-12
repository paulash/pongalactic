using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnUpdateHealth(int currentHealth, int maxHealth);
public delegate void OnDeath(Ship enemy);

public class Ship : MonoBehaviour
{
    public float deathTime = 1;
    public int maxHealth = 10;
    public int currentHealth { get; private set; }
    public Collider2D weakSpot = null;

    public OnDeath onDeath;

    public OnUpdateHealth onUpdateHealth;

    Collider2D shipCollider;
    SpriteRenderer spriteRenderer;
    Material shipMaterial;

    bool immunity = false;
    public bool dead { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
        shipCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
            shipMaterial = spriteRenderer.material;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (weakSpot == null)
        {
            if (collision.otherCollider != shipCollider)
                return;
        }
        else if (weakSpot != collision.otherCollider)
            return;


        if (collision.gameObject.CompareTag("Ball"))
        {
            GameInstance.Instance.explosionManager.SpawnExplosion(collision.contacts[0].point, transform);
            UpdateHealth(-1);

            GameObject.Destroy(collision.gameObject);

            StartCoroutine(FlashTime());
        }
    }

    public void UpdateHealth(int amount)
    {
        if (immunity) return;
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (onUpdateHealth != null)
            onUpdateHealth(currentHealth, maxHealth);

        if (currentHealth == 0 && !dead)
            StartCoroutine(Death());
    }

    public void UpdateMaxHealth(int amount)
    {
        maxHealth = Mathf.Clamp(maxHealth + amount, 0, 35);
        if (amount > 0)
            currentHealth += amount;

        if (onUpdateHealth != null)
            onUpdateHealth(currentHealth, maxHealth);
    }

    private void Reset()
    {
        currentHealth = maxHealth;
        dead = false;
    }

    IEnumerator Death()
    {
        SendMessage("OnStartDeath", SendMessageOptions.DontRequireReceiver);
        dead = true;

        for (int i = 0; i < deathTime*10; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 randomPosition = (Random.insideUnitCircle * shipCollider.bounds.extents.x) + (Vector2)transform.position;
            GameInstance.Instance.explosionManager.SpawnExplosion(randomPosition);
        }

        if (onDeath != null)
            onDeath(this);

        SendMessage("OnDeath", SendMessageOptions.DontRequireReceiver);
    }

    IEnumerator FlashTime()
    {
        SendMessage("OnHit", SendMessageOptions.DontRequireReceiver);
        immunity = true;

        if (shipMaterial != null)
            shipMaterial.SetFloat("_InvertColors", 1);
        yield return new WaitForSeconds(0.1f);

        if (shipMaterial != null)
            shipMaterial.SetFloat("_InvertColors", 0);
        yield return new WaitForSeconds(0.1f);
        if (shipMaterial != null)
            shipMaterial.SetFloat("_InvertColors", 1);
        yield return new WaitForSeconds(0.1f);

        if (shipMaterial != null)
            shipMaterial.SetFloat("_InvertColors", 0);
        immunity = false;
    }
}
