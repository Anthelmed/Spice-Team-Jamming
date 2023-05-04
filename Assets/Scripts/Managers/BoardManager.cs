using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public enum Biome
{
    Grass,
    Desert,
    Mountain,
    Forest,
    Water,
}

public class BoardManager : MonoBehaviour
{
    // TODO move this into level data SO

    [SerializeField] Transform mapGraphicsParent;
    
    [Header ("Biomes")]
    [SerializeField] private BiomePrefabHolder grassPrefabHolder;
    [SerializeField] private BiomePrefabHolder desertPrefabHolder;
    [SerializeField] private BiomePrefabHolder mountainPrefabHolder;
    [SerializeField] private BiomePrefabHolder forestPrefabHolder;
    [SerializeField] private BiomePrefabHolder waterPrefabHolder;
    [SerializeField] private float sameBiomeChance = 0.5f;
    [SerializeField] private bool randomBiomeRotation = false;
    [SerializeField] private List<Biome> spawnableBiomes = new List<Biome>();

    private Dictionary<Biome, BiomePrefabHolder> _biomePrefabHolders = new Dictionary<Biome, BiomePrefabHolder>();

    [SerializeField] private Vector2Int widthHeight;
    [SerializeField] private Vector2 noiseScale;
    [SerializeField] private float noiseOffset;
    [SerializeField] private float noiseThreshold;
    
    public bool generateRoundMap = false;
    
    public static GameTile[,] MapTiles;
    private static float[,] _mapHeights;
    private Transform _transform;
    Transform mapTilesParent;
    
    [SerializeField] private bool debugMode = false;
    [SerializeField] float mapHeightOffset;

    public static BoardManager instance;
    public GameObject mapGraphics;
    public Camera mapCamera;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
        }

        InitMapArrays();

        _transform = transform;

        _biomePrefabHolders.Add(Biome.Grass, grassPrefabHolder);
        _biomePrefabHolders.Add(Biome.Desert, desertPrefabHolder);
        _biomePrefabHolders.Add(Biome.Mountain, mountainPrefabHolder);
        _biomePrefabHolders.Add(Biome.Forest, forestPrefabHolder);
        _biomePrefabHolders.Add(Biome.Water, waterPrefabHolder);

        noiseOffset = Random.Range(-100f, 100f);
    }

    private void InitMapArrays()
    {
        int x = Mathf.Max(1, widthHeight.x);
        int y = Mathf.Max(1, widthHeight.y);
        MapTiles = new GameTile[x, y];
        _mapHeights = new float[x, y];
    }


    public void Generate()
    {
        var tilesParent = new GameObject();
        tilesParent.name = "tilesParent";
        mapTilesParent = tilesParent.transform;
        mapTilesParent.parent = mapGraphicsParent;
       
        
        _GenerateMapHeights();
        _GenerateMapTiles();
    }
    public void ClearMap()
    {
        InitMapArrays();
        Destroy(mapTilesParent.gameObject);
    }
    
    public static int[] WorldPosToGrid(Vector3 pos)
    {
        if (MapTiles == null)
        {
            Debug.LogError("MapTiles is null");
            return new int[2] { 0, 0 };
        }
        
        // Vector3 localScale = fieldPrefab.transform.localScale;
        float xSize = 20;
        float zSize = 20;
        int xMax = MapTiles.GetLength(0);
        int zMax = MapTiles.GetLength(1);
        int xLoc = Mathf.Min(Mathf.Max(0, (int) Math.Round(pos.x / xSize)), xMax);
        int zLoc = Mathf.Min(Mathf.Max(0, (int) Math.Round(pos.z / zSize)), zMax);
        Debug.Log(xLoc + ", " + zLoc);
        // int zLoc =  (int) Math.Round(pos.z / zSize);
        return new int[2] { xLoc, zLoc };
            
    }
    
    public static int ManhattanDistance(Vector2Int A, Vector2Int B)
    {
        return Mathf.Abs(A.x - B.x) + Mathf.Abs(A.y - B.y);
    }
    
    

    private void _GenerateMapHeights()
    {

        int _x = Mathf.Max(1, widthHeight.x);
        int _y = Mathf.Max(1, widthHeight.y);
        if (_mapHeights == null)
            _mapHeights = new float[_x, _y];
        
        for (int x = 0; x < _mapHeights.GetLength(0); x++)
        {
            for (int y = 0; y < _mapHeights.GetLength(1); y++)
            {
                _mapHeights[x, y] = Mathf.PerlinNoise((x + noiseOffset) * noiseScale.x, (y + noiseOffset) * noiseScale.y);
                if (_mapHeights[x, y] >= noiseThreshold)
                {
                    _mapHeights[x, y] = 1;
                }
        
            }
        }
    }


    
    private Biome ReplaceIfSingleCell(Biome currentBiome, int currentX, int currentY)
    {
        List<Biome> neighbouringBiomes = new List<Biome>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                var localX = currentX - x;
                var localY = currentY - y;
                if (localX < 0 || localX >= _mapHeights.GetLength(0) || localY < 0 || localY >= _mapHeights.GetLength(1)) continue;
                if (MapTiles[localX, localY] == null) continue;
                neighbouringBiomes.Add(MapTiles[localX, localY].Biome);
            }
        }

        var tilesOfSameType = neighbouringBiomes.Where(biome => biome == currentBiome).ToList();

        if (tilesOfSameType.Count > 2) return currentBiome;
        ///
        if (neighbouringBiomes.Count == 0) return currentBiome; 
        ///
        var mostNeighbouringBiome = neighbouringBiomes.GroupBy(i=> i).OrderByDescending(grp=>grp.Count())
            .Select(grp=>grp.Key).First();
        return mostNeighbouringBiome;
    }
    
    
    private Biome GenerateCurrentBiomeType(int currentX, int currentY)
    {
        List<Biome> neighbouringBiomes = new List<Biome>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                var localX = currentX - x;
                var localY = currentY - y;
                if (localX < 0 || localX >= _mapHeights.GetLength(0) || localY < 0 || localY >= _mapHeights.GetLength(1)) continue;
                if (MapTiles[localX, localY] == null) continue;
                neighbouringBiomes.Add(MapTiles[localX, localY].Biome);
            }
        }
        
        Biome? currentRandomBiome = null;
        if (neighbouringBiomes.Count == 0)
        {

            var randomIdex = Random.Range(0, spawnableBiomes.Count);
            currentRandomBiome = spawnableBiomes[randomIdex];


            return currentRandomBiome.Value;
        }

        ///
        if (neighbouringBiomes.Count == 0) return Biome.Grass;
        ///
        var mostNeighbouringBiome = neighbouringBiomes.GroupBy(i=> i).OrderByDescending(grp=>grp.Count())
            .Select(grp=>grp.Key).First();

        
        if (Random.Range(0, 1f) < sameBiomeChance)
        {
            return mostNeighbouringBiome;
        }

 
        var randIndex = Random.Range(0, spawnableBiomes.Count);
        currentRandomBiome = spawnableBiomes[randIndex];
            
        
        return currentRandomBiome.Value;
        
    }

    private void _GenerateMapTiles()
    {
        int xNum = MapTiles.GetLength(0);
        int yNum = MapTiles.GetLength(1);
        
        
        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                
                if (_mapHeights[x, y] <= noiseThreshold) continue;
                // Round map?
                if (generateRoundMap && Mathf.Abs(Vector2.Distance(new Vector2(MapTiles.GetLength(0) / 2, MapTiles.GetLength(1) / 2), new Vector2(x, y))) >= widthHeight[0] / 2f) continue;
                
                Biome currentBiome = GenerateCurrentBiomeType(x,  y);
                
                
                var currentPrefab = _biomePrefabHolders[currentBiome].GetRandomBiomePrefab();
                
                var tilePrefabTransform = currentPrefab.transform;
                var localScale = new Vector3(20, 1, 20);// tilePrefabTransform.localScale;
                float xSize = localScale.x;
                float zSize = localScale.z;

                Vector3 pos = new Vector3(x * xSize, mapHeightOffset, y * zSize);

                // We can rotate the tile [0, 90, 180, 270] degrees to get more variation

                GameObject go = Instantiate(currentPrefab, pos, Quaternion.identity, _transform);
                go.transform.SetParent(mapGraphicsParent);
                go.transform.localEulerAngles = new Vector3(go.transform.localEulerAngles.x, Random.Range(0, 4) * 90f, go.transform.localEulerAngles.z);
                var tile = go.GetComponentInChildren<GameTile>();
                
                tile.InitTileData(currentBiome, new Vector2Int(x, y));

                MapTiles[x, y] = tile;
                
                
            }

        }
        
        

        for (int x = 0; x < xNum; x++)
        {
            for (int y = 0; y < yNum; y++)
            {
                
                if (MapTiles[x, y] == null)
                {
                    Biome currentBiome = Biome.Water;
                    var currentPrefab = _biomePrefabHolders[currentBiome].GetRandomBiomePrefab();
                    
                    var tilePrefabTransform = currentPrefab.transform;
                    var localScale = new Vector3(20, 1, 20);// tilePrefabTransform.localScale;;
                    float xSize = localScale.x;
                    float zSize = localScale.z;

                    Vector3 pos = new Vector3(x * xSize, mapHeightOffset, y * zSize);

                    // We can rotate the tile [0, 90, 180, 270] degrees to get more variation
                    
                    GameObject go = Instantiate(currentPrefab, pos, Quaternion.Euler(0, 90 * Random.Range(0, 4), 0), _transform);
                    var tile = go.GetComponentInChildren<GameTile>();
                    go.transform.SetParent(mapGraphicsParent);
                    tile.InitTileData(currentBiome, new Vector2Int(x,y));
                    tile.IsObstacle = true;
                    MapTiles[x, y] = tile;


                }
                else
                {

                    var currentBiome = MapTiles[x, y].Biome;
                    var mostCommonBiome = ReplaceIfSingleCell(currentBiome, x, y);
                    if (mostCommonBiome == MapTiles[x, y].Biome) continue;
                    Destroy(MapTiles[x, y].gameObject);
                    
                    var currentPrefab = _biomePrefabHolders[mostCommonBiome].GetRandomBiomePrefab();
                    
                    var tilePrefabTransform = currentPrefab.transform;
                    var localScale = new Vector3(20, 1, 20);// tilePrefabTransform.localScale;;
                    float xSize = localScale.x;
                    float zSize = localScale.z;
                
                    Vector3 pos = new Vector3(x * xSize, mapHeightOffset, y * zSize);
                
                    // We can rotate the tile [0, 90, 180, 270] degrees to get more variation
                    GameObject go = Instantiate(currentPrefab, pos, Quaternion.identity, _transform);
                    var tile = go.GetComponentInChildren<GameTile>();
                    go.transform.SetParent(mapGraphicsParent);
                    tile.InitTileData(currentBiome, new Vector2Int(x, y));
                    MapTiles[x, y] = tile;
                }

                
                
            }

        }

    }


    void OnValidate()
    {
        _GenerateMapHeights();

    }



    public void AnimateGridSelection(int x, int y, float delay)
    {
        StartCoroutine(AnimateGridSelectionCoroutine(x, y, delay));
    }

    private IEnumerator AnimateGridSelectionCoroutine(int x, int y, float delay)
    {
        int rows = MapTiles.GetLength(0);
        int cols = MapTiles.GetLength(1);

        bool[,] visited = new bool[rows, cols];
        Queue<(int, int, int)> queue = new Queue<(int, int, int)>();
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        visited[x, y] = true;
        queue.Enqueue((x, y, 0));
        yield return null;
        while (queue.Count > 0)
        {
            (int cx, int cy, int distance) = queue.Dequeue();

            var tempIndex = new Vector2Int(cx, cy);
            BumpTiles(tempIndex);
            
            yield return new WaitForSeconds(delay * distance);
            
            for (int i = 0; i < 4; i++)
            {
                int nx = cx + dx[i];
                int ny = cy + dy[i];

                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny, distance + 1));
                }
            }
        }
    
        void BumpTiles(Vector2Int index)
        {
            MapTiles[index.x, index.y].gameObject.transform.DOPunchPosition((Vector3.up * 10), 0.4f, 1, 1, false);
        }
    }
     
    public void AnimateReturnToMap(int x, int y, float delay)
    {
        CachePositions();
        MoveTiles(-5);
        var cachedPos = cachedPositions[x,y];
        MapTiles[x,y].gameObject.transform.DOMove(cachedPos, 0.2f).SetEase(Ease.InBounce);
        StartCoroutine(AnimateReturnCoroutine(x, y, delay));
    }

    private IEnumerator AnimateReturnCoroutine(int x, int y, float delay)
    {
        int rows = MapTiles.GetLength(0);
        int cols = MapTiles.GetLength(1);

        bool[,] visited = new bool[rows, cols];
        Queue<(int, int, int)> queue = new Queue<(int, int, int)>();
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        visited[x, y] = true;
        queue.Enqueue((x, y, 0));

        while (queue.Count > 0)
        {
            (int cx, int cy, int distance) = queue.Dequeue();

            var tempIndex = new Vector2Int(cx, cy);
            ReturnTile(tempIndex);
            yield return new WaitForSeconds(delay * distance);

            for (int i = 0; i < 4; i++)
            {
                int nx = cx + dx[i];
                int ny = cy + dy[i];

                if (nx >= 0 && nx < rows && ny >= 0 && ny < cols && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    queue.Enqueue((nx, ny, distance + 1));
                }
            }
        }

        void ReturnTile(Vector2Int index)
        {
            var cachedPos = cachedPositions[index.x, index.y];
            MapTiles[index.x, index.y].gameObject.transform.DOMove(cachedPos, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private Vector3[,] cachedPositions;

    private void CachePositions()
    {
        int rows = MapTiles.GetLength(0);
        int cols = MapTiles.GetLength(1);

        cachedPositions = new Vector3[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                cachedPositions[i, j] = MapTiles[i, j].transform.position;
            }
        }
    }

    public void MoveTiles(float distance)
    {
        if (cachedPositions == null)
        {
            CachePositions();
        }

        int rows = cachedPositions.GetLength(0);
        int cols = cachedPositions.GetLength(1);


        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                Vector3 newPosition = cachedPositions[i, j] + new Vector3(0, distance, 0);
                MapTiles[i, j].transform.position = newPosition;
            }
        }
    }



    // private void OnDrawGizmos()
    // {
    //
    //     if (!debugMode) return;
    //     // Debug.Log("Drawing Gizmos");
    //     
    //     
    //     float xSize = tilePrefab.transform.localScale.x;
    //     float zSize = tilePrefab.transform.localScale.z;
    //     int xNum = widthHeight[0];
    //     int yNum = widthHeight[1];
    //     
    //     int xIndex = 0;
    //     int yIndex = 0;
    //     
    //     for (int x = 0; x < xNum; x++)
    //     {
    //         for (int y = 0; y < yNum; y++)
    //         {
    //             
    //             // if (Mathf.Abs(Vector2.Distance(Vector2.zero, new Vector2(x, y))) >= widthHeight[0] / 2f) continue;
    //             if (_mapHeights[xIndex, yIndex] < 1)
    //             {
    //                 yIndex++;
    //                 continue;
    //             }
    //
    //             bool edgeTile = _mapHeights[xIndex, yIndex] < 1;
    //
    //             
    //             float yScale = 1;
    //             if (edgeTile)
    //             {
    //                 yScale = Random.Range(1.2f, 2.1f);
    //             }
    //
    //             var scale = tilePrefab.transform.localScale;
    //             
    //             yScale = scale.y * yScale;
    //
    //             var localScale = tilePrefab.transform.localScale;;
    //             
    //             Vector3 pos = new Vector3(x * xSize, 0, y * zSize);
    //
    //             Gizmos.DrawCube(pos, new Vector3(localScale.x, yScale, localScale.z));
    //
    //             yIndex++;
    //         }
    //         xIndex++;
    //         yIndex = 0;
    //     }
    //
    // }
}



