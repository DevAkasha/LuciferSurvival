
using System;
using UnityEngine;

[Serializable]
public class KeyBindAxis : KeyBindBase
{
    [field: SerializeField, KeyBind] public KeyCode increase { get; set; }
    [field: SerializeField, KeyBind] public KeyCode decrease { get; set; }

    public KeyCode KeyCode { get; set; }
}

[Serializable]
public class KeyBindPress : KeyBindBase
{
    [field: SerializeField, KeyBind] public KeyCode KeyCode { get; set; }
    public KeyCode increase { get; set; }
    public KeyCode decrease { get; set; }
}

public interface KeyBindBase
{
    public KeyCode increase { get; set; }
    public KeyCode decrease { get; set; }
    public KeyCode KeyCode { get; set; }
}