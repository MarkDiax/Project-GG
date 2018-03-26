﻿using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>, IManager
{
    #region Base Classes
    public class FloatEvent : UnityEvent<float> { }

    public class GameObjectEvent : UnityEvent<GameObject> { }
    
    public class RopePartEvent : UnityEvent<RopePart> { }

    public class BoolEvent : UnityEvent<bool> { }
    #endregion

    public static class RopeEvent
    {
        public static BoolEvent OnRope;
        public static RopePartEvent OnRopeTrigger;
        public static FloatEvent OnRopeClimbing;

        public static UnityAction OnHandSwitch;
        public static UnityAction OnRopeHold;

        public static void Init() {
            OnRope = new BoolEvent();
            OnRopeClimbing = new FloatEvent();
            OnRopeTrigger = new RopePartEvent();
        }
    }

    public override void Init() {
        RopeEvent.Init();
    }

    public void Update() {
        //
    }
}