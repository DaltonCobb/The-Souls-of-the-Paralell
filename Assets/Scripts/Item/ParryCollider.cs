using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCollider : MonoBehaviour
{
    StateManager states;
    EnemyStates eStates;
    public void InitPlayer(StateManager st)
    {
        states = st;
    }

    public void InitEnemy(EnemyStates st)
    {
        eStates = st;
    }

    private void OnTriggerEnter(Collider other)
    {
    //    DamageColider dc = other.GetComponent<DamageColider>();
    //    if (dc == null)
    //        return;

        if (states)
        {
            EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

            if (e_st != null)
            {
                e_st.CheckForParry(transform.root, states);
            }
        }

        if(eStates)
        {
            //check for player
        }
    }
}
