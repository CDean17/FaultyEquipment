using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    //Game variables
    public float roundEndTimer = 3f;
    public string mapToLoad = "SampleScene";
    


    private PlayerInputManager iManager;
    private GameObject endRoundUI;
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> spawns = new List<GameObject>();
    public List<int> scores = new List<int>();

    //WeaponSpawningVariables
    private bool weaponSpawnsEnable = false;
    public float wepSpawnFrequency = 10f;
    public float wepInitialSpawnDelay = 0.1f;
    private float wepNextSpawn;
    public float wepSpawnsPer = 3f;
    public float wepSpawnPadding = 0.2f;
    public GameObject weaponCrate;

    private AudioSource src;

    // Start is called before the first frame update
    void Start()
    {
        src = gameObject.GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        iManager = transform.GetComponent<PlayerInputManager>();
        DontDestroyOnLoad(gameObject);

        weaponSpawnsEnable = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Called when time to spawn weapons
        if(wepNextSpawn < Time.time && weaponSpawnsEnable)
        {
            
            //For each crate we want to spawn
            for(int i = 0; i < wepSpawnsPer; i++)
            {
                Vector3 spawnPos = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(wepSpawnPadding, 1 - wepSpawnPadding), 1, 0));
                spawnPos = new Vector3(spawnPos.x, spawnPos.y, 0);
                Debug.Log("Spawned!" + spawnPos);
                Instantiate(weaponCrate, spawnPos + new Vector3(0, 5, 0), transform.rotation);
            }
            wepNextSpawn = Time.time + wepSpawnFrequency;
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        //check if max number of players is reached
        if(players.Count <= 3) {
            //if not add the player and set it to not be destroyed on scene transition
            DontDestroyOnLoad(playerInput.gameObject);
            players.Add(playerInput.gameObject);
            scores.Add(0);
        }
        else
        {
            Destroy(playerInput.gameObject);
        }
        
    }

    public void initializeGame()
    {
        if(players.Count >= 2)
        {
            iManager.DisableJoining();
            SceneManager.LoadSceneAsync(mapToLoad, LoadSceneMode.Single);
        }
        
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        wepNextSpawn = Time.time + wepInitialSpawnDelay;
        if(SceneManager.GetActiveScene().name != "LobbyScene")
        {
            weaponSpawnsEnable = true;
            GameObject[] obs = GameObject.FindGameObjectsWithTag("Respawn");
            endRoundUI = GameObject.FindGameObjectWithTag("roundScoreUI");
            endRoundUI.SetActive(false);
            spawns = obs.ToList();

            //Hard coded max number of spawnpoints. Add more falses for more spawns.
            //This is a really terrible way to do this.
            //Too bad!
            List<bool> playerSpawned = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false };

            //loop through each player to spawn them

            foreach (GameObject g in players)
            {
                //Spawning is performed in a loop. If the spawn is not occupied spawn the player there and set that spawn as occupied. If occupied try again.
                while (true)
                {
                    int spawnPoint = (int)Random.Range(-0.9f, spawns.Count - 0.1f);
                    if (playerSpawned[spawnPoint] == false)
                    {
                        playerSpawned[spawnPoint] = true;
                        g.transform.position = spawns[spawnPoint].transform.position;
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (GameObject e in players)
            {
                Destroy(e);
            }
            Destroy(gameObject);
        }

        if(SceneManager.GetActiveScene().name == "MainMenu")
        {
            foreach (GameObject e in players)
            {
                Destroy(e);
            }
            Destroy(gameObject);
        }
        
    }

    public void dropPlayer(GameObject g)
    {
        
        //If p1 calls drop player then destroy this object
        if(g.GetComponent<PlayerInventory>().playerNumber == 1)
        {
            SceneManager.LoadScene("MainMenu");
            
        }

        players.Remove(g);
        scores.Clear();
        foreach(GameObject e in players)
        {
            scores.Add(0);
        }
    }

    public void checkAlive()
    {
        int alive = 0;

        foreach(GameObject g in players)
        {
            if (!g.GetComponent<PlayerInventory>().dead)
            {
                alive++;
            }
        }
        Debug.Log("Checking Alive");
        if(alive == 1)
        {
            for(int i = 0; i<players.Count; i++)
            {
                if (!players[i].GetComponent<PlayerInventory>().dead)
                {
                    scores[i]++;
                    string survivingP = "P" + (i+1).ToString();
                    src.Play();
                    endRoundUI.SetActive(true);
                    endRoundUI.GetComponent<EndRoundUIScript>().UpdateRoundUI(scores, survivingP);
                    break;
                }
            }
            
            StartCoroutine(RestartGame(roundEndTimer));
            
        }
        else if(alive == 0)
        {
            endRoundUI.SetActive(true);
            endRoundUI.GetComponent<EndRoundUIScript>().UpdateRoundUI(scores, "Nobody");
            StartCoroutine(RestartGame(roundEndTimer));
        }
    }

    IEnumerator RestartGame(float time)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("Restarting Game");
        foreach (GameObject g in players)
        {
            if (g.TryGetComponent(out PlayerInventory p))
            {
                p.dead = false;
                p.currentHealth = p.maxHealth;
                p.resetInventory();
            }
        }
        SceneManager.LoadSceneAsync(mapToLoad, LoadSceneMode.Single);
    }


    public void ReturnToLobby(GameObject g)
    {
        //
        if (g.GetComponent<PlayerInventory>().playerNumber == 1)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
