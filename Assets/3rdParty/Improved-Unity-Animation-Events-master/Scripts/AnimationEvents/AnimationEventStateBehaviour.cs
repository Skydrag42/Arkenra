using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventStateBehaviour : StateMachineBehaviour {
    [System.Serializable] public class AnimationEventData
    {
        public string eventName;
        [Range(0f, 1f)] public float triggerTime;

        [HideInInspector] public bool hasTriggered;

        public AnimationEventData() { }
        public AnimationEventData(string eventName, float triggerTime, bool hasTriggered)
        {
            this.eventName = eventName;
            this.triggerTime = triggerTime;
            this.hasTriggered = hasTriggered;
        }
    }

    public List<AnimationEventData> events = new List<AnimationEventData>() { 
        new AnimationEventData("OnPrepareAttack", 0, false),
        new AnimationEventData("EnableHitboxes", 0.25f, false),
        new AnimationEventData("OnAttack", 0.25f, false),
        new AnimationEventData("OnCombo0", 0.25f, false),
        new AnimationEventData("DisableHitboxes", 0.5f, false),
        new AnimationEventData("OnFinishAttack", 0.5f, false),
        new AnimationEventData("OnNothing", 0.75f, false),
    };
    public bool sendOnStateExitEvent = false;
    public string onStateExitEventName = "";

    AnimationEventReceiver receiver;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (var e in events)
        {
            e.hasTriggered = false;
        }
        receiver = animator.GetComponent<AnimationEventReceiver>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float currentTime = stateInfo.normalizedTime % 1f;


        //float frame = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length * currentTime * animator.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate;
        //Debug.Log($"Current frame: {frame}, time: {currentTime}/{animator.GetCurrentAnimatorClipInfo(0)[0].clip.length}, clips: {animator.GetCurrentAnimatorClipInfo(0).Length}");
        foreach (var e in events)
        {
            if (!e.hasTriggered && currentTime >= e.triggerTime)
            {
                NotifyReceiver(e.eventName);
                e.hasTriggered = true;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (sendOnStateExitEvent)
            NotifyReceiver(onStateExitEventName);
    }

    void NotifyReceiver(string eventName) {
        if (receiver != null) {
            receiver.OnAnimationEventTriggered(eventName);
        }
    }
}
