using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BottomDisplay : MonoBehaviour
{
    public Text TextA;
    public Text TextAGlow;
    public Text TextB;
    public Text TextBGlow;
    public Text TextC, TextCGlow;
    public Text TextD, TextDGlow;
    public string TimeOfDrawPrefix;
    public string BuyInPrefix;
    public string JackpotPrefix;
    public string GoodLuck;

    private string TextAPartA;
    private string TextAPartB;

    private Coroutine FlashCoroutine;

    public void SetTimeOfDraw(int hours, int minutes, bool isPM)
    {
        HideCentredTexts();
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

    public void ShowGoodLuck()
    {
        TextAPartA = "\n";
        TextC.text = TextCGlow.text = GoodLuck + "\n";
        AssignA();
    }

    public void SetJackpot(float jackpot)
    {
        TextB.text = TextBGlow.text = "\n" + JackpotPrefix + jackpot.ToString("N1");
    }

    public void SetCentredTexts(string c, string d = "")
    {
        TextA.text = TextAGlow.text = TextB.text = TextBGlow.text = "";

        TextC.text = TextCGlow.text = c + "\n";
        TextD.text = TextDGlow.text = "\n" + d;
    }

    public void StartFlash() => FlashCoroutine = StartCoroutine(FlashText());

    public void StopFlash()
    {
        if (FlashCoroutine == null)
            return;
        StopCoroutine(FlashCoroutine);
        FlashCoroutine = null;

        TextC.enabled = TextCGlow.enabled = TextD.enabled = TextDGlow.enabled = true;
    }

    private IEnumerator FlashText()
    {
        while (true)
        {
            TextC.enabled = TextCGlow.enabled = false;
            TextD.enabled = TextDGlow.enabled = false;
            yield return new WaitForSeconds(0.2f);
            TextC.enabled = TextCGlow.enabled = true;
            TextD.enabled = TextDGlow.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void HideCentredTexts()
    {
        TextC.text = TextCGlow.text = TextD.text = TextDGlow.text = "";
    }
}
