using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NPCType { None, king, danghagwan, sumoonjang, muyeah, dangsanggwan_1, dangsanggwan_2 }
public enum ActionType { None, Narration, Animation }
public enum TriggerType { None, PlaySound }

[CreateAssetMenu(fileName = "NPC Action Data", menuName = "Scriptable Object/NPC Action Data", order = int.MaxValue)]
public class NPCActionData : ScriptableObject
{
    #region Data
    [SerializeField] private NPCType npcType;
    [SerializeField] private List<ActionData> actionDatas;
    #endregion

    #region Property
    public NPCType NPCType => npcType;
    //public IEnumerable<ActionData> ActionDatas => actionDatas.GetEnumerator();
    #endregion
}

[Serializable]
internal class ActionData
{
    #region Data
    [SerializeField] private ActionType actionType;
    [SerializeField] private string key;
    [SerializeField] private List<TimeTriggerData> triggers;
    #endregion

    #region Property
    public ActionType ActionType => actionType;
    public string Key => key;
    //public IEnumerable<ActionData> TimeTriggers => triggers.GetEnumerator();
    #endregion
}

[Serializable]
internal class TimeTriggerData
{
    #region Data
    [SerializeField] private TriggerType triggerType;
    [SerializeField] private float triggerTime;
    [SerializeField] private string key;
    #endregion

    #region Property
    public TriggerType TriggerType => triggerType;
    public float TriggerTime => triggerTime;
    public string Key => key;
    #endregion
}