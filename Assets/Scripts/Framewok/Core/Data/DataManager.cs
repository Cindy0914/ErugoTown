using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public override bool dontDestroy { get; set; } = true;

    public JsonManager Json { get; private set; } = new JsonManager();
    public StaticDataModule SD { get; private set; } = new StaticDataModule();

    protected override void Initialize()
    {
        base.Initialize();
        SD.Init();
    }
}
