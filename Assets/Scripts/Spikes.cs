using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour {

    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Transform m_TongueTopPivot;

    [Header("Attack")]
    [SerializeField] private float m_AttackVelocity = 50f;
    [SerializeField] private Transform m_AttackDestination;
    [SerializeField] private FeedbackOnTrigger m_FeedbackOnTrigger;

    [Header("Feedbacks")]
    [SerializeField] private MMF_Player m_AttackFB;

    private bool m_IsAttacking = false;

    void Update()
    {
        m_LineRenderer.SetPosition(1, m_TongueTopPivot.localPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        if (!m_IsAttacking)
        {
            m_FeedbackOnTrigger.IsAvailable = false;
            m_IsAttacking = true;
            Vector3 attackPos = collision.transform.position;
            attackPos.z = 1f;
            m_AttackDestination.position = attackPos;
            m_AttackFB.PlayFeedbacks();
            StartCoroutine(WaitEndAttack());
        }
    }

    private IEnumerator WaitEndAttack()
    {
        yield return new WaitUntil(() => !m_AttackFB.IsPlaying);
        m_IsAttacking = false;
        m_FeedbackOnTrigger.IsAvailable = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

    }
}
