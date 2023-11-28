using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType { None, king, danghagwan, sumoonjang, muyeah, dangsanggwan_1, dangsanggwan_2 }

[CreateAssetMenu(fileName = "NPC Action Data", menuName = "Scriptable Object/NPC Action Data", order = int.MaxValue)]
public class NPCActionData : ScriptableObject
{
    #region Data
    [SerializeField] private NPCType npcType;
    [SerializeField] Dictionary<NPCType, string> test;
    #endregion

    #region Property
    public NPCType NPCType { get { return npcType; } }
    #endregion
}