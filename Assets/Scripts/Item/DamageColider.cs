using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

        if (eStates == null)
            return;

        //do damage
        eStates.DoDamage(45);
    }
}
