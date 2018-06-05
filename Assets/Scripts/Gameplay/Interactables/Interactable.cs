using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent OnInteract;
    public bool isInteractableByPlayer = true;
    public bool useButtonPrompt = true;

    public virtual void Interact(GameObject Object) {
        if (!isInteractableByPlayer && Object == Player.Instance.gameObject)
            return;

        Debug.Log(name + " has been interacted by: " + Object.name);
        OnInteract.Invoke();
    }

    protected virtual void Update() {

    }

}