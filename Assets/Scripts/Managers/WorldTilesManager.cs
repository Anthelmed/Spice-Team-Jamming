using UnityEngine;
using System.Collections.Generic;
using System;

public class WorldTilesManager : MonoBehaviour
{
    [Header("generate tiles using context menu")]
    public GameObject tilePrefab; // The prefab object to spawn
    public int rows = 20; // The number of rows in the grid
    public int columns = 20; // The number of columns in the grid
    public float spacing = 40f; // The spacing between objects in the grid
    public Vector2Int playerSpawnCoords;

    private Dictionary<Vector2Int, WorldTile> gridTiles = new Dictionary<Vector2Int, WorldTile>(); // A dictionary to store the grid Tiles objects
   
    public static WorldTilesManager instance;

    [SerializeField] List<WorldTile> allGeneratedTiles = new List<WorldTile>();
   // public GameObject GroundFxRTCam;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
        }

        if (allGeneratedTiles.Count > 0)
        {
            BuildTileDictionary();
        }
        else print("no generated tiles found");
    }

     void BuildTileDictionary()
    {
        gridTiles.Clear();
        foreach (var tile in allGeneratedTiles)
        {
            gridTiles.Add(tile.gridLocation, tile);
        }
    }
    private void Start()
    {
        SleepAllTiles();
        var startTile = GetTileAtGridPosition(playerSpawnCoords);
        startTile.WakeUp();
    }
    [ContextMenu("Generate Tiles")]
    void GenerateTiles()   ///assuming we'll pre-generate tiles. if not this needs to be called in awake. still need to pass this the biome info etc
    {
        allGeneratedTiles.Clear();
        gridTiles.Clear();
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int rowIndex = 0; rowIndex < rows; rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < columns; columnIndex++)
            {
                float xPos = columnIndex * spacing;
                float zPos = rowIndex * spacing;
                Vector3 position = new Vector3(xPos, 0.0f, zPos);

                GameObject spawnedObject = Instantiate(tilePrefab, position, Quaternion.identity);
                spawnedObject.gameObject.name = "Tile " + columnIndex + " / " + rowIndex;
                spawnedObject.transform.SetParent(transform);
                var tile = spawnedObject.GetComponent<WorldTile>();
                
                Vector2Int gridCoordinates = new Vector2Int(columnIndex, rowIndex);
                tile.gridLocation = gridCoordinates;
                gridTiles.Add(gridCoordinates, tile);
                tile.Sleep();
            }
        }

        foreach (WorldTile tile in gridTiles.Values)
        {
            allGeneratedTiles.Add(tile);
        }
    }
    [ContextMenu("Deactivate all tiles")]
    public void SleepAllTiles()
    {
        foreach (WorldTile tile in gridTiles.Values)
        {
            tile.Sleep();
        }
    }

    internal void UpdateActiveTiles(WorldTile occupiedTile)
    { 
        var tilesToWake = new List<WorldTile>();
        tilesToWake = GetNeighboringTiles(occupiedTile.gridLocation);
        tilesToWake.Add(occupiedTile); // add the tile we're actually on
        foreach (WorldTile tile in tilesToWake) tile.WakeUp();

        foreach (WorldTile tile in gridTiles.Values)
        {
            if (!tilesToWake.Contains(tile))
            {
                tile.Sleep();
            }
        }
    }

    public WorldTile GetTileAtGridPosition(Vector2Int gridPosition)
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

    public List<WorldTile> GetNeighboringTiles(Vector2Int gridPosition)
    {
        List<WorldTile> neighboringTiles = new List<WorldTile>();

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



