using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _3C.Player;
using System;

public class LevelTile : MonoBehaviour
{
    
    LevelTilesManager worldTilesManager;
 
    public Transform teleportPoint;
    public bool hasPlayer;
    public bool tileActivated;
    WorldTileStatus status;
    public Vector2Int gridLocation;
    [SerializeField] RenderTexture groundFXPersistantRT;
    [SerializeField] GameObject RTCam;
    [SerializeField] GameObject environmentArt;

    private void Awake()
    {
        Sleep();
    }
    private void Start()
    {
        if (LevelTilesManager.instance != null) worldTilesManager = LevelTilesManager.instance;

    }
    public void ActivateGroundFX()
    {
        RTCam.SetActive(true);
    }
    public void DeactivateGroundFX()
    {
        RTCam.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out PlayerStateHandler player);
            if (player == null) return;
     
        hasPlayer = true;
        tileActivated = true;
        worldTilesManager.UpdateActiveTiles(this);
    }

    internal void WakeUp()
    {
        if (!tileActivated)
        {
            tileActivated = true;
            if (RTCam != null)  RTCam.SetActive(true);
            environmentArt.SetActive(true);
        }
    }
    internal void Sleep()
    {
            tileActivated = false;
            if (RTCam != null) RTCam.SetActive(false);
            environmentArt.SetActive(false);
            hasPlayer = false;
           //tell mobs to go home or whatever
      
    }

    internal void Init(MapTileData mapTileData, Vector2Int gridCoordinates)
    {
        status = mapTileData.tileStatus;
        gridLocation = gridCoordinates;
    }
}

public enum WorldTileStatus
{
    neutral,
    contested,
    burnt,
    frozen
}
