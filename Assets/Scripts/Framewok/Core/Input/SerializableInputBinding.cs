using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UserAction = KeyBind.UserAction;

[System.Serializable]
public class SerializableInputBinding
{
    public BindPair[] bindPairs;

    public SerializableInputBinding(InputBinding binding)
    {
        int length = binding.Bindings.Count;
        int index = 0;

        bindPairs = new BindPair[length];

        foreach(var pair in binding.Bindings)
        {
            bindPairs[index++] = new BindPair(pair.Key, pair.Value);
        }
    }
}

[System.Serializable]
public class BindPair
{
    public UserAction key;
    public KeyCode value;

    public BindPair(UserAction key,KeyCode value)
    {
        this.key = key;
        this.value = value;
    }
}