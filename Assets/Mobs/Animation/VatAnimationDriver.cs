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
    private Vector4 m_previousData;

    private readonly int ANIM_DATA = Shader.PropertyToID("_AnimData");
    private readonly int PREV_ANIM_DATA = Shader.PropertyToID("_PrevAnimData");

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
        var data = animationData.animations[desiredAnimation];
        data.w = Time.timeSinceLevelLoad;
        m_mpb.SetVector(ANIM_DATA, data);
        m_mpb.SetVector(PREV_ANIM_DATA, m_previousData);
        m_previousData = data;
        m_renderer.SetPropertyBlock(m_mpb);
    }
}
