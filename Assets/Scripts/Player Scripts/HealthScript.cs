using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthScript : MonoBehaviour
{
    private EnemyAnimator enemyAnimator;
    private NavMeshAgent navMeshAgent;
    private EnemyController enemyController;
    public float health = 100f;
    public bool isPlayer, isBoar, isCannibal;
    private bool isDead; 
    private EnemyAudio enemyAudio;
    private PlayerStats playerStats;

    void Awake() {
        if (isBoar || isCannibal) {
            enemyAnimator = GetComponent<EnemyAnimator>();
            enemyController = GetComponent<EnemyController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            // get enemy audio
            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }
        if (isPlayer) {
            playerStats = GetComponent<PlayerStats>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage) {
        if (isDead) return;
        health -= damage;
        if (isPlayer) {
            // show the stats
            playerStats.DisplayHealthStats(health);
        }

        if (isBoar || isCannibal) {
            if (enemyController.EnemyState == EnemyState.PATROL) {
                enemyController.chaseDistance = 50f;
            }
        }

        if (health <= 0f) {
            PlayerDied();
            isDead = true;
        }
    }

    void PlayerDied() {
        if (isCannibal) {
            GetComponent<Animator>().enabled = false;
            GetComponent<BoxCollider>().isTrigger = false;
            GetComponent<Rigidbody>().AddTorque(-transform.forward * 50f);
            enemyController.enabled = false;
            navMeshAgent.enabled = false;
            enemyAnimator.enabled = false;

            // start coroutine
            StartCoroutine("DeadSound");
            // enemy manager spawn more enemies
            EnemyManager.instance.EnemyDied(true);
        }
        if (isBoar) {
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.isStopped = true;
            enemyController.enabled = false;
            enemyAnimator.Dead();

            // start coroutine
            StartCoroutine("DeadSound");
            // enemy manager spawn more enemies
            EnemyManager.instance.EnemyDied(false);
        }
        if (isPlayer) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(Tags.ENEMY_TAG);
            for (int i = 0; i < enemies.Length; ++i) {
                enemies[i].GetComponent<EnemyController>().enabled = false;
            }
            // call enemy manager to stop spawning enemies
            EnemyManager.instance.StopSpawning();

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<WeaponManager>().GetCurrentSelectedWeapon().gameObject.SetActive(false);
        }
        if (tag == Tags.PLAYER_TAG) {
            Invoke("RestartGame", 3f);
        } else {
            Invoke("TurnOffGameObject", 3);
        }
    }

    void RestartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    void TurnOffGameObject() {
        gameObject.SetActive(false);
    }

    IEnumerable DeadSound() {
        yield return new WaitForSeconds(0.3f);
        enemyAudio.PlayDeadSound();
    }
}
