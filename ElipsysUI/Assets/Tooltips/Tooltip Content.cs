using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "Tooltip Content", menuName = "ScriptableObjects/Tooltip Content", order = 1)]
public class TooltipContent : ScriptableObject
{
    [SerializeField]
    private string tooltipText;

    public string tooltipContent { get { return tooltipText; } }
}
