using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TileSelector : MonoBehaviour
{
    
    
    Camera _camera;
    private void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        
        foreach (var tile in BoardManager.MapTiles)
        {
            tile.Unhighlight();
        }

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Tile"))) return;
        
        var index =  BoardManager.WorldPosToGrid(hit.point);
        var selectedTile = BoardManager.MapTiles[index[0], index[1]];
        if (selectedTile.IsObstacle) return;
        selectedTile.Highlight();

    }
}
