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
		public static BoolEvent OnEquipBow;
		public static BoolEvent OnEquipSword;
		public static RopePartEvent OnGrabRope;
		public static ControllerEvent OnControllerOverride;
		public static Vec2Event OnMove;

		public static void Init() {
			OnEquipBow = new BoolEvent();
			OnEquipSword = new BoolEvent();
			OnGrabRope = new RopePartEvent();
			OnControllerOverride = new ControllerEvent();
			OnMove = new Vec2Event();
		}
	}

	public static class RopeEvent
	{
		public static RopeTypeEvent OnRopeBreak;

		public static UnityEvent OnRopeClimb;
		public static UnityEvent OnRopeHold;

		public static void Init() {
			OnRopeBreak = new RopeTypeEvent();

			OnRopeClimb = new UnityEvent();
			OnRopeHold = new UnityEvent();
		}
	}

	public static class AIEvent
	{
		public static GameObjectEvent OnEnemyDeath;

		public static void Init() {
			OnEnemyDeath = new GameObjectEvent();
		}
	}

	public override void Init() {
		InputEvent.Init();
		PlayerEvent.Init();
		RopeEvent.Init();
		AIEvent.Init();
	}
}