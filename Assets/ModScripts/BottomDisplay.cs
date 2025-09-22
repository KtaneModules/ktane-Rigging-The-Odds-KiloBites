using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomDisplay : MonoBehaviour
{
    public Text TextA;
    public Text TextAGlow;
    public Text TextB;
    public Text TextBGlow;
    public string TimeOfDrawPrefix;
    public string BuyInPrefix;
    public string JackpotPrefix;

    private string TextAPartA;
    private string TextAPartB;

    public void SetTimeOfDraw(int hours, int minutes, bool isPM)
    {
        TextAPartA = TimeOfDrawPrefix + hours.ToString("00") + ":" + minutes.ToString("00") + (isPM ? "pm" : "am") + "\n";
        AssignA();
    }

    public void SetBuyIn(int buyIn)
    {
        TextAPartB = BuyInPrefix + (buyIn == 100 ? "MAX" : buyIn.ToString());
        AssignA();
    }

    private void AssignA()
    {
        if (TextAPartA != null && TextAPartB != null)
            TextA.text = TextAGlow.text = TextAPartA + TextAPartB;
    }

    public void SetJackpot(float jackpot)
    {
        TextB.text = TextBGlow.text = "\n" + JackpotPrefix + jackpot.ToString("N1");
    }
}
