using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Weapon
{

    //Fires the weapon with the given direction
    public void shoot(Vector2 direction);

    // Updates a specified weapon attribute based on the given level
}

