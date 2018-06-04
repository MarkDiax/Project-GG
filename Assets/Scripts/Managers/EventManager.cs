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
        public static BoolEvent OnCameraZoom;

        public static void Init() {
            OnBowDraw = new BoolEvent();
            OnCameraZoom = new BoolEvent();
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
        public static RopeTypeEvent OnRopeBreak;

        public static UnityEvent OnRopeClimb;
        public static UnityEvent OnRopeHold;

        public static void Init() {
            OnRope = new BoolEvent();
            OnRopeTrigger = new RopePartEvent();
            OnRopeBreak = new RopeTypeEvent();

            OnRopeClimb = new UnityEvent();
            OnRopeHold = new UnityEvent();
        }
    }

    public override void Init() {
        InputEvent.Init();
        PlayerEvent.Init();
        RopeEvent.Init();
    }
}