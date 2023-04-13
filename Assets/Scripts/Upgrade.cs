using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class Upgrade
{
    public string name;
    public int levelAmount;
    public bool unlimited;
    public bool maxed;
    public float scaleAmount;
    public string desc;
    public Sprite image;

    [System.Serializable]
    public class LevelUpEvent : UnityEvent<float> {}

    [SerializeField]
    LevelUpEvent onLevel;

    public void PerfomUpgrade() {
        onLevel.Invoke(scaleAmount);

        if(!unlimited) {
            levelAmount--;
            if(levelAmount <= 0)
                maxed = true;
        }
    }
}
