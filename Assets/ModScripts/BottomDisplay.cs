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
    public Text TextA2, TextA2Glow;
    public Text TextB2, TextB2Glow;
    public string TimeOfDrawPrefix;
    public string BuyInPrefix;
    public string JackpotPrefix;

    private string TextAPartA;
    private string TextAPartB;

    private Coroutine flashRoutine;

    public void SetTimeOfDraw(int hours, int minutes, bool isPM)
    {
        Hide2Texts();
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
        TextA2.text = TextA2Glow.text = "Good Luck!\n";
        AssignA();
    }

    public void Set2Text(string a, string b = null)
    {
        if (new[] { TextA.text, TextAGlow.text, TextB.text, TextBGlow.text }.Any(x => x.Length != 0))
        {
            TextA.text = TextAGlow.text = string.Empty;
            TextB.text = TextBGlow.text = string.Empty;
        }

        TextA2.text = TextA2Glow.text = a + "\n";
        TextB2.text = TextB2Glow.text = "\n" + b ?? string.Empty;
    }

    public void StartFlash() => flashRoutine = StartCoroutine(FlashText());

    public void StopFlash()
    {
        if (flashRoutine == null)
            return;

        StopCoroutine(flashRoutine);

        flashRoutine = null;

        TextA2.enabled = TextA2Glow.enabled = true;
        TextB2.enabled = TextB2Glow.enabled = true;
    }

    private IEnumerator FlashText()
    {
        while (true)
        {
            TextA2.enabled = TextA2Glow.enabled = false;
            TextB2.enabled = TextB2Glow.enabled = false;
            yield return new WaitForSeconds(0.2f);
            TextA2.enabled = TextA2Glow.enabled = true;
            TextB2.enabled = TextB2Glow.enabled = true;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void Hide2Texts()
    {
        TextA2.text = TextA2Glow.text = string.Empty;
        TextB2.text = TextB2Glow.text = string.Empty;
    }

    public void SetJackpot(float jackpot)
    {
        TextB.text = TextBGlow.text = "\n" + JackpotPrefix + jackpot.ToString("N1");
    }
}
