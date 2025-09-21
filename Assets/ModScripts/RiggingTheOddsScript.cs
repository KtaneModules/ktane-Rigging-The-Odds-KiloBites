using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using static UnityEngine.Random;
using static UnityEngine.Debug;

public class RiggingTheOddsScript : MonoBehaviour {

	public KMBombInfo Bomb;
	public KMAudio Audio;
	public KMBombModule Module;
	public LargeDisplay LargeDisplay;
	public BottomDisplay BottomDisplay;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	void Awake()
    {

		moduleId = moduleIdCounter++;

		/*
		foreach (KMSelectable button in Buttons)
			button.OnInteract() += delegate () { ButtonPress(button); return false; };
		*/

		//Button.OnInteract += delegate () { ButtonPress(); return false; };

		Module.OnActivate += delegate { StartCoroutine(Tester()); };

        BottomDisplay.SetTimeOfDraw(16, 45, false);
        BottomDisplay.SetBuyIn(350);
        BottomDisplay.SetJackpot(12345.45f);
    }

	
	void Start()
    {

    }
	
	
	void Update()
    {

    }

	private IEnumerator Tester()
	{
		yield return new WaitForSeconds(1f);
		LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(2f);
        LargeDisplay.SetDigits("456");
        yield return new WaitForSeconds(2f);
        LargeDisplay.SetDigits("789");
        yield return new WaitForSeconds(2f);
        LargeDisplay.SetDigits("qqq");
        yield return new WaitForSeconds(2f);
        LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(0.5f);
        LargeDisplay.SetDigits("qqq");
        yield return new WaitForSeconds(0.4f);
        LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(0.3f);
        LargeDisplay.SetDigits("qqq");
        yield return new WaitForSeconds(0.2f);
        LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(0.2f);
        LargeDisplay.SetDigits("qqq");
        yield return new WaitForSeconds(0.2f);
        LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(0.1f);
        LargeDisplay.SetDigits("qqq");
        yield return new WaitForSeconds(0.1f);
        LargeDisplay.SetDigits("123");
        yield return new WaitForSeconds(0.1f);
        LargeDisplay.SetDigits("qqq");
    }

	// Twitch Plays


#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} something";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
    {
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
		yield return null;
    }

	IEnumerator TwitchHandleForcedSolve()
    {
		yield return null;
    }


}





