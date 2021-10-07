using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponScriptableObject : ScriptableObject
{
    [NonReorderable]
    public List<Weapon> weapons_all = new List<Weapon>();  
}
