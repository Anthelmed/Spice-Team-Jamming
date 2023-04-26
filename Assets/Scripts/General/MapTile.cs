using System;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    // [SerializeField] private Transform spawnPoint;
    // [SerializeField] private GameObject highlight;
    // [SerializeField] private GameObject spawnVFX;
    
    [SerializeField] private Biome biome;
    public Biome Biome => biome;

    private Renderer _renderer;
    public Renderer Renderer => _renderer;

    private bool _isObstacle = false;
    private bool _isSpawnArea = false;
    private bool _isOccupied = false;
    
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
    }

    public bool IsSpawnArea
    {
        get => _isSpawnArea;
        set => _isSpawnArea = value;
    }

    // public Transform SpawnPoint => spawnPoint;
    
    private bool isHighlighted = false;

    public void Highlight()
    {
        if (!_isObstacle)
            isHighlighted = true;
    }

    public void Unhighlight()
    {
        isHighlighted = false;
    }

    // private void OnDrawGizmos()
    // {
    //     if (isHighlighted)
    //         Gizmos.DrawCube(spawnPoint.transform.position, Vector3.one * 0.1f);
    // }
    //
    
}