using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemies", order = 1)]

public class EnemyData : ScriptableObject
{
    public int hp, damage;

    public float speed;
}
