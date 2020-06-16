using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    [SerializeField]
    private GameObject boarPrefab, cannibalPrefab;

    public Transform[] cannibalSpawnPoints;
    public Transform[] boarSpawnPoints;

    [SerializeField]
    private int cannibalEnemyCount, boarEnemyCount;
    private int initialCannibalCount, initialBoarCount;
    public float waitBeforeSpawnEnemiesTime = 10f;

    void Awake() {
        MakeInstance();
    }

    void MakeInstance() {
        if (instance == null) {
            instance = this;
        }
    }

    void Start() {
        initialCannibalCount = cannibalEnemyCount;
        initialBoarCount = boarEnemyCount;
        SpawnEnemies();
        StartCoroutine("CheckToSpawnEnemies");
    }

    void SpawnEnemies() {
        SpawnCannibals();
        SpawnBoars();
    }

    void SpawnCannibals() {
        for (int i = 0, index = 0; i < cannibalEnemyCount; ++i, ++index) {
            if (index >= cannibalSpawnPoints.Length) index = 0;
            Instantiate(cannibalPrefab, cannibalSpawnPoints[index].position, Quaternion.identity);
        }
        cannibalEnemyCount = 0;
    }

    void SpawnBoars() {
        for (int i = 0, index = 0; i < boarEnemyCount; ++i, ++index) {
            if (index >= boarSpawnPoints.Length) index = 0;
            Instantiate(boarPrefab, boarSpawnPoints[index].position, Quaternion.identity);
        }
        boarEnemyCount = 0;
    }

    public void EnemyDied(bool cannibal) {
        if (cannibal) {
            cannibalEnemyCount++;
            if (cannibalEnemyCount > initialCannibalCount) {
                cannibalEnemyCount = initialCannibalCount;
            }
        } else {
            boarEnemyCount++;
            if (boarEnemyCount > initialBoarCount) {
                boarEnemyCount = initialBoarCount;
            }
        }
    }

    IEnumerator CheckToSpawnEnemies() {
        yield return new WaitForSeconds(waitBeforeSpawnEnemiesTime);
        SpawnCannibals();
        SpawnBoars();
        StartCoroutine("CheckToSpawnEnemies");
    }

    public void StopSpawning() {
        StopCoroutine("CheckToSpawnEnemies");
    }
}
