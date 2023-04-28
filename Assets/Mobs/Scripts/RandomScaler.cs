using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScaler : MonoBehaviour
{
    [SerializeField][MinMaxSlider(0f, 2f)] private Vector2 m_heightRange = new Vector2(0.5f, 1.5f);
    [SerializeField][MinMaxSlider(0f, 2f)] private Vector2 m_widthRange = new Vector2(0.5f, 1.5f);

    private void Start()
    {
        var height = Random.Range(m_heightRange.x, m_heightRange.y);
        var width = Random.Range(m_widthRange.x, m_widthRange.y);
        transform.localScale = new Vector3(width, height, width);
    }
}
