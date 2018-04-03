using UnityEngine;
using System.Collections;

public class Arrow : Interactable
{
    [SerializeField]
    private RopeBehaviour _rope;

    public override void Interact(GameObject Object) {
        base.Interact(Object);
    }

    private void OnCollisionEnter(Collision collision) {
        RopeConnector Object = collision.collider.GetComponent<RopeConnector>();

        if (Object != null) {
            Object.Interact(this.gameObject);

            GameObject gb = Instantiate(_rope).gameObject;
            gb.transform.position = transform.position;

            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<Collider>());
        }
    }
}