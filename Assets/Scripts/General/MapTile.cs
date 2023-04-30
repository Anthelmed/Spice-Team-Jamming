using System;
using DG.Tweening;
using Units;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    [SerializeField] private Transform centerPoint;
    [SerializeField] private GameObject highlight;
    // [SerializeField] private GameObject spawnVFX;
    
    [SerializeField] private Biome biome;
    public Biome Biome => biome;

    private Renderer _renderer;
    public Renderer Renderer => _renderer;

    private bool _isObstacle = false;
    private bool _isSpawnArea = false;
    private bool _isOccupied = false;

    Vector3 startPos;
    Vector3 startScale;
    public TreeVisuals[] treeVisuals;
    public Transform playerIndicator;
    public MapTileData mapTileData = new MapTileData();

    public static event Action<MapTileData> MapTileClicked = delegate { };
    public static event Action<MapTileData> MapTileStatusChanged = delegate { };
    public bool IsOccupied
    {
        get => _isOccupied;
        set => _isOccupied = value;
    }
    
    public bool IsObstacle
    {
        get => _isObstacle;
        set => _isObstacle = value;
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        playerIndicator.gameObject.SetActive(false);
        GameManager.instance.OnInitialLevelLoad += SetupEvent;
    }
    void OnDestroy()
    {
            GameManager.instance.OnInitialLevelLoad -= SetupEvent;
    }

    public bool IsSpawnArea
    {
        get => _isSpawnArea;
        set => _isSpawnArea = value;
    }
    public Transform contestedIndicator;
    // public Transform SpawnPoint => spawnPoint;

    bool hasPlayer = false;
    public void SetTileState(WorldTileStatus status)
    {
       
        foreach (var tree in treeVisuals)
        {
            switch (status)
            {
                case WorldTileStatus.neutral:
                {
                    if(mapTileData.tileStatus == WorldTileStatus.contested)
                    {
                        contestedIndicator.DOKill();
                        contestedIndicator.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => contestedIndicator.gameObject.SetActive(false));
                    }
                    tree.SetNatureState();
                    break;
                }
                case WorldTileStatus.contested:
                {
                    contestedIndicator.gameObject.SetActive(true);
                    contestedIndicator.DOKill();
                    contestedIndicator.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
                    tree.SetNatureState();
                    break;
                }
                case WorldTileStatus.frozen:
                {
                    if(mapTileData.tileStatus == WorldTileStatus.contested)
                    {
                        contestedIndicator.DOKill();
                        contestedIndicator.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => contestedIndicator.gameObject.SetActive(false));
                    }
                    tree.SetFrozenState();
                    break;
                }
                case WorldTileStatus.burnt:
                {
                    if(mapTileData.tileStatus == WorldTileStatus.contested)
                    {
                        contestedIndicator.DOKill();
                        contestedIndicator.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() => contestedIndicator.gameObject.SetActive(false));
                    }
                    tree.SetBurntState();
                    break;
                }
                default:
                break;
            }
        }
         mapTileData.tileStatus = status;

         MapTileStatusChanged?.Invoke(mapTileData);
    }
    public void OnPlayerMove(Vector2Int newTile)
    {
        if(hasPlayer && newTile != mapTileData.tileCoords)
        {
            playerIndicator.gameObject.SetActive(false);
        }
        else if (newTile == mapTileData.tileCoords)
        {
            playerIndicator.gameObject.SetActive(true);
            hasPlayer = true;
        }
    }
    public void Highlight()
    {
        transform.DOKill();
        transform.DOMoveY(startPos.y + 7.5f, 0.4f).SetEase(Ease.OutBack);
        // transform.DOMove(startPos + Vector3.up * 7.5f, 0.4f).SetEase(Ease.OutBack);
        transform.DOScale(startScale * 1.25f, 0.4f).SetEase(Ease.OutBack);
        highlight.SetActive(true);
        // if (!_isObstacle)
        //     isHighlighted = true;
    }

    public void Unhighlight()
    {
        highlight.SetActive(false);

        transform.DOKill();
         transform.DOScale(startScale, 0.2f).SetEase(Ease.InBack);
        transform.DOMove(startPos, 0.2f).SetEase(Ease.InBack);
        // isHighlighted = false;
    }

    internal void InitTileData(Biome tileBiome, Vector2Int coords)
    {
        mapTileData.tileCoords = coords;
        mapTileData.biome = tileBiome;
        mapTileData.tileStatus = WorldTileStatus.neutral;
        startPos = transform.position;
        startScale = transform.localScale;
        if (contestedIndicator != null)
        {
            contestedIndicator.DOKill();
            contestedIndicator.localScale = Vector3.zero;
            contestedIndicator.gameObject.SetActive(false);

            foreach (var tree in treeVisuals)
            {
                tree.SetNatureState(true);
            }
        }

    }
    void SetupEvent()
    {
        LevelTilesManager.instance.PlayerTileIndexChanged += OnPlayerMove;
    }


    // private void OnDrawGizmos()
    // {
    //     if (isHighlighted)
    //         Gizmos.DrawCube(spawnPoint.transform.position, Vector3.one * 0.1f);
    // }
    //

}

public struct MapTileData
{
    public Biome biome;
    public WorldTileStatus tileStatus;
    public Vector2Int tileCoords;
}
