using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackOnTrigger : MonoBehaviour
{
    [SerializeField] private MMF_Player[] m_EnterTriggerFB;
    [SerializeField] private MMF_Player[] m_ExitTriggerFB;

    public bool IsAvailable = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsAvailable)
            return;

        if (!collision.CompareTag("Player"))
            return;

        foreach (var feedback in m_EnterTriggerFB)
            feedback?.PlayFeedbacks();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsAvailable)
            return;

        if (!collision.CompareTag("Player"))
            return;

        foreach (var feedback in m_ExitTriggerFB)
            feedback?.PlayFeedbacks();
    }
}
