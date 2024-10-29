using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private MMF_Player m_PickUpWeaponFB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player.Instance.GiveWeapon();
        m_PickUpWeaponFB?.PlayFeedbacks();
        GameObject.Destroy(gameObject);
    }
}
