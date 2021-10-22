using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        EnemyStates es = other.GetComponentInParent<EnemyStates>();
        if(es != null)
        {
            es.DoDamage_();
            SpellEffectsManager.singleton.UseSpellEffect("onFire", null, es);
        }
    }
}
