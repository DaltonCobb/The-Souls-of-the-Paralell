using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    float vertical;
    float horizontal;
    bool b_input;
    bool a_input;
    bool x_input;
    bool y_input;

    bool rb_input;
    float rt_axis;
    bool rt_input;
    bool lb_input;
    float lt_axis;
    bool lt_input;

    float d_y;
    float d_x;
    bool d_up;
    bool d_down;
    bool d_right;
    bool d_left;

    bool p_d_up;
    bool p_d_down;
    bool p_d_left;
    bool p_d_right;


    bool leftAxis_down;
    bool rightAxis_down;

    float b_timer;
    float rt_timer;
    float lt_timer;

    float delta;

    StateManager states;
    CameraManager camManager;
    UiManager uiManager;
    void Start()
    {
        QuickSlot.singleton.Init();

        states = GetComponent<StateManager>();
        states.Init();

        camManager = CameraManager.singleton;
        camManager.Init(states);

        uiManager = UiManager.singleton;
    }

    void FixedUpdate()
    {
        delta = Time.fixedDeltaTime;
        GetInput();
        UpdateStates();
        states.FixedTick(delta);
        camManager.Tick(delta);

    }

    void Update()
    {
        delta = Time.deltaTime;
        states.Tick(delta);
        ResetInputNStates();
        states.MonitorStats();
        uiManager.Tick(states.characterStats, delta);
    }
    void GetInput()
    {
        vertical = Input.GetAxis(StaticStrings.Vertical);
        horizontal = Input.GetAxis(StaticStrings.Horizontal);
        b_input = Input.GetButton(StaticStrings.B);
        a_input = Input.GetButton(StaticStrings.A);
        y_input = Input.GetButtonUp(StaticStrings.Y);
        x_input = Input.GetButton(StaticStrings.X);
        rt_input = Input.GetButton(StaticStrings.RT);
        rt_axis = Input.GetAxis(StaticStrings.RT);
        if (rt_axis != 0)
            rt_input = true;

        lt_input = Input.GetButton(StaticStrings.LT);
        lt_axis = Input.GetAxis(StaticStrings.LT);
        if (lt_axis != 0)
            lt_input = true;
        rb_input = Input.GetButton(StaticStrings.RB);
        lb_input = Input.GetButton(StaticStrings.LB);

        rightAxis_down = Input.GetButtonUp(StaticStrings.L) || Input.GetKeyUp(KeyCode.T);

        if (b_input)
            b_timer += delta;

        d_x = Input.GetAxis(StaticStrings.Pad_x);
        d_y = Input.GetAxis(StaticStrings.Pad_y);

        d_up = Input.GetKeyUp(KeyCode.Alpha1) || d_y > 0;
        d_down = Input.GetKeyUp(KeyCode.Alpha2) || d_y < 0;
        d_left = Input.GetKeyUp(KeyCode.Alpha3) || d_x < 0;
        d_right = Input.GetKeyUp(KeyCode.Alpha4) || d_x > 0;
    }

    void UpdateStates()
    {
        states.horizontal = horizontal;
        states.vertical = vertical;

        Vector3 v = vertical * camManager.transform.forward;
        Vector3 h = horizontal * camManager.transform.right;
        states.moveDir = (v + h).normalized;
        float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
        states.moveAmount = Mathf.Clamp01(m);

        if (x_input)
            b_input = false;

        if (b_input && b_timer > 0.5f)
        {
            states.run = (states.moveAmount > 0) && states.characterStats._stamina > 0;
        }

        if (b_input == false && b_timer > 0 && b_timer < 0.5f)
            states.rollInput = true;

        states.itemInput = x_input;
        states.rt = rt_input;
        states.lt = lt_input;
        states.rb = rb_input;
        states.lb = lb_input;

        if(y_input)
        {
            states.isTwoHanded = !states.isTwoHanded;
            states.HandleTwoHanded();
        }

        if (states.lockonTarget != null)
        {
            if (states.lockonTarget.eStates.isDead)
            {
                states.lockOn = false;
                states.lockonTarget = null;
                states.lockOnTransform = null;
                camManager.lockon = false;
                camManager.lockOnTarget = null;
            }
        }else
        {
            states.lockOn = false;
            states.lockonTarget = null;
            states.lockOnTransform = null;
            camManager.lockon = false;
            camManager.lockOnTarget = null;
        }

        if (rightAxis_down)
        {
            states.lockOn = !states.lockOn;

            states.lockonTarget = EnemyManager.singleton.GetEnemy(transform.position);
            if (states.lockonTarget == null)
                states.lockOn = false;

            camManager.lockOnTarget = states.lockonTarget;
            states.lockOnTransform = states.lockonTarget.GetTarget();
            camManager.lockonTransform = states.lockOnTransform;
            camManager.lockon = states.lockOn;
        }
        HandleQuickSlotChanges();
    }
    void HandleQuickSlotChanges()
    {

        if (states.isSpellcasting || states.usingItem)
            return;

        if (d_up)
        {
            if (!p_d_up)
            {
                p_d_up = true;
                states.inventoryManager.ChangeToNextSpell();
            }
        }
        if (!d_up)
            p_d_up = false;

        if (!d_down)
            p_d_down = false;

        if (states.canMove == false)
            return;
        if (states.isTwoHanded)
            return;

        if (d_left)
        {
            if (!p_d_left)
            {
                states.inventoryManager.ChangeToNextWeapon(true);
                p_d_left = true;
            }

        }
        if (d_right)
        {
            if (!p_d_right)
            {
                states.inventoryManager.ChangeToNextWeapon(false);
                p_d_right = true;

            }
        }
       
        if (!d_left)
            p_d_left = false;
        if (!d_right)
            p_d_right = false;
    }

    void ResetInputNStates()
    {
        if (b_input == false)
            b_timer = 0;
        if (states.rollInput)
            states.rollInput = false;
        if (states.run)
            states.run = false;
    }

}
