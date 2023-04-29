using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public enum GameState
{
    title,
    map,
    level,
    pause
}
public class GameManager : MonoBehaviour
{
 

    [SerializeField] string battleSceneName;
    [SerializeField] Camera mapCamera;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector2Int mapDestination;
    [SerializeField] GameObject mapGraphics;

  
    GameState currentGameState;

    public static GameManager instance;

    public event Action<bool> loadingScreenVisibilityEvent = delegate { };
    public event Action<bool> startScreenVisibilityEvent = delegate { };
    public event Action<bool> pauseScreenVisibilityEvent = delegate { };

    GameObject loadedPlayer;
    bool battleMapLoaded;

    public event Action<GameState> OnGameStateChanged = delegate { };

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;

        }

        HideAllPanels();
        startScreenVisibilityEvent(true);
    }

 
    public void StartGame()
    {
        if (currentGameState == GameState.title)
        {
            HideAllPanels();
            TransitionToState(GameState.map);
        }
    }
    
     void TransitionToState(GameState newState)
    {
        GameState tmpInitialState = currentGameState;
        OnStateExit(tmpInitialState, newState);
        currentGameState = newState;
        OnStateEnter(newState, tmpInitialState);
    }

    /// <summary>
    /// /state
    /// </summary>

    public void OnStateExit(GameState state, GameState toState)
    {
        switch (state)
        {
            case GameState.title:
                break;
            case GameState.map:
                break;
            case GameState.level:
                break;
            case GameState.pause:
                {
                    pauseScreenVisibilityEvent(false);
                }
                break;
            default:
                break;
        }
    }
    public void OnStateEnter(GameState state, GameState fromState)
    {
        switch (state)
        {
            case GameState.title:
                break;
            case GameState.map:
                {
                    mapCamera.gameObject.SetActive(true);
                   
                }
                break;
            case GameState.level:
                {
                    mapCamera.gameObject.SetActive(false);
                }
                break;
            case GameState.pause:
                {
                    pauseScreenVisibilityEvent(true);
                    //switch input map
                }
                break;
            default:
                break;
        }
        OnGameStateChanged(state);
    }


     GameTile clickedTile;
     bool isHoldingDown = false;
     float holdTimer;
     float holdDownDuration = 1.0f;


    void Update()
    {

        switch (currentGameState)
        {
            case GameState.title:
                break;
            case GameState.map:
                {
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        Vector2 mousePosition = Mouse.current.position.ReadValue();
                        Ray ray = mapCamera.ScreenPointToRay(mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            Debug.Log("Clicked on " + hitObject.name);
                            hitObject.TryGetComponent(out GameTile tile);

                        if (tile != null)
                        {
                                print("found game tile on object");
                                // Player clicked on a tile, start the timer
                                clickedTile = tile;
                                mapDestination = clickedTile.mapTileData.tileCoords;
                                TryTransitionToLevel();
                            }
                    }
              
                }
                }
                break;
            case GameState.level:
                break;
            case GameState.pause:
                break;
            default:
                break;
        }  
    }


    public void LoadLevel(string sceneName)
    {
        if (currentGameState == GameState.level) return;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        print("scene index" + scene.buildIndex);
        StartCoroutine(LoadLevelScene(sceneName));
    }

    IEnumerator LoadLevelScene(String sceneToLoad)
    {
        loadingScreenVisibilityEvent(true);
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }

        yield return new WaitForSeconds(1);

        if (LevelTilesManager.instance == null)
        {
            print("world map not generated! tile manager not found");
            yield return null;
        }
        LevelTilesManager.instance.GenerateTiles();
        TransitionToLevel();
        battleMapLoaded = true;
    }

    public void TransitionToMap()
    {
        if (currentGameState != GameState.level) return;
        if (loadedPlayer != null) loadedPlayer.SetActive(false);

        LevelTilesManager.instance.SleepAllTiles();

        mapGraphics.SetActive(true); /// do this better
        TransitionToState(GameState.map);
    }

    void TryTransitionToLevel()
    {
        if (battleMapLoaded) TransitionToLevel();
        else LoadLevel(battleSceneName); // this ends up being async that's why it's like this
    }
    void TransitionToLevel()
    {

        var spawnTile = LevelTilesManager.instance.GetTileAtGridPosition(mapDestination);
        spawnTile.WakeUp();
        var spawnPos = spawnTile.teleportPoint.position;
 
        if (loadedPlayer == null)
        {
         loadedPlayer = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }
        else
        {
            loadedPlayer.transform.position = spawnPos;
            if (!loadedPlayer.activeInHierarchy) loadedPlayer.SetActive(true);

        }
      
        mapGraphics.SetActive(false); /// do this better

        TransitionToState(GameState.level);
        loadingScreenVisibilityEvent(false);

    }


    [ContextMenu(" hide all panels")]
    public void HideAllPanels()
    {
        startScreenVisibilityEvent(false);
        pauseScreenVisibilityEvent(false);
        loadingScreenVisibilityEvent(false);
    }

    public void OnTogglePause(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
            switch (currentGameState)
            {

                case GameState.level:
                    {
                        TransitionToState(GameState.pause);
                    }
                    break;
                case GameState.pause:
                    {
                        TransitionToState(GameState.level);
                    }
                    break;
            }
    }


    public void TeleportPlayerToMapPoint(Vector2Int gridCoords)
    {
        if (LevelTilesManager.instance == null) return;

        var desinationTile = LevelTilesManager.instance.GetTileAtGridPosition(gridCoords);

        playerPrefab.transform.position = desinationTile.teleportPoint.position;
        playerPrefab.transform.rotation = desinationTile.teleportPoint.rotation;

        //or this?
        //var spawnPos = WorldTilesManager.instance.GetTileAtGridPosition(mapDestination).teleportPoint.position;
        //Instantiate(playerPrefab, spawnPos, Quaternion.identity);

    }

    [ContextMenu(" load test scene")]
    public void LoadTestScene()
    {
        LoadLevel(battleSceneName);
    }
  




}


