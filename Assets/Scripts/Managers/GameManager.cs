using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum GameState
{
    title,
    map,
    level,
    pause
}
public class GameManager : MonoBehaviour
{
 
    [Header("references")]
    [SerializeField] string battleSceneName;
    [SerializeField] Camera mapCamera;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Vector2Int mapDestination;
    [SerializeField] GameObject mapGraphics;
    
    [Header("misc")]
    [SerializeField] string mapClickSound = "uiClickStone";

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

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
            HideAllPanels();
            TransitionToState(GameState.map);
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
    GameTile cachedHoverTile;
    bool wigglin;
     

    void Update()
    {

        switch (currentGameState)
        {
            case GameState.title:
                break;
            case GameState.map:
                {
                    ////hovering over tiles. for juice only
                    HighlightingJuice();


                    //actually selecting tiles and telporting there
                    if (Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        Vector2 mousePosition = Mouse.current.position.ReadValue();
                        Ray ray = mapCamera.ScreenPointToRay(mousePosition);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            GameObject hitObject = hit.collider.gameObject;
                            hitObject.TryGetComponent(out GameTile tile);

                            if (tile != null)
                            {
                                clickedTile = tile;
                                if (tile.mapTileData.biome == Biome.Water)
                                {
                                    print("you cant swim!");
                                    return;
                                }
                                var cachedPos = tile.gameObject.transform.position;

                                if (AudioManager.instance != null)  AudioManager.instance.PlaySingleClip(mapClickSound, SFXCategory.ui, 0, 0);

                                tile.gameObject.transform.DOPunchPosition((Vector3.up * 15), 0.8f, 1, 1, false).OnComplete(() =>
                                {
                                    mapDestination = clickedTile.mapTileData.tileCoords;
                                    tile.gameObject.transform.position = cachedPos;
                                    TryTransitionToLevel();

                                });



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
    private void HighlightingJuice()
    {
        RaycastHit hoverHit;

        Ray hoverRay = mapCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(hoverRay, out hoverHit))
        {
            hoverHit.collider.gameObject.TryGetComponent(out GameTile hoverTile);
            if (hoverTile != null && !wigglin && cachedHoverTile != hoverTile)
            {
                if (hoverTile.mapTileData.biome == Biome.Water) return; // dont juice a place you cant go

                wigglin = true;
                cachedHoverTile = hoverTile;
                var cachedTilePos = hoverTile.gameObject.transform.position;
                hoverTile.gameObject.transform.DOPunchPosition(Vector3.up * 5, 0.15f, 1, 1, false).OnComplete(() =>
                {
                    wigglin = false;
                    hoverTile.gameObject.transform.position = cachedTilePos;
                });
            }
        }
    }

    [ContextMenu(" load test scene")]
    public void LoadTestScene()
    {
        LoadLevel(battleSceneName);
    }
  




}


