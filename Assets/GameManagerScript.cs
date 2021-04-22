using System.Collections;
using System.Collections.Generic;
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

    public void restartGame()
    {

    }


    public void initializeGame()
    {
        iManager.DisableJoining();
        SceneManager.LoadSceneAsync(mapToLoad, LoadSceneMode.Single);
    }

    private void OnSceneLoaded(Scene aScene, LoadSceneMode aMode)
    {
        foreach(GameObject g in players)
        {
          //  g.transform.position = 
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
}
