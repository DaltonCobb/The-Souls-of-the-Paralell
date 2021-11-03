using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public int actionIndex;
    public List<Action> actionSlots = new List<Action>();
    public ItemAction consumableItem;
    StateManager states;
    public void Init(StateManager st)
    {
        states = st;

        UpdateActionOneHanded();
    }

    public void UpdateActionOneHanded()
    {
        EmptyAllSlots();

        StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
        StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);

        if (states.inventoryManager.hasLeftHandWeapon)
        {
            StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlots,true);
            StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlots,true);
        }
        else
        {
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlots);
        }
    }

    public void UpdateActionTwoHanded()
    {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.rightHandWeapon.instance;

        for (int i = 0; i < w.two_handedActions.Count; i++)
        {
            Action a = StaticFunctions.GetAction(w.two_handedActions[i].input, actionSlots);
            a.targetAnim = w.two_handedActions[i].targetAnim;
            StaticFunctions.DeepCopyStepsList(w.two_handedActions[i], a);
            a.type = w.two_handedActions[i].type;
        }
    }

    void EmptyAllSlots()
    {
        for(int i = 0; i < 4; i++)
        {
            Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
            a.steps = null;
            a.mirror = false;
            a.type = ActionType.attack;
        }
    }

    ActionManager()
    {
        for (int i = 0; i < 4; i++)
        {
            Action a = new Action();
            a.input = (ActionInput)i;
            actionSlots.Add(a);
        }

    }

    public Action GetActionSlot(StateManager st)
    {
        ActionInput a_input = GetActionInput(st);
        return StaticFunctions.GetAction(a_input, actionSlots);
    }
    
    public Action GetActionFromInput(ActionInput a_input)
    {
        return StaticFunctions.GetAction(a_input, actionSlots);
    }

    public ActionInput GetActionInput(StateManager st)
    {
        if (st.rb)
            return ActionInput.rb;
        if (st.rt)
            return ActionInput.rt;
        if (st.lb)
            return ActionInput.lb;
        if (st.lt)
            return ActionInput.lt;

        return ActionInput.rb;
    }

    public bool IsLeftHandSlot(Action slot)
    {
        return (slot.input == ActionInput.lb || slot.input == ActionInput.lt);
    }
}

public enum ActionInput
{
   rb,lb,rt,lt
}

public enum ActionType
{
    attack,block,spells,parry
}

public enum SpellClass
{
    pyromancy, miracles, sorcery
}

public enum SpellType
{
    projectile, buff, looping
}
[System.Serializable]
public class Action
{
    public ActionInput input;
    public ActionType type;
    public SpellClass spellClass;
    public string targetAnim;
    [NonReorderable]
    public List<ActionSteps> steps;
    public bool mirror = false;
    public bool canBeParried = true;
    public bool changeSpeed = false;
    public float animSpeed = 1;
    public bool canParry = false;
    public bool canBackStab = false;
    public float staminaCost = 5;
    public int focusCost = 0;

    public ActionSteps GetActionStep(ref int indx)
    {
        if(steps == null || steps.Count == 0)
        {
            ActionSteps defaultStep = new ActionSteps();
            defaultStep.branches = new List<ActionAnim>();
            ActionAnim aa = new ActionAnim();
            aa.input = input;
            aa.targetAnim = targetAnim;
            defaultStep.branches.Add(aa);

            return defaultStep;
        }

        if (indx > steps.Count - 1)
            indx = 0;

        ActionSteps retVal = steps[indx];

        if (indx > steps.Count - 1)
            indx = 0;
        else
            indx++;

        return retVal;
    }

    [HideInInspector]
    public float parryMultiplier;
    [HideInInspector]
    public float backstabMultiplier;

    public bool overideDamageAnim;
    public string damageAnim;
}

[System.Serializable]
public class ActionSteps
{
    [NonReorderable]
    public List<ActionAnim> branches = new List<ActionAnim>();

    public ActionAnim GetBranch(ActionInput inp)
    {
        for(int i =0; i <branches.Count;i++)
        {
            if (branches[i].input == inp)
                return branches[i];
        }

        return branches[0];
    }
}


[System.Serializable]
public class ActionAnim
{
    public ActionInput input;
    public string targetAnim;
}

[System.Serializable]
public class SpellAction
{
    public ActionInput input;
    public string targetAnim;
    public string throwAnim;
    public float castTime;
    public float focusCost;
    public float staminaCost;
}


[System.Serializable]
public class ItemAction
{
    public string targetAnim;
    public string item_id;
}
