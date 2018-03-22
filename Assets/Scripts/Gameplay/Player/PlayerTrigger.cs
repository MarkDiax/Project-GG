#region Copyright Notice
// For use of this code, please contact the company or creator of the code.
// Creator: Mark Offenberg
// Copyright (c) 2018 Despresso Games - All Rights Reserved
#endregion

using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class PlayerTrigger : MonoBehaviour
{
    public delegate void OnRopeTrigger(RopePart ropePart);
    public event OnRopeTrigger onRopeTrigger;

    private void OnTriggerEnter(Collider other) {
        if (other.tag != Player.Instance.tag) {
            if (other.gameObject.layer == (int)Layers.Rope) {

                if (onRopeTrigger != null)
                    onRopeTrigger(other.GetComponent<RopePart>());
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.tag != Player.Instance.tag) {
            if (other.gameObject.layer == (int)Layers.Rope) {

                if (onRopeTrigger != null)
                    onRopeTrigger(other.GetComponent<RopePart>());
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag != Player.Instance.tag) {
            if (other.gameObject.layer == (int)Layers.Rope) {

                if (onRopeTrigger != null)
                    onRopeTrigger(null);
            }
        }
    }
}