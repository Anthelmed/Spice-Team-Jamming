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

    [SerializeField] string testBattleMapName;
    [SerializeField] GameObject mapGraphics;
    UIPanel[] allPanels;
  
    GameState currentGameState;
    String currentBattleScene;

    public static GameManager instance;

    float pauseCoolDown;

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
                break;
            case GameState.battle:
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

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.title:
                {
                    //on press any key start gameplay
                }
                break;
            case GameState.overworld:
                {
                    //on click tile load scene
                }
                break;
            case GameState.battle:
                {
                    // if (pausePressed)
                  //  TransitionToState(GameState.pause);
                }
                break;
            case GameState.pause:
                {
                  //   if (pausePressed)
                  //   TransitionToState(GameState.battle);
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
        if (currentGameState == GameState.battle && currentBattleScene != null)
        {
            SceneManager.UnloadSceneAsync(currentBattleScene);
            TransitionToState(GameState.battle);
            mapGraphics.SetActive(true); /// do this in the map script. just the graphics should get turned off
        }
    }

    IEnumerator LoadScene(String sceneToLoad)
    {
        foreach (var panel in allPanels) panel.Hide();
        loadingScreenVisibilityEvent(true);
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }
        currentBattleScene = sceneToLoad;

        yield return new WaitForSeconds(1);

        mapGraphics.SetActive(false); /// do this in the map script. just the graphics should get turned off

        TransitionToState(GameState.battle);
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
          // wait for the hold
        }
    }

    public void OnTileReleased(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Canceled)
        {
            // either cancel or raycast and load the selected scene
        }
    }




    /// <summary>
    /// //debbuggin stuff below here
    /// </summary>
    public void LoadTestScene()
    {
        LoadBattleMap(testBattleMapName);
    }





}


