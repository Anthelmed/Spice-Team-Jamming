using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class VatAnimationDriver : MonoBehaviour
{
    public VatData animationData;
    public int desiredAnimation = 0;

    [SerializeField] private MeshRenderer m_renderer;

    private MaterialPropertyBlock m_mpb;
    private int m_currentAnimation = -1;

    private readonly int ANIM_DATA = Shader.PropertyToID("_AnimData");

    private void OnValidate()
    {
        m_renderer = GetComponent<MeshRenderer>();
    }

    private void Reset()
    {
        OnValidate();
    }

    private void Start()
    {
        m_mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (!animationData || m_currentAnimation == desiredAnimation)
            return;

        m_currentAnimation = desiredAnimation;
        m_mpb.SetVector(ANIM_DATA, animationData.animations[desiredAnimation]);
        m_renderer.SetPropertyBlock(m_mpb);
    }
}
