using UnityEngine;
using System;
using System.Collections;

public struct GraveInfo
{
    public string userName;
    public string deathMessage;
    public string messageId;
    public enum CurseType { None, Damage, Heal }
    public CurseType curseType;
    public Vector3 position;
    public bool isUsed;
}
