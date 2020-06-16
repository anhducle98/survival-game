using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weaponManager;
    public float fireRate = 15f;
    private float nextTimeToFire;
    public float damage = 20f;

    private Animator zoomCameraAnim;
    private bool zommed;
    private Camera mainCam;
    private GameObject crosshair;
    private bool isAiming;

    [SerializeField]
    private GameObject arrowPrefab, spearPrefab;
    [SerializeField]
    private Transform arrowBowStartPosition;

    void Awake() {
        weaponManager = GetComponent<WeaponManager>();
        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT).transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();
        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);
        mainCam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        WeaponShoot();
        ZoomInAndOut();
    }

    void WeaponShoot() {
        if (weaponManager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE) {
            // assault rifle
            if (Input.GetMouseButton(0) && Time.time > nextTimeToFire) {
                nextTimeToFire = Time.time + 1f / fireRate;
                weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                BulletFired();
            }
        } else {
            if (Input.GetMouseButtonDown(0)) {
                if (weaponManager.GetCurrentSelectedWeapon().tag == Tags.AXE_TAG) {
                    weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                } else if (weaponManager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.BULLET) {
                    weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                    BulletFired();
                } else {
                    // arrow or spear
                    if (isAiming) {
                        weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                        if (weaponManager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.ARROW) {
                            // throw arrow
                            ThrowArrowOrSpear(true);
                        } else if (weaponManager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.SPEAR) {
                            // throw spear
                            ThrowArrowOrSpear(false);
                        }
                    }
                }
            }
        }
    }

    void ZoomInAndOut() {
        if (weaponManager.GetCurrentSelectedWeapon().weaponAim == WeaponAim.AIM) {
            if (Input.GetMouseButtonDown(1)) {
                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM);
                crosshair.SetActive(false);
            }
            if (Input.GetMouseButtonUp(1)) {
                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);
                crosshair.SetActive(true);
            }
        }
        if (weaponManager.GetCurrentSelectedWeapon().weaponAim == WeaponAim.SELF_AIM) {
            if (Input.GetMouseButtonDown(1)) {
                weaponManager.GetCurrentSelectedWeapon().Aim(true);
                isAiming = true;
            }
            if (Input.GetMouseButtonUp(1)) {
                weaponManager.GetCurrentSelectedWeapon().Aim(false);
                isAiming = false;
            }
        }
    }

    void ThrowArrowOrSpear(bool throwArrow) {
        if (throwArrow) {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = arrowBowStartPosition.position;
            arrow.GetComponent<ArrowBowScript>().Launch(mainCam);
        } else {
            GameObject spear = Instantiate(spearPrefab);
            spear.transform.position = arrowBowStartPosition.position;
            spear.GetComponent<ArrowBowScript>().Launch(mainCam);
        }
    }

    void BulletFired() {
        RaycastHit hit;
        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit)) {
            print("We hit: " + hit.transform.gameObject.name);
            if (hit.transform.tag == Tags.ENEMY_TAG) {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(damage);
            }
        }
    }
}
