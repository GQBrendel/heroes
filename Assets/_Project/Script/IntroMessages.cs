using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Intro Message", menuName = "IntroMessage")]
public class IntroMessages : ScriptableObject
{
    [TextArea(5,15)]
    public List<string> Messages;
}
