using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WeaponPlacer : MonoBehaviour
{
    public string weaponId;

    public GameObject weaponModel;

    public bool leftHand;
    public bool saveWeapon;
    void Update()
    {
        if (!saveWeapon)
            return;

        saveWeapon = false;

        if (weaponModel == null)
            return;
        if (string.IsNullOrEmpty(weaponId))
            return;

        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
            return;

        for (int i = 0; i < obj.weapons_all.Count; i++)
        {
            if (obj.weapons_all[i].itemName == weaponId)
            {
                Weapon w = obj.weapons_all[i];

                if(leftHand)
                {
                    w.l_model_eulers = weaponModel.transform.localEulerAngles;
                    w.l_model_pos = weaponModel.transform.localPosition;
                }
                else
                {
                    w.r_model_eulers = weaponModel.transform.localEulerAngles;
                    w.r_model_pos = weaponModel.transform.localPosition;
                }
                w.model_scale = weaponModel.transform.localScale;

                return;
            }
        }

        Debug.Log(weaponId + " wasn't found in inventory");
    }
}
