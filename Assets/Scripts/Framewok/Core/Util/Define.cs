using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public static string StaticDataPath = "StaticData/";

    public enum SceneType
    {
        Loading,
    }

    public enum Layer
    {
        Water = 4,
        UI,
        Player,
        HideObject,
        Ground,
        Grabbable,
        Car,
        Bobber,
        IgnorePhysics = 30
    }

    public enum ItemType
    {
        Object,
    }

    public enum AtlasType
    {

    }

    public enum GrabbedObjectTag
    {
        Gun = 0,
        None
    }
}

public class KeyBind
{
    public enum UserAction
    {
        MoveForward,
        MoveBackward,
        MoveLeft,
        MoveRight,

        Attack,
        Run,
        Jump,

        UI_Inventory,
        UI_Status,
        UI_Skill,
    }
}
