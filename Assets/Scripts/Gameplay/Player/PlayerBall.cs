using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{

    private Coroutine Climb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Rope" && Climb == null)
            Climb = StartCoroutine(RopeClimb(other));
    }

    private IEnumerator RopeClimb(Collider Part)
    {

        CableComponent Cable = Part.gameObject.GetComponentInParent<CableComponent>();

        Collider currentPart = Part;
        //transform.parent = currentPart.transform;

        int index = int.MaxValue;
        for (int i = 0; i < Cable.Points.Length; i++)
        {
            if (Cable.Points[i] == currentPart)
                index = i;
        }


        while (index > 0)
        {
            if (Vector3.Distance(transform.position, currentPart.gameObject.transform.position) < 0.1f)
            {
                index--;
                currentPart = Cable.Points[index];
                transform.parent = currentPart.transform;
            }

            Debug.Log(index);

            transform.position = Vector3.Lerp(transform.position, currentPart.transform.position, 2f * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        transform.parent = null;
        transform.position += Vector3.up * 2f;
        Climb = null;
    }
}
