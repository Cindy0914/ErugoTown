using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UserAction = KeyBind.UserAction;

public class InputBinding
{
    private Dictionary<UserAction, KeyCode> _bindingDict;
    public Dictionary<UserAction, KeyCode> Bindings => _bindingDict;

    public InputBinding(bool initialize = true)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>();

        if (initialize)
            ResetAll();
    }

    public InputBinding(SerializableInputBinding sib)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>();

        foreach(var pair in sib.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }

    public void ApplyNewBindings(InputBinding newBinding)
    {
        _bindingDict = new Dictionary<UserAction, KeyCode>(newBinding._bindingDict);
    }

    public void ApplyNewBindings(SerializableInputBinding newBinding)
    {
        _bindingDict.Clear();

        foreach (var pair in newBinding.bindPairs)
        {
            _bindingDict[pair.key] = pair.value;
        }
    }

    public void Bind(in UserAction action, in KeyCode code, bool allowOverlap = false)
    {
        if (!allowOverlap && _bindingDict.ContainsValue(code))
        {
            var copy = new Dictionary<UserAction, KeyCode>(_bindingDict);

            foreach (var pair in copy)
            {
                if(pair.Value.Equals(code))
                {
                    _bindingDict[pair.Key] = KeyCode.None;
                }
            }
        }
        _bindingDict[action] = code;
    }

    public void ResetAll()
    {
        Bind(UserAction.MoveForward, KeyCode.W);
        Bind(UserAction.MoveBackward, KeyCode.S);
        Bind(UserAction.MoveLeft, KeyCode.A);
        Bind(UserAction.MoveRight, KeyCode.D);

        /*
        Bind(KeyBind.UserAction.Run, KeyCode.LeftShift);
        Bind(KeyBind.UserAction.Jump, KeyCode.Space);
        Bind(KeyBind.UserAction.Attack, KeyCode.Mouse0);

        Bind(KeyBind.UserAction.UI_Inventory, KeyCode.I);
        Bind(KeyBind.UserAction.UI_Status, KeyCode.P);
        Bind(KeyBind.UserAction.UI_Skill, KeyCode.K);
        */
    }
}
