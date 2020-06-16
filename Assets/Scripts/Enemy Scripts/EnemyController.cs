using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState {
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyController : MonoBehaviour
{
    private EnemyAnimator enemyAnimator;
    private NavMeshAgent navAgent;
    private EnemyState enemyState;

    public float walkSpeed = 0.5f;
    public float runSpeed = 4f;
    public float chaseDistance = 7f;
    private float currentChaseDistance;
    public float attackDistance = 1.8f;
    public float chaseAfterAttackDistance = 2f;
    public float patrolForThisTime = 15f;
    public float patrolRadiusMin = 20f;
    public float patrolRadiusMax = 60f;
    private float patrolTimer;
    public float waitBeforeAttack = 2f;
    private float attackTimer;
    private Transform target;
    public GameObject attackPoint;
    private EnemyAudio enemyAudio;

    void Awake() {
        enemyAnimator = GetComponent<EnemyAnimator>();
        navAgent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;
        enemyAudio = GetComponentInChildren<EnemyAudio>();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyState = EnemyState.PATROL;
        patrolTimer = patrolForThisTime;
        attackTimer = waitBeforeAttack;
        currentChaseDistance = chaseDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.PATROL) {
            Patrol();
        }
        if (enemyState == EnemyState.CHASE) {
            Chase();
        }
        if (enemyState == EnemyState.ATTACK) {
            Attack();
        }
    }

    void Patrol() {
        navAgent.isStopped = false;
        navAgent.speed = walkSpeed;
        patrolTimer += Time.deltaTime;
        if (patrolTimer > patrolForThisTime) {
            SetNewRandomDestination();
            patrolTimer = 0;
        }

        if (navAgent.velocity.sqrMagnitude > 0) {
            enemyAnimator.Walk(true);
        } else {
            enemyAnimator.Walk(false);
        }

        if (Vector3.Distance(transform.position, target.position) <= chaseDistance) {
            enemyAnimator.Walk(false);
            enemyState = EnemyState.CHASE;
            // play spotted audio
            enemyAudio.PlayScreamSound();
        }
    }

    void Chase() {
        //navAgent.gameObject.transform.LookAt(target);
        navAgent.isStopped = false;
        navAgent.speed = runSpeed;
        navAgent.SetDestination(target.position);
        if (navAgent.velocity.sqrMagnitude > 0) {
            enemyAnimator.Run(true);
        } else {
            enemyAnimator.Run(false);
        }
        if (Vector3.Distance(transform.position, target.position) <= attackDistance) {
            // stop the animations
            enemyAnimator.Walk(false);
            enemyAnimator.Run(false);
            enemyState = EnemyState.ATTACK;
            if (chaseDistance != currentChaseDistance) {
                chaseDistance = currentChaseDistance;
            }
        } else if (Vector3.Distance(transform.position, target.position) > chaseDistance) {
            // player run away from enemy
            // stop running
            enemyAnimator.Run(false);
            enemyState = EnemyState.PATROL;
            patrolTimer = patrolForThisTime;
            if (chaseDistance != currentChaseDistance) {
                chaseDistance = currentChaseDistance;
            }
        }
    }

    void Attack() {
        //navAgent.gameObject.transform.LookAt(target);
        navAgent.velocity = Vector3.zero;
        navAgent.isStopped = true;
        attackTimer += Time.deltaTime;
        if (attackTimer > waitBeforeAttack) {
            enemyAnimator.Attack();
            attackTimer = 0f;
            // play attack sound
            enemyAudio.PlayAttackSound();
        }
        if (Vector3.Distance(transform.position, target.position) > attackDistance + chaseAfterAttackDistance) {
            enemyState = EnemyState.CHASE;
        }
    }

    void SetNewRandomDestination() {
        float randRadius = Random.Range(patrolRadiusMin, patrolRadiusMax);
        Vector3 randDir = Random.insideUnitSphere * randRadius;
        randDir += transform.position;
        NavMeshHit navMeshHit;
        NavMesh.SamplePosition(randDir, out navMeshHit, randRadius, -1);
        navAgent.SetDestination(navMeshHit.position);
    }

    void TurnOnAttackPoint() {
        attackPoint.SetActive(true);
    }

    void TurnOffAttackPoint() {
        if (attackPoint.activeInHierarchy) {
            attackPoint.SetActive(false);
        }
    }

    void OnAnimationDamage() {
        
    }

    public EnemyState EnemyState {
        get; set;
    }
}
