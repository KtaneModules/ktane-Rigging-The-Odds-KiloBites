using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TopDisplay : MonoBehaviour
{
    public Text TextA;
    public Text TextAGlow;
    public Text TextB;
    public Text TextBGlow;

    public void SetStation(Station station)
    {
        TextA.text = TextAGlow.text = $"{(station.IsStartingStation ? "STARTING" : "")}\nSTATION";
        TextB.text = TextBGlow.text = (station.StationID + 1).ToString("00");
    }
}