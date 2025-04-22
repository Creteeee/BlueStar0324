using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmitterDataList_SO",menuName = "Inventory/EmitterDataList")]
public class EmitterDataList_SO : ScriptableObject
{
    public List<EmitterDetails> EmitterDataList;
}
