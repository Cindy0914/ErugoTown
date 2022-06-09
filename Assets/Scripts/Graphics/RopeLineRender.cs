using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeLineRender : MonoBehaviour
{
    public Transform lineStart;
    public Transform lineEnd;

    LineRenderer line;

    private void Start()
    {
        TryGetComponent(out line);
    }

    void Update()
    {
        line.SetPosition(0, lineStart.position);
        line.SetPosition(1, lineEnd.position);
    }
}
