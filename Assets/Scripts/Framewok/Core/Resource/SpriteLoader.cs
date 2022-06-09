using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class SpriteLoader
{
    static Dictionary<Define.AtlasType, SpriteAtlas> atlasDict = new Dictionary<Define.AtlasType, SpriteAtlas>();

    public static void SetAtlas(SpriteAtlas[] atlases)
    {
        for (int i = 0; i < atlases.Length; ++i)
        {
            var key = (Define.AtlasType)Enum.Parse(typeof(Define.AtlasType), atlases[i].name);
            atlasDict.Add(key, atlases[i]);
        }
    }

    public static Sprite GetSprite(Define.AtlasType type,string spriteKey)
    {
        if (!atlasDict.ContainsKey(type))
            return null;

        return atlasDict[type].GetSprite(spriteKey);
    }
}
