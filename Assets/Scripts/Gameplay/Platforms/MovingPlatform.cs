using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    [SerializeField]
    public Transform platform;

    public Transform startPosition;
    public Transform endPosition;

    public  float platformSpeed;
    private Vector3 direction;
    Transform destination;

	// Use this for initialization
	void Start () {
        SetDestination(startPosition);
	}
	

	void FixedUpdate () {
        platform.transform.Translate(direction * platformSpeed * Time.fixedDeltaTime, Space.World);

        if(Vector3.Distance (platform.position, destination.position) < platformSpeed * Time.fixedDeltaTime)
        {
            SetDestination(destination == startPosition ? endPosition : startPosition);
        }

        if (Physics.Raycast(platform.position, -transform.up, 1f))
        {
            platformSpeed = 0.0f;
        }
        else
        {
            platformSpeed = 2f;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startPosition.position, startPosition.localScale);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(endPosition.position, endPosition.localScale);

    }

    void SetDestination(Transform dest)
    {
        destination = dest;
        direction = (destination.position - platform.position).normalized;
    }

}
