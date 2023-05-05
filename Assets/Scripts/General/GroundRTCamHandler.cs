using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRTCamHandler : MonoBehaviour
{
    [SerializeField] Transform vCamTransform;
    [SerializeField] RenderTexture groundFXtex;

    Vector2Int resolution;

    private void Awake()
    {
        resolution = new Vector2Int(Screen.width, Screen.height);
        ResizeRT(groundFXtex, resolution.x, resolution.y);
    }

    private void Update()
    {
        CheckResolution();
    }

    private void CheckResolution()
    {
        
        if (groundFXtex.width != Screen.width || groundFXtex.height != Screen.height)
        {
           
            resolution.x = Screen.width;
            resolution.y = Screen.height;

            ResizeRT(groundFXtex, resolution.x, resolution.y);
        }
    }

    void ResizeRT(RenderTexture renderTexture, int width, int height)
    {
        if (renderTexture)
        {
            print("resizing rt");
            renderTexture.Release();
            renderTexture.width = width;
            renderTexture.height = height;
        }
    }
}
