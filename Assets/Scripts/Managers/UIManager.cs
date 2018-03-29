using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private GameObject[] _showOnLoad;

    public List<UIObject> UIStack = new List<UIObject>();
    [SerializeField]
    private List<UIObject> _cache = new List<UIObject>();


    /*
     * loadobject instantiates an object in uistack and adds it to cache
     * 
     * 
     * 
     * 
     * */

    public void LoadUI(UIObject Object) {
        if (!_cache.Contains(Object)) {

        }
    }
}