using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_moveSpeed = (2.0f * 60.0f);
    public float m_hitDamage = 1.0f;
    public string m_TargetTag = "Enemy";
    public string m_SelfTag = "Player";

    [SerializeField] private MMF_Player m_ShootFB;
    [SerializeField] private MMF_Player m_ImpactFB;

    private Rigidbody2D m_rigidBody = null;
    private Vector2 m_vel = new Vector2(0, 0);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    // Start is called before the first frame update
    void Start()
    {
        m_rigidBody = transform.GetComponent<Rigidbody2D>();
        
        foreach (var feedback in m_ImpactFB.GetFeedbacksOfType<MMF_InstantiateObject>())
            feedback.ParentTransform = transform.root;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = m_rigidBody.transform.position;
        pos.x += m_vel.x * Time.fixedDeltaTime * m_moveSpeed;
        pos.y += m_vel.y * Time.fixedDeltaTime * m_moveSpeed;
        m_rigidBody.transform.position = pos;
    }

    public void Fire(Vector3 startPos, Vector3 direction)
    {
        gameObject.SetActive(true);
        transform.position = startPos;
        m_vel.x = direction.x;
        m_vel.y = direction.y;

        StartCoroutine(Fire_Internal());
    }

    private IEnumerator Fire_Internal()
    {
        yield return null;
        m_ShootFB?.PlayFeedbacks();
    }

    private void ProcessCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(m_SelfTag))
            return;

        if (m_rigidBody)
        {

            Vector3 pos = m_rigidBody.transform.position;
            bool doImpactFB = true;
            if(collision.gameObject.CompareTag(m_TargetTag))
            {
                //Do damage
                if (collision.gameObject.TryGetComponent(out Enemy enemyObject))
                    doImpactFB = enemyObject.InflictDamage(m_hitDamage, collision.GetContact(0).point);
                else if (collision.gameObject.TryGetComponent(out PlayerHealth player))
                    player.DoDamage();
            }

            if (doImpactFB)
                m_ImpactFB?.PlayFeedbacks();

            foreach (ContactPoint2D contact in collision.contacts)
            {
                //Push back out
                Vector2 impulse = contact.normal * (contact.normalImpulse / Time.fixedDeltaTime);
                pos.x += impulse.x;
                pos.y += impulse.y;

                //Is this a wall, or an enemy?
                ObjectPooler.Instance.ReturnObject(gameObject);
            }
            m_rigidBody.transform.position = pos;
        }
    }
}
