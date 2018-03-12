using UnityEngine;
using System.Collections;

public class PlayerActions : MonoBehaviour
{ 
    private Coroutine _climbRoutine;



    private IEnumerator ClimbRoutine(Collider Part)
    {
        CableComponent Cable = Part.gameObject.GetComponentInParent<CableComponent>();

        Collider currentPart = Part;

        int index = int.MaxValue;
        for (int i = 0; i < Cable.Colliders.Length; i++)
        {
            if (Cable.Colliders[i] == currentPart)
                index = i;
        }

        while (index > 0)
        {
            if (Vector3.Distance(transform.position, currentPart.gameObject.transform.position) < 0.1f)
            {
                index--;
                currentPart = Cable.Colliders[index];

                transform.parent = currentPart.transform;
            }

            Debug.Log(index);

            transform.position = Vector3.Lerp(transform.position, currentPart.transform.position, 2f * Time.deltaTime);

            yield return new WaitForFixedUpdate();
        }

        transform.parent = null;
        transform.position += Vector3.up * 2f;
        _climbRoutine = null;
    }
}
