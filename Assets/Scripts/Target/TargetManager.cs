using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoSingleton<TargetManager>
{
    public List<TargetConfig> Targets;

    public TargetConfig GetLevel(int i)
    {
        return Targets[i - 1];
    }
    public int MaxLevel()
    {
        return Targets.Count;
    }
}
