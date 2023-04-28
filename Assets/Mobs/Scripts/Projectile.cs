using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 target;
    public float height = 1f;
    public float duration = 1f;

    private Vector3 m_startPosition;
    private Vector3 m_direction;
    private float m_a;
    private float m_b;
    private float m_distance;
    private float m_startTime;

    private void OnEnable()
    {
        m_startTime = Time.timeSinceLevelLoad;
        m_startPosition = transform.position;

        var flatTarget = target;
        flatTarget.y = m_startPosition.y;

        m_direction = flatTarget - m_startPosition;
        m_distance = m_direction.magnitude;
        m_direction /= m_distance;

        m_a = height / m_distance;
        m_b = m_distance * m_a;
    }

    private void LateUpdate()
    {
        var time = Time.timeSinceLevelLoad - m_startTime;
        if (time > duration)
        {
            gameObject.SetActive(false);
            return;
        }

        var x = m_distance * time / duration;
        transform.position = m_startPosition + m_direction * x + (-m_a * x * x + m_b * x) * Vector3.up;
    }
}
