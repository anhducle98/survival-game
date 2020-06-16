using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAim {
    NONE,
    SELF_AIM,
    AIM
}

public enum WeaponFireType {
    SINGLE,
    MULTIPLE
}

public enum WeaponBulletType {
    BULLET,
    ARROW,
    SPEAR,
    NONE
}

public class WeaponHandler : MonoBehaviour
{
    private Animator animator;
    public WeaponAim weaponAim;
    
    [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField]
    private AudioSource shootSound, reloadSound;
    
    public WeaponFireType fireType;
    public WeaponBulletType bulletType;
    public GameObject attackPoint;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShootAnimation() {
        animator.SetTrigger(AnimationTags.SHOOT_TRIGGER);
    }

    public void Aim(bool canAim) {
        animator.SetBool(AnimationTags.AIM_PARAMETER, canAim);
    }

    void turnOnMuzzleFlash() {
        muzzleFlash.SetActive(true);
    }

    void turnOffMuzzleFlash() {
        muzzleFlash.SetActive(false);
    }

    void PlayShootSound() {
        shootSound.Play();
    }

    void PlayReloadSound() {
        reloadSound.Play();
    }

    void TurnOnAttackPoint() {
        attackPoint.SetActive(true);
    }

    void TurnOffAttackPoint() {
        if (attackPoint.activeInHierarchy) {
            attackPoint.SetActive(false);
        }
    }

}
