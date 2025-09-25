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
	public TopDisplay TopDisplay;
	public LargeDisplay LargeDisplay;
	public SmallDisplay SmallDisplay;
	public BottomDisplay BottomDisplay;
	public KMSelectable[] Buttons;

	private Coroutine increaseJP, currentlyCommitting;

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;
	private bool isActivated;

	private int currentStationPosition;

	private List<Station> stations;
	private BottomDisplayInfo bottomInfo;

	private List<Station> answeredStations;

	private RTOPuzzle puzzle;

	void Awake()
    {

		moduleId = moduleIdCounter++;

		Module.OnActivate += Activate;

		foreach (KMSelectable button in Buttons)
			button.OnInteract += delegate () { ButtonPress(button); return false; };
    }

	
	void Start()
    {
		stations = StationGenerator.GenerateStations();
		currentStationPosition = stations.IndexOf(x => x.IsStartingStation);

		TopDisplay.SetStation(stations[currentStationPosition]);
		bottomInfo = new BottomDisplayInfo();

		BottomDisplay.SetJackpot(bottomInfo.JackpotValue);
		BottomDisplay.SetBuyIn(bottomInfo.BuyInAmount + 1);
		BottomDisplay.SetTimeOfDraw(bottomInfo.TimeOfDraw.Hour, bottomInfo.TimeOfDraw.Minutes, bottomInfo.TimeOfDraw.IsPM);

		puzzle = new RTOPuzzle(stations);

		puzzle.DetermineKeyStation();
    }

	void ButtonPress(KMSelectable button)
	{
		button.AddInteractionPunch(0.4f);
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);

		if (moduleSolved || !isActivated || currentlyCommitting != null)
			return;

		switch (Array.IndexOf(Buttons, button))
		{
			case 0:
				currentStationPosition = (currentStationPosition - 5 + 30) % 30;
				MarkStation();
				break;
			case 1:
				currentStationPosition = (currentStationPosition - 1 + 30) % 30;
				MarkStation();
				break;
			case 2:
				if (increaseJP != null)
					StopCoroutine(increaseJP);

				currentlyCommitting = StartCoroutine(Commit());
				break;
			case 3:
				currentStationPosition++;
				currentStationPosition %= 30;
				MarkStation();
				break;
			case 4:
				currentStationPosition += 5;
				currentStationPosition %= 30;
				MarkStation();
				break;

		}
	}

	void MarkStation()
	{
		TopDisplay.SetStation(stations[currentStationPosition]);
		LargeDisplay.SetDigits(stations[currentStationPosition].IsAlreadySeen ? "qqq" : stations[currentStationPosition].Digits.Join(""));

		if (stations[currentStationPosition].IsStartingStation || stations[currentStationPosition].IsAlreadySeen)
			return;

		stations[currentStationPosition].IsAlreadySeen = true;
	}

	void Activate()
	{
		isActivated = true;
		LargeDisplay.SetDigits(stations[currentStationPosition].Digits.Join(""));
		increaseJP = StartCoroutine(IncreaseJackpot());
	}

	public void PlayBlip()
	{

	}

	IEnumerator Commit()
	{
		SmallDisplay.StartCommit(stations[currentStationPosition], this);

		yield return new WaitUntil(() => SmallDisplay.CommitCoroutine == null);

		IEnumerable<Station> answerStations;

		puzzle.ProvideAnswer(bottomInfo, out answerStations);

		answeredStations = answerStations.ToList();
	}
	
	IEnumerator IncreaseJackpot()
	{
		while (true)
		{
			if (bottomInfo.JackpotValue == 500000)
				break;

			bottomInfo.JackpotValue += 0.1f;
			BottomDisplay.SetJackpot(bottomInfo.JackpotValue);
			yield return new WaitForSeconds(GetSeconds((int)bottomInfo.JackpotValue));
		}

		increaseJP = null;
	}

	private float GetSeconds(int jp) => Enumerable.Range(0, 1000).Contains(jp) ? 1.6f : Enumerable.Range(1000, 5000).Contains(jp) ? 6.6f :
		Enumerable.Range(5000, 10000).Contains(jp) ? 8.3f : Enumerable.Range(10000, 25000).Contains(jp) ? 25 : Enumerable.Range(25000, 50000).Contains(jp) ? 41.6f : 75;
	
	
	void Update()
    {

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





