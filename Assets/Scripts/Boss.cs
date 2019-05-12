using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BossState
{
    Wait,
    Intro,
    PingPong,
    CircleShot
}

public class Boss : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform handL;
    public Transform handR;
    public Transform[] muzzles;

    public float pingPongSpeed = 2;
    public float pingPongSpinRate = 2;
    public float pingPongRateOfFire = 0.2f;

    public AudioClip hitSound;
    public AudioClip deathSound;

    BossState currentState = BossState.Wait;
    float stateTimeout = 0;
    float fireTimeout = 0;

    bool flippedPingPong = false;

    Rigidbody2D rigidbody2d;
    Ship ship;
    


    private void Start()
    {
        ship = GetComponent<Ship>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        SetState(BossState.Intro);
    }

    private void Update()
    {
        if (ship.dead) return;

        switch (currentState)
        {
            case BossState.CircleShot:
                rigidbody2d.velocity = Vector2.zero;
                break;
            case BossState.PingPong:
                float timeRotation = Time.time * pingPongSpinRate;
                float handRotation = Mathf.PingPong(timeRotation, 180) - 90;

                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
                handL.eulerAngles = new Vector3(0, 0, handRotation);
                handR.eulerAngles = new Vector3(0, 0, handRotation);

                if (fireTimeout < Time.time)
                {
                    for (int i = 0; i < muzzles.Length; i++)
                    {
                        Transform muzzle = muzzles[i];
                        GameObject projectile = GameObject.Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
                        projectile.GetComponent<Rigidbody2D>().velocity = flippedPingPong ? -muzzle.up : muzzle.up * 1;
                    }
                    fireTimeout = Time.time + pingPongRateOfFire;
                }

                break;
            case BossState.Intro:
                rigidbody2d.velocity = new Vector2(0, -4);
                break;
        }

        if (stateTimeout < Time.time) // state expired
        {
            switch (currentState)
            {
                case BossState.PingPong:
                    SetState(BossState.CircleShot);
                    break;
                case BossState.CircleShot:
                    SetState(BossState.PingPong);
                    break;
                case BossState.Intro:
                    SetState(BossState.CircleShot);
                    break;
            }
        }
    }

    void SetState(BossState newState)
    {
        switch (currentState) // sleep state.
        {
            case BossState.PingPong:
                break;
        }

        currentState = newState;
        switch (currentState) // wake state
        {
            case BossState.Intro:
                stateTimeout = Time.time + 3;
                break;
            case BossState.PingPong:
                rigidbody2d.velocity = new Vector2((Random.value > 0.5f ? -1 : 1) * pingPongSpeed, 0);
                stateTimeout = Time.time + 20;
                flippedPingPong = !flippedPingPong;
                break;
            case BossState.CircleShot:
                rigidbody2d.velocity = Vector2.zero;

                for (int x = 0; x < 25; x++)
                {
                    float rotation = (x / 25f) * 360f;
                    for (int i = 0; i < muzzles.Length; i++)
                    {
                        Transform muzzle = muzzles[i];
                        GameObject projectile = GameObject.Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
                        projectile.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, rotation) * (Vector2.one * 2);
                    }
                }

                stateTimeout = Time.time + 5;
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (currentState)
        {
            case BossState.PingPong:
                if (collision.gameObject.name == "LeftWall")
                    rigidbody2d.velocity = new Vector2(pingPongSpeed, 0);
                if (collision.gameObject.name == "RightWall")
                    rigidbody2d.velocity = new Vector2(-pingPongSpeed, 0);

                break;
        }
    }

    void OnHit()
    {
        AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position);
    }

    void OnStartDeath()
    {
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position);
    }

    void OnDeath()
    {


        GameObject.Destroy(gameObject);
    }
}
