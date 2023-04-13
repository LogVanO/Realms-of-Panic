using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "ScriptableObjects/EnemyWave", order = 1)]

public class EnemyWave : ScriptableObject
{
    public GameObject[] enemies;

    //enemies per second
    public float spawnRate;

}
