using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        title,
        overworld,
        battle,
        pause
    }

    [SerializeField] string battleSceneName;
    [SerializeField] GameObject mapCam;
    [SerializeField] GameObject threeCee;
    [SerializeField] Vector2Int mapDestination;
  
    GameState currentGameState;

    public static GameManager instance;

    public event Action<bool> loadingScreenVisibilityEvent = delegate { };
    public event Action<bool> startScreenVisibilityEvent = delegate { };
    public event Action<bool> pauseScreenVisibilityEvent = delegate { };


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
            TransitionToState(GameState.overworld);
        }
    }
    
     void TransitionToState(GameState newState)
    {
        GameState tmpInitialState = currentGameState;
        OnStateExit(tmpInitialState, newState);
        currentGameState = newState;
        OnStateEnter(newState, tmpInitialState);
    }
    public void OnStateExit(GameState state, GameState toState)
    {
        switch (state)
        {
            case GameState.title:
                break;
            case GameState.overworld:
                break;
            case GameState.battle:
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
            case GameState.overworld:
                {
                    mapCam.SetActive(true);
                    threeCee.SetActive(false);
                }
                break;
            case GameState.battle:
                {
                    mapCam.SetActive(false);
                    threeCee.SetActive(true);
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
    }


    public void LoadBattleMap(string sceneName)
    {
        if (currentGameState == GameState.battle) return;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        print("scene index" + scene.buildIndex);
        StartCoroutine(LoadScene(sceneName));

    }

    public void ReturnToOverworld()
    {
        if (currentGameState != GameState.battle) return;
            TransitionToState(GameState.battle);
    }

    IEnumerator LoadScene(String sceneToLoad)
    {
        loadingScreenVisibilityEvent(true);
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }

        yield return new WaitForSeconds(1);

        TransitionToState(GameState.battle);
        loadingScreenVisibilityEvent(false);
        TeleportPlayerToMapPoint(mapDestination);
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

                case GameState.battle:
                    {
                        TransitionToState(GameState.pause);
                    }
                    break;
                case GameState.pause:
                    {
                        TransitionToState(GameState.battle);
                    }
                    break;
            }
    }
    public void OnTileClicked(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
        {
         //raycast to see what tile we hit. get the grid info. pass that to whatever's gonna teleport us to the map
        }
    }

    public void OnTileReleased(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Canceled)
        {
            // either cancel or raycast and load the selected scene
        }
    }

    //do this better and maybe not in this script
    public void TeleportPlayerToMapPoint(Vector2Int gridCoords)
    {
        if (WorldTilesManager.instance == null) return;

        var desinationTile = WorldTilesManager.instance.GetTileAtGridPosition(gridCoords);

        threeCee.transform.position = desinationTile.teleportPoint.position;
        threeCee.transform.rotation = desinationTile.teleportPoint.rotation;

    }

    [ContextMenu(" load test scene")]
    public void LoadTestScene()
    {
        LoadBattleMap(battleSceneName);
    }
  




}


