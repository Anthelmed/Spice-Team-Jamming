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
    pause,
    gameOver
}

public enum GameResult
{
    Win,
    Lose
}
public class GameManager : MonoBehaviour
{

    [Header("references")]

    [SerializeField] string gameSceneName;
    [SerializeField] string mainMenuBackgroundSceneName;
    [SerializeField] CanvasGroup loaderCanvas;
    [SerializeField] GameObject playerInstance;
    [SerializeField] GameObject playerCharacter;

    [SerializeField] CinemachineVirtualCamera characterVirtualCamera;
    [SerializeField] Vector2Int mapDestination;
    [Header ("input from player controller")]
    [SerializeField] PlayerController playerController;
    [SerializeField] Transform playerTeleportLimbo;

    [Header("misc")]
    [SerializeField] string mapClickSound = "uiClickStone";
    [SerializeField] CanvasGroup tempCanvas;
    [SerializeField] GameObject tempLose;
    [SerializeField] GameObject tempWin;

    [Header("Game Length (in minutes)")]
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
    GameResult result;

    public static GameManager instance;

    public static event Action<GameTile> onHoverTileChanged;
    public event Action OnInitialLevelLoad = delegate { };

    Animator playerAnimator;// DO this much better
    bool transitioning;
    
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

    private void Start()
    {
        loaderCanvas.alpha = 1;
        TransitionToState(GameState.title);
    }

    public void NewGame()
    {
        print("new game clicked");
        StartCoroutine(NewGameCoroutine());
    }

 
    public void StartGame()
    {
        loaderCanvas.DOFade(0, 0.3f);
        playerCharacter.GetComponent<Unit>().ResetHealth(); // do this better
        playerCharacter.SetActive(false);
        BoardManager.instance.mapGraphics.SetActive(true);

        gameTimer = gameTime * 60;
        timerRunning = true;
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
                    BoardManager.instance.mapCamera.gameObject.SetActive(true);
                    UIRouter.GoToRoute(UIRouter.RouteType.Map);
                }
                break;
            case GameState.level:
                {
                    timerRunning = true;
                    Time.timeScale = 1;
                    BoardManager.instance.mapCamera.gameObject.SetActive(false);
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
                case GameState.title:
                {
                    tempWin.SetActive(false);
                    tempLose.SetActive(false);
                    tempCanvas.gameObject.SetActive(false);

                    timerRunning = false;
                     Time.timeScale = 1;
                     StartCoroutine(ToMainMenuCoroutine());
                     
                }
                break;
                case GameState.gameOver:
                {
                    tempCanvas.gameObject.SetActive(true);
                    Time.timeScale = 0;
                    switch (result)
                    {
                        case GameResult.Win:
                        {
                                tempWin.SetActive(true);
                        }
                        break;
                        case GameResult.Lose:
                        {
                             tempLose.SetActive(true);
                        }
                       break;
                    }
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
                    if (playerController.inputState.confirm && !transitioning)
                    {
                        GameplayData.UIPressThisFrame = false; // no idea what this was for but keeping it here
                        Vector2 mousePosition = GameplayData.CursorPosition;
                        Ray ray = BoardManager.instance.mapCamera.ScreenPointToRay(mousePosition);
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
                                BoardManager.instance.AnimateGridSelection(tileIndex.x, tileIndex.y, 0.00001f);
                                mapDestination = clickedTile.mapTileData.tileCoords;
                                StartCoroutine( TransitionToLevel());

                            }
                        }

                    }
                }
                break;
            case GameState.level:
                {
                    if (playerController.inputState.pause) Pause();
                    if (playerController.inputState.mapFromGame) TryTransitionToMap();
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
                result = GameResult.Win;
                TransitionToState(GameState.gameOver);
               
                OnGameTimerElapsed?.Invoke();
            }
        }
    }

    IEnumerator ToMainMenuCoroutine()
    {
        loaderCanvas.DOFade(1, 0.3f);
        yield return new WaitForSecondsRealtime(.3f);
         UIRouter.GoToRoute(UIRouter.RouteType.Main);
        if (SceneManager.GetSceneByName(gameSceneName).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(gameSceneName);
            while (!asyncUnload.isDone)
            {
                print("un loading old game scene");
                yield return null;
            }
            print("finished unloading");
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainMenuBackgroundSceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(mainMenuBackgroundSceneName));
        UIRouter.GoToRoute(UIRouter.RouteType.Main);
        loaderCanvas.DOFade(0, 0.3f);
    }
    IEnumerator NewGameCoroutine()
    {
        loaderCanvas.DOFade(1, 0.3f);
        yield return new WaitForSecondsRealtime(.3f);
        if (SceneManager.GetSceneByName(mainMenuBackgroundSceneName).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(mainMenuBackgroundSceneName);
            while (!asyncUnload.isDone)
            {
                print("un loading main meny background");
                yield return null;
            }
            print("finished unloading");
        }
        if (SceneManager.GetSceneByName(gameSceneName).isLoaded)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(gameSceneName);
            while (!asyncUnload.isDone)
            {
                print("un loading old game scene");
                yield return null;
            }
            print("finished unloading");
        }
        StartCoroutine(LoadGameScene());

    }
    
    float fadeDuration = 0.5f;
    IEnumerator LoadGameScene()
    {

        //do loading screen fade stuff here
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameSceneName));
        yield return new WaitForEndOfFrame();
        if (BoardManager.instance == null)
        {
            print("map not generated! board manager not found");
            yield return null;
        }
        yield return new WaitForEndOfFrame();

        BoardManager.instance.Generate();
        yield return new WaitForEndOfFrame();
        OnInitialLevelLoad?.Invoke();

        yield return new WaitForEndOfFrame();

        if (LevelTilesManager.instance == null)
        {
            print("level not generated! world tile manager not found");
            yield return null;
        }
        LevelTilesManager.instance.GenerateTiles();
        yield return new WaitForSecondsRealtime(0.3f);

        StartGame();
    }
     public void TryTransitionToMainMenu()
    {
        switch (currentGameState)
        {
            case GameState.map:
                {
                    TransitionToState(GameState.title);
                }
                break;
            case GameState.level:
                {
                    TransitionToState(GameState.title);
                }
                break;
            case GameState.gameOver:
                {
                    TransitionToState(GameState.title);
                }
                break;
            default:
                break;
        }
    }

    public void TryTransitionToMap()
    {
        if (currentGameState != GameState.level) return;
        Invoke(nameof(DeactivatePlayer), 1f);
        playerAnimator.SetTrigger("Teleport Out");
        StartCoroutine(TransitionToMap());
    }

    float mapFade = 0.5f;
    IEnumerator TransitionToMap()
    {

        transitioning = true;
        float elapsedTime = 0f;
        yield return new WaitForSeconds(0.7f);

        while (elapsedTime < mapFade)
        {
            elapsedTime += Time.deltaTime;
            loaderCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / mapFade);
            yield return null;
        }

        var wizTile = LevelTilesManager.instance.playerTileIndex;
        BoardManager.instance.AnimateReturnToMap(wizTile.x, wizTile.y, 0.00003f);

        playerCharacter.SetActive(false);
        CancelInvoke(nameof(DeactivatePlayer));
        playerInstance.transform.position = playerTeleportLimbo.position;
        playerCharacter.transform.position = playerTeleportLimbo.position;
        LevelTilesManager.instance.SleepAllTiles();

        BoardManager.instance.mapGraphics.SetActive(true); /// do this better
        TransitionToState(GameState.map);

        elapsedTime = 0f;
        while (elapsedTime < mapFade)
        {
            elapsedTime += Time.deltaTime;
            loaderCanvas.alpha = Mathf.Lerp(1, 0, elapsedTime / mapFade);
            yield return null;
        }

        loaderCanvas.alpha = 0;
        transitioning = false;
    }

    void DeactivatePlayer()
    {
        playerCharacter.SetActive(false);
    }

    IEnumerator TransitionToLevel()
    {
        transitioning = true;
        float elapsedTime = 0f;

            yield return new WaitForSeconds(0.5f);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                loaderCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
                yield return null;
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
        BoardManager.instance.mapGraphics.SetActive(false); /// do this better

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
        transitioning = false;
    }

    public void Pause()
    {

        if (currentGameState != GameState.pause) TransitionToState(GameState.pause);
    }

    public void UnPause()
    {
        if (currentGameState == GameState.pause) TransitionToState(lastState);
    }

    public void LoseGame()
    {
        result = GameResult.Lose;
        TransitionToState(GameState.gameOver);
        StartCoroutine(ReturnToStart());
    }

    IEnumerator ReturnToStart()
    {
        yield return new WaitForSecondsRealtime(3);
        TryTransitionToMainMenu();
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


        RaycastHit hoverHit;

        Ray hoverRay = BoardManager.instance.mapCamera.ScreenPointToRay(GameplayData.CursorPosition);
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


}




