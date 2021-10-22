using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectsManager : MonoBehaviour
{
    Dictionary<string, int> s_effects = new Dictionary<string, int>();

    public void UseSpellEffect(string id, StateManager c, EnemyStates e = null)
    {
        int index= GetEffect(id);

        if(index == -1)
        {
            Debug.Log("Spell effect doesn't exist");
            return;
        }

        switch(index)
        {
            case 0:
                FireBreath(c);
                break;
            case 1:
                DarkShield(c);
                break;
            case 2:
                HealingSmall(c);
                break;
            case 3:
                FireBall(c);
                break;
            case 4:
                OnFire(c, e);
                break;

            default:
                break;
        }
    }

    int GetEffect(string id)
    {
        int index = -1;

        if(s_effects.TryGetValue(id, out index))
        {
            return index;
        }

       return index;
    }

    void FireBreath(StateManager c)
    {
        c.spellCast_start = c.inventoryManager.OpenBreathCollider;
        c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
        c.spellCast_stop = c.inventoryManager.CloseBreathCollider;
    }

    void DarkShield(StateManager c)
    {
        c.spellCast_start = c.inventoryManager.OpenBlockCollider;
        c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
        c.spellCast_stop = c.inventoryManager.CloseBlockCollider;
    }

    void HealingSmall(StateManager c)
    {
        c.spellCast_loop = c.AddHealth;
    }
    void FireBall(StateManager c)
    {
        c.spellCast_start = c.inventoryManager.CloseBlockCollider;
        c.spellCast_start = c.inventoryManager.CloseBreathCollider;
        c.spellCast_loop = c.inventoryManager.EmitSpellParticle;
    }

    void OnFire(StateManager c, EnemyStates e)
    {
        if(c != null)
        {

        }

        if(e != null)
        {
            e.spellEffect_loop = e.OnFire;
        }
    }

    public static SpellEffectsManager singleton;
    void Awake()
    {
        singleton = this;

        s_effects.Add("firebreath", 0);
        s_effects.Add("darkshield", 1);
        s_effects.Add("healingSmall", 2);
        s_effects.Add("fireball", 3);
        s_effects.Add("onFire", 4);
    }
}
