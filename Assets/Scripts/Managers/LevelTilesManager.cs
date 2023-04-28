using UnityEngine;
using System.Collections.Generic;
using System;

public class LevelTilesManager : MonoBehaviour
{
    [Header("generation")]
    [SerializeField] GameObject defaultTilePrefab;
    [SerializeField] float spacing = 40f; //dependant on prefab
    [SerializeField] List<LevelTile> allGeneratedTiles = new List<LevelTile>();

    [Header("prefabs")]
    [SerializeField] GameObject[] grassBiomePrefabs;
    [SerializeField] GameObject[] forestBiomePrefabs;


    public Vector2Int playerSpawnCoords;

    private Dictionary<Vector2Int, LevelTile> gridTiles = new Dictionary<Vector2Int, LevelTile>(); // A dictionary to store the grid Tiles objects
   
    public static LevelTilesManager instance;


    // public GameObject GroundFxRTCam;
    int rows = 20; // The number of rows in the grid
    int columns = 20; // The number of columns in the grid


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
        }


    }
    private void Start()
    {
        if (BoardManager.MapTiles.Length == 0) print(" no map found");

        rows = BoardManager.MapTiles.GetLength(0);
        columns = BoardManager.MapTiles.GetLength(1);

        //wait for singleton. 
        //build tiles. 
        //build dictionary
        //sleep tiles

        //SleepAllTiles();
        //var startTile = GetTileAtGridPosition(playerSpawnCoords);
        //startTile.WakeUp();

    
    }
    //void BuildTileDictionary()
    //{
    //    gridTiles.Clear();
    //    foreach (var tile in allGeneratedTiles)
    //    {
    //        gridTiles.Add(tile.gridLocation, tile);
    //    }
    //}

    [ContextMenu("Generate Tiles")]
   public void GenerateTiles()   ///assuming we'll pre-generate tiles. if not this needs to be called in awake. still need to pass this the biome info etc
    {
        allGeneratedTiles.Clear();
        gridTiles.Clear();
        foreach (Transform child in transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }

        for (int xIndex = 0; xIndex < rows; xIndex++)
        {
            for (int yIndex = 0; yIndex < columns; yIndex++)
            {
                float xPos = xIndex * spacing;
                float yPos = yIndex * spacing;
                Vector3 position = new Vector3(xPos, 0.0f, yPos); //i know i know

                var mapTile = BoardManager.MapTiles[xIndex, yIndex];
                GameObject prefabToSpawn = GetPrefab(mapTile);

                GameObject spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
                spawnedObject.gameObject.name = "Tile " + xIndex + " _ " + yIndex;
                spawnedObject.transform.SetParent(transform);
                var tile = spawnedObject.GetComponent<LevelTile>();

                Vector2Int gridCoordinates = new Vector2Int(xIndex, yIndex);

                gridTiles.Add(gridCoordinates, tile);
                tile.Init(mapTile.mapTileData, gridCoordinates);
                tile.Sleep();
            }
        }
        //for (int columnIndex = 0; columnIndex < columns; columnIndex++)
        //{
        //    for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        //    {
        //        float xPos = columnIndex * spacing;
        //        float zPos = rowIndex * spacing;
        //        Vector3 position = new Vector3(xPos, 0.0f, zPos);

        //        var mapTile = BoardManager.MapTiles[rowIndex, columnIndex];
        //        GameObject prefabToSpawn = GetPrefab(mapTile);

        //        GameObject spawnedObject = Instantiate(prefabToSpawn, position, Quaternion.identity);
        //        spawnedObject.gameObject.name = "Tile " + rowIndex + " / " + columnIndex;
        //        spawnedObject.transform.SetParent(transform);
        //        var tile = spawnedObject.GetComponent<LevelTile>();

        //        Vector2Int gridCoordinates = new Vector2Int(columnIndex, rowIndex);

        //        gridTiles.Add(gridCoordinates, tile);
        //        tile.Init(mapTile.mapTileData, gridCoordinates);
        //        tile.Sleep();
        //    }
        //}

        foreach (LevelTile tile in gridTiles.Values)
        {
            allGeneratedTiles.Add(tile);
        }
    }

    private GameObject GetPrefab(GameTile mapTile)
    {
        var prefab = new GameObject();
        switch (mapTile.Biome)
        {
            case Biome.Grass:
                {
                    if (grassBiomePrefabs.Length > 0)
                    {
                        var randomIndex = UnityEngine.Random.Range(0, grassBiomePrefabs.Length);
                        prefab = grassBiomePrefabs[randomIndex];
                    }
                    else prefab = defaultTilePrefab;
                }
                break;
            case Biome.Desert:
                {
                    prefab = defaultTilePrefab;
                }
                break;
            case Biome.Mountain:
                {
                    prefab = defaultTilePrefab;
                }
                break;
            case Biome.Forest:
                {
                        if (forestBiomePrefabs.Length > 0)
                        {
                            var randomIndex = UnityEngine.Random.Range(0, forestBiomePrefabs.Length);
                            prefab = forestBiomePrefabs[randomIndex];
                        }
                      else prefab = defaultTilePrefab;
                    
                }
                break;
            case Biome.Water:
                {
                    prefab = defaultTilePrefab;
                }
                break;
            default:
                {
                    prefab = defaultTilePrefab;
                }
                break;
        }
        return prefab;
    }

    [ContextMenu("sleep all tiles")]
    public void SleepAllTiles()
    {
        foreach (LevelTile tile in gridTiles.Values)
        {
            tile.Sleep();
        }
    }

    internal void UpdateActiveTiles(LevelTile occupiedTile)
    { 
        var tilesToWake = new List<LevelTile>();
        tilesToWake = GetNeighboringTiles(occupiedTile.gridLocation);
        tilesToWake.Add(occupiedTile); // add the tile we're actually on
        foreach (LevelTile tile in tilesToWake) tile.WakeUp();

        foreach (LevelTile tile in gridTiles.Values)
        {
            if (!tilesToWake.Contains(tile))
            {
                tile.Sleep();
            }
        }
    }

    public LevelTile GetTileAtGridPosition(Vector2Int gridPosition)
    {
        if (gridTiles.ContainsKey(gridPosition))
        {
            return gridTiles[gridPosition];
        }
        else
        {
            Debug.LogError("No object exists at grid position " + gridPosition);
            return null;
        }
    }

    public List<LevelTile> GetNeighboringTiles(Vector2Int gridPosition)
    {
        List<LevelTile> neighboringTiles = new List<LevelTile>();

        // Check the tile to the right
        if (gridTiles.ContainsKey(gridPosition + Vector2Int.right))
        {
            neighboringTiles.Add(gridTiles[gridPosition + Vector2Int.right]);
        }

        // Check the tile to the left
        if (gridTiles.ContainsKey(gridPosition + Vector2Int.left))
        {
            neighboringTiles.Add(gridTiles[gridPosition + Vector2Int.left]);
        }

        // Check the tile above
        if (gridTiles.ContainsKey(gridPosition + Vector2Int.up))
        {
            neighboringTiles.Add(gridTiles[gridPosition + Vector2Int.up]);
        }

        // Check the tile below
        if (gridTiles.ContainsKey(gridPosition + Vector2Int.down))
        {
            neighboringTiles.Add(gridTiles[gridPosition + Vector2Int.down]);
        }

        // Check the tile to the top-right
        if (gridTiles.ContainsKey(gridPosition + new Vector2Int(1, 1)))
        {
            neighboringTiles.Add(gridTiles[gridPosition + new Vector2Int(1, 1)]);
        }

        // Check the tile to the top-left
        if (gridTiles.ContainsKey(gridPosition + new Vector2Int(-1, 1)))
        {
            neighboringTiles.Add(gridTiles[gridPosition + new Vector2Int(-1, 1)]);
        }

        // Check the tile to the bottom-right
        if (gridTiles.ContainsKey(gridPosition + new Vector2Int(1, -1)))
        {
            neighboringTiles.Add(gridTiles[gridPosition + new Vector2Int(1, -1)]);
        }

        // Check the tile to the bottom-left
        if (gridTiles.ContainsKey(gridPosition + new Vector2Int(-1, -1)))
        {
            neighboringTiles.Add(gridTiles[gridPosition + new Vector2Int(-1, -1)]);
        }

        return neighboringTiles;
    }


}



