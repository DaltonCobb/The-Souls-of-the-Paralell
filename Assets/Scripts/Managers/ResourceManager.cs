using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    Dictionary<string, int> spell_ids = new Dictionary<string, int>();
    Dictionary<string, int> weapon_ids = new Dictionary<string, int>();
    Dictionary<string, int> weaponstats_ids = new Dictionary<string, int>();
    public static ResourceManager singleton;
    private void Awake()
    {
        singleton = this;
        LoadWeaponIds();
        LoadSpellIds();
    }

    void LoadSpellIds()
    {
        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;

        if (obj == null)
        {
            Debug.Log("SpellItemScriptableObject couldnt be loaded");
            return;
        }

        for(int i = 0; i < obj.spell_item.Count; i++)
        {
            if(spell_ids.ContainsKey(obj.spell_item[i].itemName))
            {
                Debug.Log(obj.spell_item[i].itemName + " item is a duplicate");
            }
            else
            {
                spell_ids.Add(obj.spell_item[i].itemName, i);
            }
        }
    }

    void LoadWeaponIds()
    {
        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if(obj == null)
        {
            Debug.Log("WeaponItemScriptableObject couldnt be loaded");
            return;
        }

        for(int i = 0; i < obj.weapons_all.Count; i++)
        {
            if(weapon_ids.ContainsKey(obj.weapons_all[i].itemName))
            {
                Debug.Log(obj.weapons_all[i].itemName +  " item is a duplicate");

            }
            else
            {
                weapon_ids.Add(obj.weapons_all[i].itemName, i);
            }
        }

        for (int i = 0; i < obj.weaponStats.Count; i++)
        {
            if (weaponstats_ids.ContainsKey(obj.weaponStats[i].weaponid))
            {
                Debug.Log(obj.weaponStats[i].weaponid + " is a duplicate");
            }
            else
            {
                weaponstats_ids.Add(obj.weaponStats[i].weaponid, i);
            }
        }
    }

    int GetWeaponIdFromString(string id)
    {
        int index = -1;
        if(weapon_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return index;
    }
    public Weapon GetWeapon(string id)
    {
        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject cant be loaded");
            return null;
        }

        int index = GetWeaponIdFromString(id);

        if (index == -1)
            return null;

        return obj.weapons_all[index];
    }

    public WeaponStats GetWeaponStats(string id)
    {
        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject cant be loaded");
            return null;
        }

        int index = GetWeaponStatsIdFromString(id);

        if (index == -1)
            return null;

        return obj.weaponStats[index];
    }


    int GetWeaponStatsIdFromString(string id)
    {
        int index = -1;
        if (weaponstats_ids.TryGetValue(id, out index))
        {
            return index;
        }
        return index;
    }

    int GetSpellIdFromString(string id)
    {
        int index = -1;
        if(spell_ids.TryGetValue(id, out index))
        {
            return index;
        }

        return index;
    }

    public Spell GetSpell(string id)
    {
        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;
        if(obj == null)
        {
            Debug.Log("SpellItemScriptableObject cant be loaded");
            return null;
        }

        int index = GetSpellIdFromString(id);
        if (index == -1)
            return null;

        return obj.spell_item[index];
    }
}
