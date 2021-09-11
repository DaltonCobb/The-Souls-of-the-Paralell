using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    public float health;
    public bool canBeParried = true;
    public bool parryIsOn = true;
   // public bool doParry = false;
    public bool isInvicible;
    public bool dontDoAnything;
    public bool canMove;
    public bool isDead;

    StateManager parredBy;

    public Animator anim;
    EnemyTarget enTarget;
    AnimatorHook a_hook;
    public Rigidbody rigid;
    public float delta;

    List<Rigidbody> ragdollRigds = new List<Rigidbody>();
    List<Collider> ragdollColliders = new List<Collider>();

    float timer;

    private void Start()
    {
        health = 100;
        anim = GetComponentInChildren<Animator>();
        enTarget = GetComponent<EnemyTarget>();
        enTarget.Init(this);

        rigid = GetComponent<Rigidbody>();

        a_hook = anim.GetComponent<AnimatorHook>();
        if (a_hook == null)
            a_hook = anim.gameObject.AddComponent<AnimatorHook>();
        a_hook.Init(null, this);

        InitRagdoll();
        parryIsOn = false;
    }

    void InitRagdoll()
    {
        Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
        for(int i = 0; i < rigs.Length; i++ )
        {
            if (rigs[i] == rigid)
                continue;
            ragdollRigds.Add(rigs[i]);
            rigs[i].isKinematic = true;

            Collider col = rigs[i].gameObject.GetComponent<Collider>();
            col.isTrigger = true;
            ragdollColliders.Add(col);
        }
    }

    public void EnableRagdoll()
    {
        for(int i = 0; i < ragdollRigds.Count; i++)
        {
            ragdollRigds[i].isKinematic = false;
            ragdollColliders[i].isTrigger = false;
        }
        Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
        controllerCollider.enabled = false;
        rigid.isKinematic = true;

        StartCoroutine("CloseAnimator");
    }

    IEnumerator CloseAnimator()
    {
        yield return new WaitForEndOfFrame();
        anim.enabled = false;
        this.enabled = false;
    }

    private void Update()
    {
        delta = Time.deltaTime;
        canMove = anim.GetBool("canMove");

        if(dontDoAnything)
        {
            dontDoAnything = !canMove;

            return;
        }

        if(health <=0)
        {
            if(!isDead)
            {
                isDead = true;
                EnableRagdoll();
            }
        }

        if(isInvicible)
        {
            isInvicible = !canMove;
        }

        if(parredBy != null && parryIsOn == false)
        {
            parredBy.parryTarget = null;
            parredBy = null;
        }

        if(canMove)
        {
            parryIsOn = false;
            anim.applyRootMotion = false;

            //debug
            timer += Time.deltaTime;
            if(timer > 3)
            {
                DoAction();
                timer = 0; 
            }
        }
    }

    void DoAction()
    {
        anim.Play("oh_attack_1");
        anim.applyRootMotion = true;
        anim.SetBool("canMove", false);
    }

    public void DoDamage(float v)
    {
        if (isInvicible)
            return;

        health -= v;
        isInvicible = true;
        anim.Play("damage_1");
        anim.applyRootMotion = true;
        anim.SetBool("canMove", false);
    }

    public void CheckForParry(Transform target, StateManager states)
    {
        if (canBeParried == false || parryIsOn == false || isInvicible)
            return;

        Vector3 dir = transform.position - target.position;
        dir.Normalize();
        float dot = Vector3.Dot(target.forward, dir);
        if (dot < 0)
            return;

        isInvicible = true;
        anim.Play("attack_interrupt");
        anim.applyRootMotion = true;
        anim.SetBool("canMove", false);
        states.parryTarget = this;
        parredBy = states;
        return;
    }

    public void IsGettingParried()
    {
        health -= 500;
        dontDoAnything = true;
        anim.SetBool("canMove", false);
        anim.Play("parry_recieved");
    }
}
