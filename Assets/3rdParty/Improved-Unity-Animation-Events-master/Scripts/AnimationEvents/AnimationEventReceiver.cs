using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class AnimationEventReceiver : MonoBehaviour {
    [SerializeField] List<AnimationEvent> animationEvents = new();

    Dictionary<string, UnityEvent> animationEventsDict;

    private void Awake()
    {
        animationEventsDict = new Dictionary<string, UnityEvent>();
        foreach (var animationEvent in animationEvents)
        {
            animationEventsDict.Add(animationEvent.eventName, animationEvent.OnAnimationEvent);
        }
    }

    public void OnAnimationEventTriggered(string eventName) {
        if (animationEventsDict.TryGetValue(eventName, out UnityEvent onEvent))
            onEvent?.Invoke();

        //AnimationEvent matchingEvent = animationEvents.Find(se => se.eventName == eventName);
        //matchingEvent?.OnAnimationEvent?.Invoke();
    }
}