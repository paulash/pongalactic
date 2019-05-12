using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void OnSpawnPlayer(GameObject player);

public class GameInstance : MonoBehaviour
{
    public OnSpawnPlayer onSpawnPlayer;

    public GameObject playerPrefab;

    public static GameInstance Instance { get; private set; }
    public GameObject LocalPlayer { get; private set; }

    public AudioClip victoryMusic;
    public AudioClip gameoverMusic;

    public LevelManager levelManager { get; private set; }
    public Healthbar healthbar { get; private set; }
    public ExplosionManager explosionManager { get; private set; }
    public GameOverUI gameoverUI { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    public void Play()
    {
        SceneManager.LoadScene("Level1");
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (levelManager != null)
        {
            levelManager.onWavesComplete -= OnWavesComplete;
            levelManager = null;
        }

        if (scene.name != "MainMenu")
        {
            levelManager = GameObject.FindObjectOfType<LevelManager>();
            healthbar = GameObject.FindObjectOfType<Healthbar>();
            explosionManager = GameObject.FindObjectOfType<ExplosionManager>();
            gameoverUI = GameObject.FindObjectOfType<GameOverUI>();
            gameoverUI.gameObject.SetActive(false);

            if (levelManager != null)
                levelManager.onWavesComplete += OnWavesComplete;

            GameObject startGO = GameObject.FindGameObjectWithTag("PlayerStart");

            LocalPlayer = GameObject.Instantiate(playerPrefab);
            if (startGO != null)
                LocalPlayer.transform.position = startGO.transform.position;

            LocalPlayer.GetComponent<ShipController>().onPlayerDeath = OnPlayerDeath;

            if (onSpawnPlayer != null)
                onSpawnPlayer(LocalPlayer);
        }
        else
        {
            levelManager = null;
            healthbar = null;
            explosionManager = null;
            gameoverUI = null;

            onSpawnPlayer = null;
        }
    }

    void OnWavesComplete()
    {
        AudioSource.PlayClipAtPoint(victoryMusic, Camera.main.transform.position, 0.2f);
        StartCoroutine(TransitionDelay());
    }

    IEnumerator TransitionDelay()
    {
        yield return new WaitForSeconds(8);
        SceneManager.LoadScene(levelManager.nextLevel);
    }

    void OnPlayerDeath(ShipController controller)
    {
        AudioSource.PlayClipAtPoint(gameoverMusic, Camera.main.transform.position, 0.2f);
        gameoverUI.gameObject.SetActive(true);
        levelManager.mainLoop.Stop();
    }
}

