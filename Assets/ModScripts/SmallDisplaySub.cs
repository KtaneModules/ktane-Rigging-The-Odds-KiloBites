using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SmallDisplaySub : MonoBehaviour
{
    public Text TextOver;
    public Text TextShadow;

    public void SetCharacter(int? number = null) => TextOver.text = TextShadow.text = number.ToString() ?? "?";
}