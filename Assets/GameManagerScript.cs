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
    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> spawns = new List<GameObject>();
    public List<int> scores = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        iManager = transform.GetComponent<PlayerInputManager>();
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        iManager.DisableJoining();
        SceneManager.LoadSceneAsync(mapToLoad, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {

        if(SceneManager.GetActiveScene().name != "LobbyScene")
        {
            GameObject[] obs = GameObject.FindGameObjectsWithTag("Respawn");
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
        
    }

    public void dropPlayer(GameObject g)
    {
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
                    
                }
            }
            StartCoroutine(RestartGame(roundEndTimer));
        }
        else if(alive == 0)
        {
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
}
