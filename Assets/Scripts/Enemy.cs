using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    public float m_moveSpeed = (0.05f * 60.0f);
    public float m_changeSpeed = 0.2f * 60.0f;
    public float m_moveDuration = 3.0f;
    public float m_holdDuration = 0.5f;
    public float m_chargeCooldownDuration = 2.0f;
    public float m_chargeMinRange = 1.0f;
    public float m_maxHealth = 4.0f;
    public float m_MinDistanceToExplode = 2.0f;

    public Player m_player = null;

    public int m_NumBulletsPerShot = 6;
    public float m_InvulnerableWaitTime = 0.7f;

    [SerializeField] private MMF_Player m_StunDieFB;
    [SerializeField] private MMF_Player m_StunAfterDieFB;
    [SerializeField] private MMF_Player m_AlertFB;
    [SerializeField] private MMF_Player m_ChaseFB;
    [SerializeField] private MMF_Player m_StopChasingFB;
    [SerializeField] private MMF_Player m_ShootFB;
    [SerializeField] private MMF_Player m_ExplodeFB;
    [SerializeField] private MMF_Player m_InvulnerableFB;
    [SerializeField] private MMF_Player m_InvulnerableReflectFB;
    [SerializeField] private MMF_Player m_DieWithBulletFB;
    [SerializeField] private Transform m_BulletReflectPivot;
    [SerializeField] private float m_BulletReflectOffset;

    private Rigidbody2D m_rigidBody = null;
    private BoxCollider2D m_Collider = null;
    private float m_health = 100.0f;
    private float m_timer = 0.0f;
    private float m_lastPlayerDiff = 0.0f;
    private Vector2 m_vel = new Vector2(0, 0);

    private bool m_PlayerIsIndideExplosion = false;
    private PlayerHealth m_playerHealth = null;

    private bool m_Stunned = false;
    private bool m_InvulnerableWaiting = false;
    private bool m_Dead = false;

    private enum WallCollision
    {
        None = 0,
        Left,
        Right
    };
    private WallCollision m_wallFlags = WallCollision.None;

    private enum State
    {
        Idle = 0,
        Walking,
        Charging,
        ChargingCooldown,
    };
    private State m_state = State.Idle;

    private void Awake()
    {
        m_playerHealth = m_player.GetComponent<PlayerHealth>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_health = m_maxHealth;
        m_rigidBody = transform.GetComponent<Rigidbody2D>();
        m_Collider = transform.GetComponent<BoxCollider2D>();
    }

    public void Stun()
    {
        StartCoroutine(Stun_Internal());
    }

    private IEnumerator Stun_Internal()
    {
        if (!m_Stunned)
        {
            m_Collider.size = new Vector2(m_Collider.size.x, 1.5f);
            m_StunDieFB?.PlayFeedbacks();
            m_rigidBody.bodyType = RigidbodyType2D.Static;
        }
        else
            m_StunAfterDieFB?.PlayFeedbacks();

        m_Stunned = true;
        this.enabled = false;

        yield return new WaitUntil(() => !m_StunDieFB.IsPlaying);


        //m_Stunned = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Stunned)
            return;

        if (m_InvulnerableWaiting)
            return;

        switch (m_state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Walking:
                Walking();
                break;
            case State.Charging:
                Charging();
                break;
            case State.ChargingCooldown:
                ChargingCooldown();
                break;
            default:
                break;
        }

        m_wallFlags = WallCollision.None;
    }

    public bool InflictDamage(float damageAmount, Vector2 reflectPos)
    {
        if (m_Dead == true)
            return false;

        if (m_Stunned)
            return false;

        if (m_InvulnerableFB.IsPlaying)
        {
            Vector3 reflectDirection = new Vector3 (reflectPos.x, reflectPos.y, transform.position.z) - transform.position;
            Vector3 reflectRealPos = new Vector3(reflectPos.x, reflectPos.y, transform.position.z);
            m_BulletReflectPivot.position = reflectRealPos + reflectDirection.normalized * m_BulletReflectOffset;
            m_InvulnerableReflectFB.PlayFeedbacks();

            reflectDirection.y = 0f;

            GameObject projectileGO = ObjectPooler.Instance.GetObject("BulletReflectEnemy");
            if (projectileGO)
            {
                projectileGO.GetComponent<Bullet>().Fire(reflectRealPos, reflectDirection.normalized);
            }
            return false;
        }

        StartCoroutine(InflictDamage_Internal(damageAmount));
        return true;
    }

    private IEnumerator InflictDamage_Internal(float damageAmount)
    {
        m_health -= damageAmount;

        m_InvulnerableWaiting = true;
        m_StopChasingFB.PlayFeedbacks();

        if (m_health > 0.0f)
        {
            m_InvulnerableFB?.PlayFeedbacks();

            yield return new WaitForSeconds(m_InvulnerableWaitTime);
        }
        else
        {
            m_Dead = true;

            m_DieWithBulletFB.PlayFeedbacks();

            yield return new WaitUntil(() => !m_DieWithBulletFB.IsPlaying);
            ShootBullets();
            {
                GameObject.Destroy(gameObject);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (m_state == State.Charging)
            m_ChaseFB.PlayFeedbacks();

        m_InvulnerableWaiting = false;
    }

    private void ShootBullets()
    {
        //Fire
        m_ShootFB?.PlayFeedbacks();
       
        float angleStep = 360f / m_NumBulletsPerShot;

        for (int i = 0; i < m_NumBulletsPerShot; i++)
        {
            float angle = i * angleStep;

            float angleRad = angle * Mathf.Deg2Rad;

            Vector3 bulletDirection = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);

            GameObject projectileGO = ObjectPooler.Instance.GetObject("BulletEnemy");
            if (projectileGO)
            {
                projectileGO.GetComponent<Bullet>().Fire(transform.position, bulletDirection.normalized);
            }
        }
    }

    void Idle()
    {
        m_vel = Vector2.zero;

        float yDiff = m_player.transform.position.y - transform.position.y;
        if(Mathf.Abs(yDiff) <= m_chargeMinRange)
        {
            //Charge at the player!
            m_lastPlayerDiff = m_player.transform.position.x - transform.position.x;
            m_vel.x = m_changeSpeed * Mathf.Sign(m_lastPlayerDiff);
            m_timer = 0;
            ChangeState(State.Charging);
            return;
        }

        m_timer += Time.deltaTime;
        if(m_timer >= m_holdDuration)
        {
            m_timer = 0;
            ChangeState(State.Walking);

            if(m_wallFlags == WallCollision.None)
            {
                //Randomly choose.
                m_vel.x = (Random.Range(0.0f, 100.0f) < 50.0f) ? m_moveSpeed : -m_moveSpeed;
            }
            else
            {
                m_vel.x = (m_wallFlags == WallCollision.Left) ? m_moveSpeed : -m_moveSpeed;
            }
            return;
        }
    }

    void Walking()
    {
        ApplyVelocity();

        float yDiff = m_player.transform.position.y - transform.position.y;
        if (Mathf.Abs(yDiff) <= m_chargeMinRange)
        {
            //Charge at the player!
            m_lastPlayerDiff = m_player.transform.position.x - transform.position.x;
            m_vel.x = m_changeSpeed * Mathf.Sign(m_lastPlayerDiff);
            m_timer = 0;
            ChangeState(State.Charging);
            return;
        }

        m_timer += Time.deltaTime;
        if (m_timer >= m_moveDuration)
        {
            //No longer on the ground, fall.
            m_timer = 0.0f;
            ChangeState(State.Idle);
            return;
        }
    }

    void Charging()
    {
        if (m_AlertFB.IsPlaying)
            return;

        //Charge towards player until you pass it's x position.
        ApplyVelocity();

        float xDiff = Mathf.Abs(m_player.transform.position.x - transform.position.x);

        if (xDiff <= m_MinDistanceToExplode)
        {
            //Charge at the player!
            m_vel.x = 0.0f;
            m_timer = 0;
            ChangeState(State.ChargingCooldown);
            return;
        }
    }

    void ChargingCooldown()
    {
        if (!m_ExplodeFB.IsPlaying)
        {
            if (m_PlayerIsIndideExplosion)
                m_playerHealth.DoDamage();
            //No longer on the ground, fall.
            m_timer = 0.0f;
            ChangeState(State.Idle);
            return;
        }
    }

    private void ChangeState(State state)
    {
        State previousState = m_state;
        switch (state)
        {
            case (State.Idle):
                m_state = State.Idle;
                if (previousState == State.Charging || previousState == State.ChargingCooldown)
                    m_StopChasingFB?.PlayFeedbacks();
                break;
            case (State.Walking):
                m_state = State.Walking;
                break;
            case (State.Charging):
                m_AlertFB?.PlayFeedbacks();
                m_state = State.Charging;
                break;
            case (State.ChargingCooldown):
                m_ExplodeFB?.PlayFeedbacks();
                m_state = State.ChargingCooldown;
                break;
        }
    }

    public void SetPlayerIsInsideExplosion(bool isInside)
    {
        m_PlayerIsIndideExplosion = isInside;
    }

    void ApplyVelocity()
    {
        Vector3 pos = m_rigidBody.transform.position;
        pos.x += m_vel.x * Time.fixedDeltaTime;
        pos.y += m_vel.y * Time.fixedDeltaTime;
        m_rigidBody.transform.position = pos;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
    }

    private void ProcessCollision(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
            return;

        Vector3 pos = m_rigidBody.transform.position;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Push back out
            Vector2 impulse = contact.normal * (contact.normalImpulse / Time.fixedDeltaTime);
            pos.x += impulse.x;
            pos.y += impulse.y;

            if (Mathf.Abs(contact.normal.y) < Mathf.Abs(contact.normal.x))
            {
                if ((contact.normal.x > 0 && m_vel.x < 0) || (contact.normal.x < 0 && m_vel.x > 0))
                {
                    m_vel.x = 0;
                    //Stop us.
                    m_wallFlags = (contact.normal.x < 0) ? WallCollision.Left : WallCollision.Right;

                    ChangeState(State.Idle);
                    m_timer = 0;
                }
            }
        }
        m_rigidBody.transform.position = pos;
    }
}
