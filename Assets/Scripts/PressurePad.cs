using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePad : MonoBehaviour 
{
    [SerializeField] Player m_Player;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player m_InteractFB;

    private PlayerHealth m_PlayerHealth;

    private void Awake()
    {
        m_PlayerHealth = m_Player.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            m_InteractFB?.PlayFeedbacks();
            m_Player.JumpPressurePad();
            m_PlayerHealth.RestoreLifePoints();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

    }
}
