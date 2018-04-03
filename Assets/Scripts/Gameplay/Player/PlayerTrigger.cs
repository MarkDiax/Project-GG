using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.tag != Player.Instance.tag) {
            if (other.gameObject.layer == (int)Layers.Rope) {

                if (EventManager.RopeEvent.OnRopeTrigger != null)
                    EventManager.RopeEvent.OnRopeTrigger.Invoke(other.GetComponent<RopePart>());
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag != Player.Instance.tag) {
            if (other.gameObject.layer == (int)Layers.Rope) {

                if (EventManager.RopeEvent.OnRopeTrigger != null)
                    EventManager.RopeEvent.OnRopeTrigger.Invoke(other.GetComponent<RopePart>());
            }
        }
    }
}