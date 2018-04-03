using UnityEngine;
using System.Collections;

public class XMLTrigger : MonoBehaviour
{
    XMLManager _xml;
    XMLManager.ItemEntry _entry;

    private void Start() {
        _xml = XMLManager.Instance;

        _entry = _xml.items.data.Find(entry => entry.triggerName == name);
        if (_entry == null)
            Debug.LogWarning("XMLSerializer is not tracking " + name + "!");
    }

    private void OnTriggerStay(Collider other) {
        if (_entry == null)
            return;

        if (other.CompareTag("Player")){
            _xml.items.data.Find(entry => entry.triggerName == name).timeInSeconds += Time.deltaTime;
        }
    }
}