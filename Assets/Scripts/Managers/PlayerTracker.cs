using UnityEngine;

public class PlayerTracker : ScriptableObject {

	private static Player player;

	public static Player Player {
		get {
			if (player == null)
				player = FindObjectOfType<Player>();

			return player; }
	}
}