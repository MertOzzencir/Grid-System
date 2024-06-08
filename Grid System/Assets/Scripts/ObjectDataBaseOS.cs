using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ObjectDataBaseOS : ScriptableObject
{

    public List<ObjectData> objectsData;


}

[Serializable]
public class ObjectData
{
    [field:SerializeField]
    public string Name { get; set; }
   
    [field: SerializeField]
    public int ID { get; set; }
   
    [field: SerializeField]
    public Vector2Int Size { get; set; } = Vector2Int.zero;
    
    [field: SerializeField]
    public GameObject Prefab{ get; set; }
}
