using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Header("Init")]
    public GameObject activeModel;

    [Header("Stats")]
    public Attributes attributes;
    public CharacterStats characterStats;

    [Header("Inputs")]
    public float vertical;
    public float horizontal;
    public float moveAmount;
    public Vector3 moveDir;
    public bool rt, rb, lt, lb;
    public bool rollInput;
    public bool itemInput;

    [Header("Stats")]
    public float moveSpeed = 2.0f;
    public float runSpeed = 3.5f;
    public float rotateSpeed = 5.0f;
    public float toGround = 0.5f;
    public float rollSpeed = 1;
    public float parryOffset = 1.4f;
    public float backStabOffset = 1.4f;

    [Header("States")]
    public bool onGround;
    public bool run;
    public bool lockOn;
    public bool inAction;
    public bool canMove;
    public bool isTwoHanded;
    public bool usingItem;
    public bool canBeParried;
    public bool parryIsOn;
    public bool isBlocking;
    public bool isLeftHand;

    [Header("Other")]
    public EnemyTarget lockonTarget;
    public Transform lockOnTransform;
    public AnimationCurve roll_curve;
    //public EnemyStates parryTarget;

    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public Rigidbody rigid;
    [HideInInspector]
    public AnimatorHook a_hook;
    [HideInInspector]
    public ActionManager actionManager;
    [HideInInspector]
    public InventoryManager inventoryManager;

    [HideInInspector]
    public float delta;
    [HideInInspector]
    public LayerMask ignoreLayers;

    [HideInInspector]
    public Action currentAtcion;

    float _actionDelay;

    public void Init()
    {
        SetUpAnimator();
        rigid = GetComponent<Rigidbody>();
        rigid.angularDrag = 999;
        rigid.drag = 4;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        inventoryManager = GetComponent<InventoryManager>();
        inventoryManager.Init(this);

        actionManager = GetComponent<ActionManager>();
        actionManager.Init(this);

        a_hook = activeModel.GetComponent<AnimatorHook>();
        if(a_hook == null)
             a_hook =  activeModel.AddComponent<AnimatorHook>();
        a_hook.Init(this, null);

        gameObject.layer = 8;
        ignoreLayers = ~(1 << 9);

        anim.SetBool("onGround", true);
    }

    void SetUpAnimator()
    {
        if (activeModel == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.Log("No model found");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }

        if (anim == null)
            anim = activeModel.GetComponent<Animator>();

        anim.applyRootMotion = false;
    }

    public void FixedTick(float d)
    {
        delta = d;

        isBlocking = false;
        usingItem = anim.GetBool("interacting");
        DetectAction();
        DetectItemAction();
        inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);

        anim.SetBool("blocking", isBlocking);
        anim.SetBool("isLeft", isLeftHand);

        if (inAction)
        {
            anim.applyRootMotion = true;

            _actionDelay += delta;
            if(_actionDelay > 0.3f)
            {
                inAction = false;
                _actionDelay = 0;
            }
            else
            {
                return;
            }

            return;
        }

        canMove = anim.GetBool("canMove");

        if (!canMove)
            return;

        // a_hook.rm_multi = 1;
        a_hook.CloseRoll();
        HandleRolls();

        anim.applyRootMotion = false;

        rigid.drag = (moveAmount > 0 || onGround == false) ? 0 : 4;

        float targetSpeed = moveSpeed;
        if(usingItem)
        {
            run = false;
            moveAmount = Mathf.Clamp(moveAmount, 0, 0.5f);
        }
        if (run)
            targetSpeed = runSpeed;

        if(onGround)
            rigid.velocity = moveDir * (targetSpeed * moveAmount);

        if (run)
            lockOn = false;

        Vector3 targetDir = (lockOn == false)? moveDir : (lockOnTransform != null)?
            lockOnTransform.transform.position - transform.position : moveDir;

        targetDir.y = 0;
        if (targetDir == Vector3.zero)
            targetDir = transform.forward;
        Quaternion tr = Quaternion.LookRotation(targetDir);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
        transform.rotation = targetRotation;

        anim.SetBool("lockon", lockOn);

        if (lockOn == false)
            HandleMovmentAnimations();
        else
            HandleLockOnAnimations(moveDir);
    }

    public void DetectItemAction()
    {
        if (canMove == false || usingItem || isBlocking)
            return;
        if (itemInput == false)
            return;

        ItemAction slot = actionManager.consumableItem;
        string targetAnim = slot.targetAnim;
        if (string.IsNullOrEmpty(targetAnim))
            return;

       // inventoryManager.curWeapon.weaponModel.SetActive(false);
        usingItem = true;
        anim.Play(targetAnim);
          
    }

    public void DetectAction()
    {
        if (canMove == false || usingItem)
            return;

        if (rb == false && rt == false && lt == false && lb == false)
            return;


        Action slot = actionManager.GetActionSlot(this);
        if (slot == null)
            return;
        switch(slot.type)
        {
            case ActionType.attack:
                AttackAction(slot);
                break;
            case ActionType.block:
                BlockAction(slot);
                break;
            case ActionType.spells:
                break;
            case ActionType.parry:
                ParryAction(slot);
                break;
        }

    }

    void AttackAction(Action slot)
    {
        if (CheckForParry(slot))
            return;

        if (CheckForBackstab(slot))
            return;

        string targetAnim = null;
        targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;


        currentAtcion = slot;
        canMove = false;
        inAction = true;

        float targetSpeed = 1;
        if (slot.changeSpeed)
        {
            targetSpeed = slot.animSpeed;
            if (targetSpeed == 0)
                targetSpeed = 1;
        }
        canBeParried = slot.canBeParried;
        anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
        anim.SetBool(StaticStrings.mirror, slot.mirror);
        anim.CrossFade(targetAnim, 0.2f);
    }

    bool CheckForParry(Action slot)
    {
        EnemyStates parryTarget = null;
        Vector3 origin = transform.position;
        origin.y += 1;
        Vector3 rayDir = transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(origin,rayDir, out hit, 3, ignoreLayers))
        {
            parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
        }

        if (parryTarget == null)
            return false;

        if (parryTarget.parriedBy == null)
            return false;

      //  float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

        //if (dis > 3)
          //  return false;

        Vector3 dir = parryTarget.transform.position - transform.position;
        dir.Normalize();
        dir.y = 0;
        float angle = Vector3.Angle(transform.forward, dir);

        if(angle < 60)
        {
            Vector3 targetPosition = -dir * parryOffset;
            targetPosition += parryTarget.transform.position;
            transform.position = targetPosition;

            if (dir == Vector3.zero)
                dir = -parryTarget.transform.forward;

            Quaternion eRotation = Quaternion.LookRotation(-dir);
            Quaternion ourRot = Quaternion.LookRotation(dir);

            parryTarget.transform.rotation = eRotation;
            transform.rotation = ourRot;
            parryTarget.IsGettingParried();
            canMove = false; 
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(StaticStrings.parry_attack, 0.2f);
            lockonTarget = null;
            return true;
        }

        return false;
    }

    bool CheckForBackstab(Action slot)
    {
        if (slot.canBackStab == false)
            return false;

        EnemyStates backstab = null;
        Vector3 origin = transform.position;
        origin.y += 1;
        Vector3 rayDir = transform.forward;
        RaycastHit hit;
        if(Physics.Raycast(origin, rayDir, out hit, 1,ignoreLayers))
        {
            backstab = hit.transform.GetComponentInParent<EnemyStates>();
        }

        if (backstab == null)
            return false;

        Vector3 dir = transform.position - backstab.transform.position;
        dir.Normalize();
        dir.y = 0;
        float angle = Vector3.Angle(backstab.transform.forward, dir);

        if (angle < 150)
        {
            Vector3 targetPosition = dir * backStabOffset;
            targetPosition += backstab.transform.position;
            transform.position = targetPosition;

            backstab.transform.rotation = transform.rotation;
            backstab.IsGettingBackstabbed();
            canMove = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(StaticStrings.parry_attack, 0.2f);
            lockonTarget = null;
            return true;
        }
        return false;
    }
    void BlockAction(Action slot)
    {
        isBlocking = true;
        isLeftHand = slot.mirror;
    }

    void ParryAction(Action slot)
    {
        string targetAnim = null;
        targetAnim = slot.targetAnim;

        if (string.IsNullOrEmpty(targetAnim))
            return;

        float targetSpeed = 1;
        if (slot.changeSpeed)
        {
            targetSpeed = slot.animSpeed;
            if (targetSpeed == 0)
                targetSpeed = 1;
        }

        anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
        canMove = false;
        inAction = true;
        anim.SetBool(StaticStrings.mirror, slot.mirror);
        anim.CrossFade(targetAnim, 0.2f);
    }
    public void Tick(float d)
    {
        delta = d;
        onGround = OnGround();

        anim.SetBool(StaticStrings.onGround, onGround);
    }

    void HandleRolls()
    {
        if (!rollInput || usingItem)
            return;

        float v = vertical;
        float h = horizontal;
        v = (moveAmount > 0.3f) ? 1 : 0;
        h = 0;

        //if (lockOn == false)
        //{
        //    v = (moveAmount > 0.3f) ? 1 : 0;
        //    h = 0;
        //}
        //else
        //{
        //    if (Mathf.Abs(v) < 0.3f)
        //        v = 0;
        //    if (Mathf.Abs(h) < 0.3f)
        //        h = 0;
        //}

        if (v != 0)
        {
            if (moveDir == Vector3.zero)
                moveDir = transform.forward;
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = targetRot;
            a_hook.InitForRoll();
            a_hook.rm_multi = rollSpeed;
        }
        else
        {
            a_hook.rm_multi = 1.3f;
        }

        anim.SetFloat(StaticStrings.vertical, v);
        anim.SetFloat(StaticStrings.horizontal, h);

        canMove = false;
        inAction = true;
        anim.CrossFade(StaticStrings.Rolls, 0.2f);
    }

    void HandleMovmentAnimations()
    {
        anim.SetBool(StaticStrings.run, run);
        anim.SetFloat(StaticStrings.vertical, moveAmount, 0.4f, delta);
    }

    void HandleLockOnAnimations(Vector3 moveDir)
    {
        Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
        float h = relativeDir.x;
        float v = relativeDir.z;

        anim.SetFloat(StaticStrings.vertical, v, .2f, delta);
        anim.SetFloat(StaticStrings.horizontal, h, .2f, delta);

    }

    public bool OnGround()
    {
        bool r = false;

        Vector3 origin = transform.position + (Vector3.up * toGround);
        Vector3 dir = -Vector3.up;
        float dis = toGround + 0.3f;
        RaycastHit hit;
        Debug.DrawRay(origin, dir * dis);
        if(Physics.Raycast(origin,dir,out hit, dis, ignoreLayers))
        {
            r = true;
            Vector3 targetPosition = hit.point;
            transform.position = targetPosition;
        }

        return r;
    }

    public void HandleTwoHanded()
    {
        anim.SetBool(StaticStrings.two_handed, isTwoHanded);

        if (isTwoHanded)
            actionManager.UpdateActionTwoHanded();
        else
            actionManager.UpdateActionOneHanded();
    }

    public void IsGettingParried()
    {

    }
}
