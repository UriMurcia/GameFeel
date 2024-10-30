using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private MMF_Player m_PlayerReceiveDamageFB;
    [SerializeField] private MMF_Player m_PlayerInvulnerableFB;
    [SerializeField] private MMF_Player m_ReceiveDamage01FB;
    [SerializeField] private MMF_Player m_ReceiveDamage02FB;
    [SerializeField] private MMF_Player m_ReceiveDamage03FB;
    [SerializeField] private MMF_Player m_DieFB;
    [SerializeField] private MMF_Player m_RestoreLifePointsFB;

    private Player m_Player;

    private int m_MaxHealth = 4;
    private int m_CurrentHealth;
    private bool m_IsInvulnerable = false;
    private bool m_IsDead = false;

    private void Awake()
    {
        m_Player = GetComponent<Player>();
        m_CurrentHealth = m_MaxHealth;
    }

    public void DoDamage()
    {
        if (m_IsDead)
            return;

        if (m_IsInvulnerable)
            return;

        m_CurrentHealth--;

        if (m_CurrentHealth == 3)
            m_ReceiveDamage01FB?.PlayFeedbacks();
        else if (m_CurrentHealth == 2)
            m_ReceiveDamage02FB?.PlayFeedbacks();
        else if (m_CurrentHealth == 1)
            m_ReceiveDamage03FB?.PlayFeedbacks();
        else if (m_CurrentHealth == 0)
        {
            m_IsDead = true;
            m_DieFB?.PlayFeedbacks();
            return;
        }

        m_PlayerReceiveDamageFB?.PlayFeedbacks();
        StartCoroutine(SetInvulnerable());
    }

    private IEnumerator SetInvulnerable()
    {
        m_IsInvulnerable = true;
        m_PlayerInvulnerableFB?.PlayFeedbacks();

        yield return new WaitUntil(() => !m_PlayerInvulnerableFB.IsPlaying);

        m_IsInvulnerable = false;
    }

    public void RestoreLifePoints()
    {
        if (m_CurrentHealth >= m_MaxHealth)
            return;

        m_RestoreLifePointsFB?.PlayFeedbacks();
        m_CurrentHealth = m_MaxHealth;
    }

    public void Die()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}
