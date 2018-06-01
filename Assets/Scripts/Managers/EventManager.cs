using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EventManager : Singleton<EventManager>
{
    #region Base Classes
    public class FloatEvent : UnityEvent<float> { }
    public class BoolEvent : UnityEvent<bool> { }
    public class Bool2Event : UnityEvent<bool, bool> { }
    public class Vec2Event : UnityEvent<Vector2> { }
    public class GameObjectEvent : UnityEvent<GameObject> { }
    public class ControllerEvent : UnityEvent<BaseController, bool> { }

    public class RopePartEvent : UnityEvent<RopePart> { }
    public class RopeTypeEvent : UnityEvent<RopeBehaviour> { }
    #endregion

    public static class InputEvent
    {
        public static BoolEvent OnBowDraw;
        public static UnityEvent OnBowShoot;
        public static UnityEvent OnMelee;
        public static BoolEvent OnCameraZoom;
        public static UnityEvent OnJump;

        public static void Init() {
            OnBowDraw = new BoolEvent();
            OnBowShoot = new UnityEvent();
            OnMelee = new UnityEvent();
            OnCameraZoom = new BoolEvent();
            OnJump = new UnityEvent();
        }
    }

    public static class PlayerEvent
    {
        public static ControllerEvent OnControllerOverride;
        public static Vec2Event OnMove;

        public static void Init() {
            OnMove = new Vec2Event();
            OnControllerOverride = new ControllerEvent();
        }
    }

    public static class RopeEvent
    {
        public static BoolEvent OnRope;
        public static RopePartEvent OnRopeTrigger;
        public static FloatEvent OnRopeClimbing;
        public static RopeTypeEvent OnRopeBreak;

        public static UnityEvent OnRopeClimb;
        public static UnityEvent OnRopeHold;

        public static void Init() {
            OnRope = new BoolEvent();
            OnRopeClimbing = new FloatEvent();
            OnRopeTrigger = new RopePartEvent();
            OnRopeBreak = new RopeTypeEvent();

            OnRopeClimb = new UnityEvent();
            OnRopeHold = new UnityEvent();
        }
    }
   
    public static class AnimationEvent
    {
        public static Bool2Event UseRootMotion;
        public static UnityEvent OnActualJump; //the moment the animation jumps / increases in heigtht
        public static UnityEvent OnDealDamage;

        public static void Init() {
            UseRootMotion = new Bool2Event();
            OnActualJump = new UnityEvent();
            OnDealDamage = new UnityEvent();
        }
    }

    public override void Init() {
        InputEvent.Init();
        PlayerEvent.Init();
        RopeEvent.Init();
        AnimationEvent.Init();
    }
}