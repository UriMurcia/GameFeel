using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyes : MonoBehaviour
{
    [SerializeField] private float m_MaxRadius = 1f;
    [SerializeField] private float m_Velocity = 10f;

    [SerializeField] private Transform m_Player;

    private Vector3 m_InitialPos;

    private void Start()
    {
        m_InitialPos = transform.position;
    }

    private void Update()
    {
        Vector3 direction = m_Player.position - transform.position;
        direction.Normalize();

        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, -1f);
        Vector3 newPos = m_InitialPos + direction * m_MaxRadius;

        transform.position = new Vector3(newPos.x, newPos.y, -1f);
    }
}
