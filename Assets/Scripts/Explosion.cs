using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnExplosionComplete(Explosion explosion);

public class Explosion : MonoBehaviour
{
    public OnExplosionComplete onExplosionComplete;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ResetAnimation()
    {
        animator.Play("explode");
    }

    public void OnExplosionComplete()
    {
        if (onExplosionComplete != null)
            onExplosionComplete(this);
    }
}
