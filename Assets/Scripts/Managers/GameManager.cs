using System;
using System.Collections;
using _3C.Player;
using Cinemachine;
using DefaultNamespace;
using DG.Tweening;
using SpiceTeamJamming.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Units;
using UnityEngine.UI;

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
    [SerializeField] CanvasGroup loaderCanvas;
    [SerializeField] GameObject playerInstance;
    [SerializeField] GameObject playerCharacter;
    [SerializeField] CinemachineVirtualCamera characterVirtualCamera;
    [SerializeField] Vector2Int mapDestination;
    [SerializeField] GameObject mapGraphics;
    [SerializeField] BoardManager boardManager;
    [Header ("input from player controller")]
    [SerializeField] PlayerController playerController;

    [Header("misc")]
    [SerializeField] string mapClickSound = "uiClickStone";

    [Header("Timer")]
    [SerializeField] float gameTime; // total duration of the game
    private bool timerRunning = false;
    float gameTimer;
    public event Action OnGameTimerElapsed = delegate { };

    public float GameTimer
    {
        get { return gameTimer; }
    }

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
      if (LevelTilesManager.instance != null)  LevelTilesManager.instance.ClearLevelForReset();
        yield return new WaitForEndOfFrame();
        boardManager.ClearMap();
        battleMapLoaded = false;
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
        gameTimer = gameTime;
        timerRunning = true;
        loaderCanvas.alpha = 0;
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

    }
    //ui switching and active cameras switched here on state enter
    public void OnStateEnter(GameState state, GameState fromState)
    {
        switch (state)
        {
     
            case GameState.map:
                {
                    timerRunning = true;
                    Time.timeScale = 1;
                    mapCamera.gameObject.SetActive(true);
                    UIRouter.GoToRoute(UIRouter.RouteType.Map);
                }
                break;
            case GameState.level:
                {
                    timerRunning = true;
                    Time.timeScale = 1;
                    mapCamera.gameObject.SetActive(false);
                    UIRouter.GoToRoute(UIRouter.RouteType.Battlefield);
                }
                break;
            case GameState.pause:
                {
                    timerRunning = false;
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
        HandleGameTimer();

        switch (currentGameState)
        {
            case GameState.map:
                {
                    if (playerController.inputState.pause)
                    {
                        Pause();
                        return;
                    }
                    ////hovering over tiles. 
                    TileHighlighting();

                    //actually selecting tiles and telporting there
                    if (playerController.inputState.confirm && !loadingBattleMap)
                    {
                        GameplayData.UIPressThisFrame = false; // no idea what this was for but keeping it here
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

                                if (AudioManager.instance != null) AudioManager.instance.PlaySingleClip(mapClickSound, SFXCategory.ui, 0, 0);

                                tile.gameObject.transform.DOPunchPosition((Vector3.up * 15), 0.4f, 1, 1, false).OnComplete(() =>
                                {
                                    tile.Unhighlight();
                                });

                                var tileIndex = tile.mapTileData.tileCoords;
                                boardManager.AnimateGridSelection(tileIndex.x, tileIndex.y, 0.0001f);
                                mapDestination = clickedTile.mapTileData.tileCoords;
                                TryTransitionToLevel();

                            }
                        }

                    }
                }
                break;
            case GameState.level:
                {
                    if (playerController.inputState.pause) Pause();
                }
                break;
            case GameState.pause:
                {
                    if (playerController.inputState.pause)
                    {
                        UnPause();
                        return;
                    }
                    if (playerController.inputState.menuBack)
                    {
                        if (UIRouter.CurrentRoute == UIRouter.RouteType.Pause)
                        {
                            UnPause();
                            return;
                        }
                        else UIRouter.GoToPreviousRoute();
                    }
              
                }
                break;
            default:
                break;
        }
    }

    private void HandleGameTimer()
    {
        if (timerRunning)
        {
            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0)
            {
                gameTimer = 0;
                timerRunning = false;
                OnGameTimerElapsed?.Invoke();
            }
        }
    }

    public void LoadLevel(string sceneName)
    {
        if (currentGameState == GameState.level) return;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        loadingBattleMap = true;
        print("scene index" + scene.buildIndex);
        StartCoroutine(LoadLevelScene(sceneName));
    }
    float fadeDuration = 0.5f;
    IEnumerator LoadLevelScene(String sceneToLoad)
    {
        float elapsedTime = 0f;
        
            yield return new WaitForSeconds(0.5f);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                loaderCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                yield return null;
            }
        


        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }

        OnInitialLevelLoad?.Invoke();

        if (LevelTilesManager.instance == null)
        {
            print("world map not generated! tile manager not found");
            yield return null;
        }
        LevelTilesManager.instance.GenerateTiles();
        loadingBattleMap = false;

        StartCoroutine(TransitionToLevel());
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
        playerCharacter.SetActive(false);
    }
    void TryTransitionToLevel()
    {
        if (battleMapLoaded) StartCoroutine(TransitionToLevel());
        else LoadLevel(battleSceneName); // this ends up being async that's why it's like this
    }
    IEnumerator TransitionToLevel()
    {
        float elapsedTime = 0f;
        if (battleMapLoaded)
        {
            yield return new WaitForSeconds(0.5f);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                loaderCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                yield return null;
            }
        }

        //   Debug.Log("transition 2");
        var spawnTile = LevelTilesManager.instance.GetTileAtGridPosition(mapDestination);
        spawnTile.WakeUp();
        var spawnPos = spawnTile.teleportPoint.position;

        Vector3 oldPosition = playerCharacter.transform.position;
        playerInstance.transform.position = spawnPos;
        playerCharacter.transform.localPosition = Vector3.zero;
        Vector3 movementOffset = playerCharacter.transform.position - oldPosition;
        playerCharacter.SetActive(true);
        characterVirtualCamera.OnTargetObjectWarped(playerCharacter.transform, movementOffset);

        playerAnimator.SetTrigger("Teleport In");
        mapGraphics.SetActive(false); /// do this better

        TransitionToState(GameState.level);
        UIRouter.GoToRoute(UIRouter.RouteType.Battlefield);


        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            loaderCanvas.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        loaderCanvas.alpha = 0;
    }

    public void Pause()
    {

        if (currentGameState != GameState.pause) TransitionToState(GameState.pause);
    }

    public void UnPause()
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
    float highlightCooldownTimer;
    private void TileHighlighting()
    {
        if (loadingBattleMap) return;

        RaycastHit hoverHit;

        Ray hoverRay = mapCamera.ScreenPointToRay(GameplayData.CursorPosition);
        if (Physics.Raycast(hoverRay, out hoverHit))
        {

            hoverHit.collider.gameObject.TryGetComponent(out GameTile hoverTile);
            if (hoverTile != null && cachedHoverTile != hoverTile && Time.time > highlightCooldownTimer)
            {
                highlightCooldownTimer = Time.time + 0.05f;
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




