using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    public GameObject container;
    HealthTick[] ticks;

    void Awake()
    {
        ticks = new HealthTick[container.transform.childCount];
        for (int i = 0; i < ticks.Length; i++)
            ticks[i] = container.transform.GetChild(i).GetComponent<HealthTick>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameInstance.Instance.LocalPlayer != null)
            OnSpawnPlayer(GameInstance.Instance.LocalPlayer);

        GameInstance.Instance.onSpawnPlayer += OnSpawnPlayer;
    }

    void OnSpawnPlayer(GameObject player)
    {
        Ship controller = player.GetComponent<Ship>();
        if (controller != null)
        {
            controller.onUpdateHealth += OnUpdateHealth;
            player.GetComponent<ShipController>().onPlayerDeath += OnPlayerDeath;
        }

        OnUpdateHealth(controller.currentHealth, controller.maxHealth);
    }

    void OnPlayerDeath(ShipController ship)
    {
        ship.onPlayerDeath -= OnPlayerDeath;
        ship.GetComponent<Ship>().onUpdateHealth -= OnUpdateHealth;
    }

    void OnUpdateHealth(int currentHealth, int maxHealth)
    {
        for (int i=0; i < ticks.Length; i++)
        {
            ticks[i].gameObject.SetActive(i < maxHealth);
            ticks[i].SetFilled(i < currentHealth);
        }
    }
}
