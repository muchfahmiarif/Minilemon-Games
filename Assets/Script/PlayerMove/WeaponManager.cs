using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public GameObject playerCam;
    public float range = 100f;
    public float damage = 25f;
    public Animator playerAnimator;
    public ParticleSystem muzzleFlash;
    public GameObject hitParticles;

    public AudioClip gunShot;
    public AudioSource audioSource;

    //Get audio file
    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    //Get Input and delay between shots
    void Update() {
        if (playerAnimator.GetBool("isShooting")) {
            playerAnimator.SetBool("isShooting", false);
        }

        if (Input.GetButtonDown("Fire1")) {
            Shoot();

        }
    }

    //Play audio and animation when weapon is fired
    void Shoot() {
        muzzleFlash.Play();
        audioSource.PlayOneShot(gunShot);
        playerAnimator.SetBool("isShooting", true);

        RaycastHit hit;

        if(Physics.Raycast(playerCam.transform.position, transform.forward, out hit, range)){
            //Debug.Log("hit");
            EnemyManager enemyManager = hit.transform.GetComponent<EnemyManager>();
            if(enemyManager != null) {
                enemyManager.Hit(damage);
                GameObject instParticles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                instParticles.transform.parent = hit.transform;
                Destroy(instParticles, 2f);
            }
        }

    }
}
