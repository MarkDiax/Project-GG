using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using System;

public class XMLManager : MonoSingleton<XMLManager>
{
    [SerializeField]
    private bool _recordCurrentSession;

    public XMLTrigger[] xmlTriggers;
    public int player = 1;

    [HideInInspector]
    public ItemDatabase items;

    private void Awake() {
        items.data.Clear();

        for (int i = 0; i < xmlTriggers.Length; i++) {
            items.data.Add(new ItemEntry {
                triggerName = xmlTriggers[i].name
            });
        }
    }

    [System.Serializable]
    public class ItemEntry
    {
        public string triggerName;
        public float timeInSeconds;
    }

    [System.Serializable]
    public class ItemDatabase
    {
        public List<ItemEntry> data = new List<ItemEntry>();
    }

    public void SaveData() {
        for (int i = 0; i < items.data.Count; i++) 
            items.data[i].timeInSeconds = (float)Math.Round((double)items.data[i].timeInSeconds, 2);

         XmlSerializer serializer = new XmlSerializer(typeof(ItemDatabase));
         string path = Application.dataPath + "/StreamingAssets/XML/playerdata.player_" + player + ".xml";
         FileStream stream = new FileStream(path, FileMode.Create);
         serializer.Serialize(stream, items);
         Debug.Log("Saving ItemData to: " + path);
         stream.Close();
     }

     private void OnApplicationQuit() {
         if (_recordCurrentSession)
             SaveData();
     }
 }