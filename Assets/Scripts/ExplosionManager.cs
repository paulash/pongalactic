using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public GameObject[] explosionPrefabs;
    Stack<Explosion> pooledExplosions = new Stack<Explosion>();

    public void SpawnExplosion(Vector2 location, Transform target=null)
    {
        Explosion wakedExplosion = null;
        if (pooledExplosions.Count != 0)
        {
            wakedExplosion = pooledExplosions.Pop();
            wakedExplosion.gameObject.SetActive(true);
        }
        else
        {
            GameObject explosionGO = GameObject.Instantiate(explosionPrefabs[Random.Range(0, explosionPrefabs.Length)]);
            wakedExplosion = explosionGO.GetComponent<Explosion>();
            wakedExplosion.onExplosionComplete += OnExplosionComplete;
        }

        wakedExplosion.transform.parent = target == null ? transform : target;
        wakedExplosion.transform.position = location;
        wakedExplosion.transform.eulerAngles = new Vector3(0, 0, Random.Range(-180, 180));
    }

    void OnExplosionComplete(Explosion explosion)
    {
        explosion.transform.parent = transform;
        explosion.gameObject.SetActive(false);
        pooledExplosions.Push(explosion);
    }
}
