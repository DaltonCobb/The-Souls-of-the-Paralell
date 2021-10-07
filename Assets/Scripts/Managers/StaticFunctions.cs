using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFunctions
{ 
    public static void DeepCopyWeapon(Weapon from, Weapon to)
    {
      //  Debug.Log(from == null);
        to.icon = from.icon;
        to.oh_idle = from.oh_idle;
        to.th_idle = from.th_idle;

        to.actions = new List<Action>();
        for(int i = 0; i < from.actions.Count; i++)
        {
            Action a = new Action();
            a.weaponStats = new WeaponStats();
            DeepCopyActionToAction(a, from.actions[i]);
            to.actions.Add(a);
        }

        to.two_handedActions = new List<Action>();

        for (int i = 0; i < from.two_handedActions.Count; i++)
        {
            Action a = new Action();
            a.weaponStats = new WeaponStats();
            DeepCopyActionToAction(a, from.two_handedActions[i]);
            to.two_handedActions.Add(a);
        }
        to.parryMultiplier = from.parryMultiplier;
        to.backstabMultiplier = from.backstabMultiplier;
        to.LeftHandMirror = from.LeftHandMirror;
        to.modelPrefab = from.modelPrefab;
        to.l_model_eulers = from.l_model_eulers;
        to.l_model_pos = from.l_model_pos;
        to.r_model_eulers = from.r_model_eulers;
        to.r_model_pos = from.r_model_pos;
        to.model_scale = from.model_scale;

    }
    public static void DeepCopyActionToAction(Action a, Action w_a)
    {
        a.input = w_a.input;
        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.spellClass = w_a.spellClass;
        a.canParry = w_a.canParry;
        a.canBeParried = w_a.canBeParried;
        a.changeSpeed = w_a.changeSpeed;
        a.animSpeed = w_a.animSpeed;
        a.canBackStab = w_a.canBackStab;
        a.overideDamageAnim = w_a.overideDamageAnim;
        a.damageAnim = w_a.damageAnim;

        DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
    }


    public static void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assign,List<Action> actionList, bool isLeftHand = false)
    {
        Action a = GetAction(assign, actionList);
        Action w_a = w.GetAction(w.actions, inp);
        if (w_a == null)
            return;

        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.spellClass = w_a.spellClass;
        a.canBeParried = w_a.canBeParried;
        a.changeSpeed = w_a.changeSpeed;
        a.animSpeed = w_a.animSpeed;
        a.canBackStab = w_a.canBackStab;
        a.overideDamageAnim = w_a.overideDamageAnim;
        a.damageAnim = w_a.damageAnim;
        a.parryMultiplier = w.parryMultiplier;
        a.backstabMultiplier = w.backstabMultiplier;

        if (isLeftHand)
        {
            a.mirror = true;
        }
        DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
    }

    public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
    {
        to.physical = from.physical;
        to.slash = from.slash;
        to.strike = from.strike;
        to.thrust = from.thrust;
        to.magic = from.magic;
        to.lighting = from.lighting;
        to.fire = from.fire;
        to.dark = from.dark;

    }
    public static Action GetAction(ActionInput inp, List<Action> actionSlots)
    {

        for (int i = 0; i < actionSlots.Count; i++)
        {
            if (actionSlots[i].input == inp)
                return actionSlots[i];
        }
        return null;
    }

    public static void DeepCopySpell(Spell from, Spell to)
    {
        to.itemName = from.itemName;
        to.itemDescription = from.itemDescription;
        to.icon = from.icon;
        to.spellType = from.spellType;
        to.projectile = from.projectile;
        to.particlePrefab = from.particlePrefab;
    }
}
