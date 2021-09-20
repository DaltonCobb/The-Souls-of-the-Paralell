using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageColider : MonoBehaviour
{
    StateManager states;

    public void Init(StateManager st)
    {
        states = st;
    }
    private void OnTriggerEnter(Collider other)
    {
        EnemyStates eStates = other.transform.GetComponentInParent<EnemyStates>();

        if (eStates == null)
            return;

        eStates.DoDamage(states.currentAtcion);
    }
}
