using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] private MMF_Player m_PickUpWeaponFB;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_PickUpWeaponFB?.PlayFeedbacks();
        Player.Instance.GiveWeapon(m_PickUpWeaponFB);
        StartCoroutine(DestroyWeapon());
    }

    private IEnumerator DestroyWeapon()
    {
        yield return new WaitUntil(() => !m_PickUpWeaponFB.IsPlaying);

        GameObject.Destroy(transform.parent.gameObject);
    }
}
