using UnityEngine;
using System.Collections;

public class SimpleRope : MonoBehaviour
{
    LineRenderer _line;
    Transform[] _positions = new Transform[2];

    private void Start() {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = _positions.Length;
        _positions[0] = transform.GetChild(0);
        _positions[1] = transform.GetChild(1);
    }

    private void Update() {

        UpdateLine();
    }

    private void UpdateLine() {

        for (int i = 0; i < _line.positionCount; i++) {
            _line.SetPosition(i, _positions[i].position);
        }
    }
}
