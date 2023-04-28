using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _3C.Player;
using System;

public class WorldTile : MonoBehaviour
{
    
    WorldTilesManager worldTilesManager;
 
    public Transform teleportPoint;
    public bool occupied;
    public bool tileActivated;
    public WorldTileStatus status;
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
        if (WorldTilesManager.instance != null) worldTilesManager = WorldTilesManager.instance;

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
     
        occupied = true;
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
            occupied = false;
           //tell mobs to go home or whatever
      
    }

}

public enum WorldTileStatus
{
    red,
    blue, 
    neutral
}
