using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class FirstDescentChecks
{
    static BindingFlags Flags=BindingFlags.Instance|BindingFlags.NonPublic;
    static void Set(object obj,string field,object value)=>obj.GetType().GetField(field,Flags).SetValue(obj,value);
    static async Task Wait(float seconds)
    {
        float until=Time.time+seconds,deadline=Time.realtimeSinceStartup+seconds+10;
        while(Time.time<until)
        {if(!Application.isPlaying||Time.realtimeSinceStartup>deadline)throw new Exception("Play Mode stopped/paused");await Task.Delay(20);}
    }
    static void Place(GameObject go,Vector3 position,Quaternion rotation)
    {
        var cc=go.GetComponent<CharacterController>();cc.enabled=false;
        go.transform.SetPositionAndRotation(position,rotation);cc.enabled=true;Physics.SyncTransforms();
    }
    static void Keys(params Key[] keys)=>InputSystem.QueueStateEvent(Keyboard.current,new KeyboardState(keys));
    static void Mouse(bool block)=>InputSystem.QueueStateEvent(UnityEngine.InputSystem.Mouse.current,new MouseState().WithButton(MouseButton.Right,block));

    public static async Task<object> Main()
    {
        if(!Application.isPlaying)throw new Exception("Play Mode required");
        bool bg=Application.runInBackground;Application.runInBackground=true;
        var p=GameObject.Find("Player");var e=GameObject.Find("Enemy_Dummy");
        var ai=e.GetComponent<EnemyAI>();var pc=p.GetComponent<PlayerCombat>();
        var pv=p.GetComponent<CombatAnimator>();var ev=e.GetComponent<CombatAnimator>();
        var ph=p.GetComponent<Health>();var eh=e.GetComponent<Health>();
        var results=new List<object>();
        try
        {
            ai.enabled=false;Keys();Mouse(false);
            Place(e,new Vector3(0,.05f,5),Quaternion.Euler(0,180,0));
            Place(p,new Vector3(0,.05f,-3),Quaternion.identity);await Wait(.3f);
            Vector3 start=p.transform.position;Keys(Key.W);await Wait(.7f);
            float walk=Vector3.Distance(start,p.transform.position);bool walkClip=pv.animator.GetCurrentAnimatorStateInfo(0).IsName("Walk");
            Keys();await Wait(.3f);start=p.transform.position;Keys(Key.W,Key.LeftShift);await Wait(.7f);
            float run=Vector3.Distance(start,p.transform.position);bool runClip=pv.animator.GetCurrentAnimatorStateInfo(0).IsName("Run");Keys();await Wait(.3f);
            results.Add(new {test="walk_run",passed=walk>.7f&&run>walk*1.3f&&walkClip&&runClip,walk,run,walkClip,runClip});
            Keys(Key.Space);await Wait(.07f);
            bool dodge=p.GetComponent<PlayerMovement>().IsDodging,invulnerable=ph.IsInvulnerable;Keys();await Wait(.8f);
            results.Add(new {test="dodge",passed=dodge&&invulnerable&&!ph.IsInvulnerable,dodge,invulnerable});
            Place(p,new Vector3(0,.05f,-1),Quaternion.identity);Place(e,new Vector3(0,.05f,.1f),Quaternion.Euler(0,180,0));await Wait(.15f);
            float before=eh.CurrentHealth;int count=pv.ContactCount;
            typeof(PlayerCombat).GetMethod("TryAttack",Flags).Invoke(pc,null);await Wait(1.05f);
            results.Add(new {test="player_weapon_contact",passed=eh.CurrentHealth==before-25&&pv.ContactCount==count+1,before,after=eh.CurrentHealth,contacts=pv.ContactCount-count});
            ai.enabled=true;Set(ai,"nextAttackTime",float.MaxValue);
            foreach(bool block in new[]{true,false})
            {
                Place(p,new Vector3(0,.05f,-1),Quaternion.identity);Place(e,new Vector3(0,.05f,.1f),Quaternion.Euler(0,180,0));
                Mouse(block);await Wait(.2f);before=ph.CurrentHealth;count=ev.ContactCount;
                ai.StartCoroutine((System.Collections.IEnumerator)typeof(EnemyAI).GetMethod("AttackRoutine",Flags).Invoke(ai,null));
                await Wait(1.4f);Set(ai,"nextAttackTime",float.MaxValue);
                results.Add(new {test=block?"animated_shield_block":"enemy_weapon_contact",passed=ph.CurrentHealth==before-(block?0:10)&&ev.ContactCount==count+1,before,after=ph.CurrentHealth,contacts=ev.ContactCount-count});
            }
            Mouse(false);Place(p,new Vector3(0,.05f,-2),Quaternion.identity);Place(e,new Vector3(0,.05f,3),Quaternion.Euler(0,180,0));
            float distance=Vector3.Distance(p.transform.position,e.transform.position);await Wait(1.0f);
            float afterDistance=Vector3.Distance(p.transform.position,e.transform.position);
            results.Add(new {test="perception_chase",passed=afterDistance<distance-1, distance,afterDistance});
            ai.enabled=false;Set(ai,"nextAttackTime",float.MaxValue);ai.enabled=true;
            Place(p,new Vector3(0,.05f,-1),Quaternion.identity);Place(e,new Vector3(0,.05f,.1f),Quaternion.Euler(0,180,0));await Wait(.15f);
            before=ph.CurrentHealth;
            ai.StartCoroutine((System.Collections.IEnumerator)typeof(EnemyAI).GetMethod("AttackRoutine",Flags).Invoke(ai,null));
            await Wait(.15f);eh.TakeDamage(1000);await Wait(1.1f);
            results.Add(new {test="animated_death_cancels_contact",passed=ph.CurrentHealth==before&&ev.animator.GetCurrentAnimatorStateInfo(0).IsName("Death"),before,after=ph.CurrentHealth});
            results.Add(new {test="floor_collision",passed=p.GetComponent<CharacterController>().isGrounded&&p.transform.position.y>-.2f,playerY=p.transform.position.y});
            return results;
        }
        finally {Keys();Mouse(false);Application.runInBackground=bg;}
    }
    public static async Task<object> Contact()
    {
        if(!Application.isPlaying)throw new Exception("Play Mode required");
        bool bg=Application.runInBackground;Application.runInBackground=true;
        var p=GameObject.Find("Player");var e=GameObject.Find("Enemy_Dummy");
        var ai=e.GetComponent<EnemyAI>(); ai.enabled=false;
        var result=new List<object>();
        try
        {
            p.GetComponent<CharacterController>().enabled=false;e.GetComponent<CharacterController>().enabled=false;
            p.transform.SetPositionAndRotation(new Vector3(0,.05f,-1),Quaternion.identity);
            e.transform.SetPositionAndRotation(new Vector3(0,.05f,.1f),Quaternion.Euler(0,180,0));
            p.GetComponent<CharacterController>().enabled=true;e.GetComponent<CharacterController>().enabled=true;
            Physics.SyncTransforms();await Wait(.1f);
            var pc=p.GetComponent<PlayerCombat>();var visual=p.GetComponent<CombatAnimator>();
            typeof(PlayerCombat).GetMethod("TryAttack",Flags).Invoke(pc,null);
            for(int i=0;i<8;i++)
            {
                await Wait(.1f);
                result.Add(new {t=i*.1f+.1f,bladeBase=p.transform.InverseTransformPoint(visual.bladeBase.position).ToString("F3"),
                    bladeTip=p.transform.InverseTransformPoint(visual.bladeTip.position).ToString("F3"),targetHP=e.GetComponent<Health>().CurrentHealth,
                    contacts=visual.ContactCount,state=visual.animator.GetCurrentAnimatorStateInfo(0).normalizedTime});
            }
            return result;
        }
        finally {ai.enabled=true;Application.runInBackground=bg;}
    }
}
