using System;
using System.Collections;
using _3C.Player;
using DefaultNamespace;
using DG.Tweening;
using SpiceTeamJamming.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Units;

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
    [SerializeField] GameObject playerInstance;
    [SerializeField] GameObject playerCharacter;
    [SerializeField] Vector2Int mapDestination;
    [SerializeField] GameObject mapGraphics;
    [SerializeField] BoardManager boardManager;
    [Header ("input from player controller")]
    [SerializeField] PlayerController playerController;

    [Header("misc")]
    [SerializeField] string mapClickSound = "uiClickStone";

    GameState currentGameState;
    GameState lastState;

    public static GameManager instance;

    public static event Action<GameTile> onHoverTileChanged;
    public event Action OnInitialLevelLoad = delegate { };

    Animator playerAnimator;// DO this much better
    bool battleMapLoaded;
    bool loadingBattleMap;
    
    public event Action<GameState> OnGameStateChanged = delegate { };


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;

        }
        playerAnimator = playerInstance.GetComponentInChildren<Animator>(true);
    }

    public void NewGame()
    {
        TransitionToState(GameState.title);
        StartCoroutine(ResetGame());
    }
    IEnumerator ResetGame()
    {
        Time.timeScale = 0f;
        LevelTilesManager.instance.ClearLevelForReset();
        yield return new WaitForEndOfFrame();
        boardManager.ClearMap();
        yield return new WaitForEndOfFrame();
        boardManager.Generate();
        playerCharacter.GetComponent<Unit>().ResetHealth(); // do this better
        playerCharacter.SetActive(false);
        mapGraphics.SetActive(true);
        Time.timeScale = 1f;
        StartGame();
    }

    private void Start()
    {
        StartGame();
    }
    public void StartGame()
    {
        TransitionToState(GameState.map);
    }
    
    public GameState GetCurrentGameState()
    {
        return currentGameState;
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

            case GameState.map:
                {
                }
                break;
            case GameState.level:
                {
                }
                break;
            case GameState.pause:
                {
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
     
            case GameState.map:
                {
                    Time.timeScale = 1;
                    mapCamera.gameObject.SetActive(true);
                    UIRouter.GoToRoute(UIRouter.RouteType.Map);
                }
                break;
            case GameState.level:
                {
                    Time.timeScale = 1;
                    mapCamera.gameObject.SetActive(false);
                    UIRouter.GoToRoute(UIRouter.RouteType.Battlefield);
                }
                break;
            case GameState.pause:
                {
                    lastState = fromState;
                    Time.timeScale = 0;
                    UIRouter.GoToRoute(UIRouter.RouteType.Pause);
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
            case GameState.map:
                {
                    ////hovering over tiles. for juice only
                    HighlightingJuice();
                    if (UIRouter.CurrentRoute != UIRouter.RouteType.Map)
                    {
                        UIRouter.GoToRoute(UIRouter.RouteType.Map);
                        
                    }

                    //actually selecting tiles and telporting there
                    if (playerController.inputState.confirm)
                    {
                        GameplayData.UIPressThisFrame = false;
                        Vector2 mousePosition = GameplayData.CursorPosition;
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
                                Debug.Log("here");
                                //tile.gameObject.transform.DOPunchScale(tile.gameObject.transform.localScale * 1.1f, 0.4f, 5, 0);
                                tile.gameObject.transform.DOPunchPosition((Vector3.up * 15), 0.4f, 1, 1, false).OnComplete(() =>
                                {
                                    mapDestination = clickedTile.mapTileData.tileCoords;
                                    TryTransitionToLevel();
                                    tile.Unhighlight();
                                    //tile.gameObject.transform.position = cachedPos;
                                    
                                });
                            }
                        }

                    }
                }
                break;
            case GameState.level:
                break;
            case GameState.pause:
                {
                if (playerController.inputState.menuBack) UIRouter.GoToPreviousRoute();
                if (playerController.inputState.mapBack) UIRouter.GoToPreviousRoute();
               // if (playerController.inputState.mapBack) TogglePause();
                }
                break;
            default:
                break;
        }  
    }

    public void LoadLevel(string sceneName)
    {
        Debug.Log("transition 3");
        if (currentGameState == GameState.level) return;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        loadingBattleMap = true;
        print("scene index" + scene.buildIndex);
        StartCoroutine(LoadLevelScene(sceneName));
    }

    IEnumerator LoadLevelScene(String sceneToLoad)
    {
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }

        yield return new WaitForEndOfFrame(); // make this longer if there's an actual loading screen that can appear

        OnInitialLevelLoad?.Invoke();

        if (LevelTilesManager.instance == null)
        {
            print("world map not generated! tile manager not found");
            yield return null;
        }
        LevelTilesManager.instance.GenerateTiles();
        loadingBattleMap = false;
        TransitionToLevel();
        battleMapLoaded = true;
    }

    public void TryTransitionToMap()
    {
        if (currentGameState != GameState.level) return;
        playerCharacter.SetActive(false);
        CancelInvoke(nameof(DeactivatePlayer));
        Invoke(nameof(DeactivatePlayer), 1f);
        playerAnimator.SetTrigger("Teleport Out");


        LevelTilesManager.instance.SleepAllTiles();

        mapGraphics.SetActive(true); /// do this better
        TransitionToState(GameState.map);
    }
    void DeactivatePlayer()
    {
      //  playerInstance.SetActive(false);
        playerCharacter.SetActive(false);
    }
    void TryTransitionToLevel()
    {
        Debug.Log("transition 1");
        if(loadingBattleMap) return;
        if (battleMapLoaded) TransitionToLevel();
        else LoadLevel(battleSceneName); // this ends up being async that's why it's like this
    }
    void TransitionToLevel()
    {
     //   Debug.Log("transition 2");
        var spawnTile = LevelTilesManager.instance.GetTileAtGridPosition(mapDestination);
        spawnTile.WakeUp();
        var spawnPos = spawnTile.teleportPoint.position;

        playerInstance.transform.position = spawnPos;
        playerCharacter.transform.localPosition = Vector3.zero;
        playerCharacter.SetActive(true);

        playerAnimator.SetTrigger("Teleport In");
        mapGraphics.SetActive(false); /// do this better

        TransitionToState(GameState.level);
        UIRouter.GoToRoute(UIRouter.RouteType.Battlefield);

    }

    public void TogglePause()
    {

        if (currentGameState != GameState.pause) TransitionToState(GameState.pause);
    }

    public void ToggleUnPause()
    {
        if (currentGameState == GameState.pause) TransitionToState(lastState);
    }


    public void TeleportPlayerToMapPoint(Vector2Int gridCoords)
    {
        if (LevelTilesManager.instance == null) return;
        Debug.Log("teleporting");
        var desinationTile = LevelTilesManager.instance.GetTileAtGridPosition(gridCoords);

        playerInstance.transform.position = desinationTile.teleportPoint.position;
        playerInstance.transform.rotation = desinationTile.teleportPoint.rotation;

    }
    private void HighlightingJuice()
    {
        RaycastHit hoverHit;

        Ray hoverRay = mapCamera.ScreenPointToRay(GameplayData.CursorPosition);
        if (Physics.Raycast(hoverRay, out hoverHit))
        {

            hoverHit.collider.gameObject.TryGetComponent(out GameTile hoverTile);
            if (hoverTile != null && cachedHoverTile != hoverTile)
            {
                if (cachedHoverTile != null)
                {
                    cachedHoverTile.Unhighlight();
                }
                cachedHoverTile = hoverTile;
                onHoverTileChanged?.Invoke(cachedHoverTile);

                if (hoverTile.mapTileData.biome == Biome.Water)return; // dont juice a place you cant go

                cachedHoverTile.Highlight();
            }
        }
    }

    [ContextMenu(" load test scene")]
    public void LoadTestScene()
    {
        LoadLevel(battleSceneName);
    }
  
   




}




