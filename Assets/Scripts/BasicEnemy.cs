using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class BasicEnemy : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform[] muzzles;

    
    public AudioClip hitSound;
    public AudioClip deathSound;

    public AnimationCurve XOverTime;
    public AnimationCurve YOverTime;
    public bool flipXRandom = true;
    public float maxLifeTime = 10;

    public float rateOfFire = 1;

    float lastFireTime = 0;
    float currentLifeTime = 0;

    bool isFlippedX = false;

    Ship ship;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ship = GetComponent<Ship>();
        isFlippedX = flipXRandom && Random.value > 0.5f;
        lastFireTime = rateOfFire + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float screenHeight = (360 / 20) + 5;
        float screenWidth = (640 / 20) * (isFlippedX ? -1 : 1);

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime <= 0)
        {
            transform.position = new Vector3(0, -screenHeight * 2);
            return;
        }


        if (!ship.dead && lastFireTime < Time.time)
        {
            for (int i = 0; i < muzzles.Length; i++)
            {
                Transform muzzle = muzzles[i];
                GameObject projectile = GameObject.Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().velocity = -muzzle.up * 10;

                animator.Play("Fire");
            }

            lastFireTime = rateOfFire + Time.time;
        }

        transform.position = new Vector3(
                XOverTime.Evaluate(currentLifeTime / maxLifeTime) * screenWidth, 
                1-YOverTime.Evaluate(currentLifeTime / maxLifeTime) * screenHeight
                ) - new Vector3(screenWidth / 2f, -screenHeight / 2f);

        if (currentLifeTime > maxLifeTime)
        {
            isFlippedX = flipXRandom && Random.value > 0.5f;
            currentLifeTime = 0;
            lastFireTime = rateOfFire + Time.time;
        }
    }

    void OnDeath()
    {

        GameObject.Destroy(gameObject);
    }

    void OnStartDeath()
    {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
    }

    void OnHit()
    {
        AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
    }
}
