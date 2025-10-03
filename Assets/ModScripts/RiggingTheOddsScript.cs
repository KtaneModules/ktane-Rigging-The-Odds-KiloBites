using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
	public float ButtonDepression;

	private Coroutine increaseJP, currentlyCommitting;
	private Coroutine[] ButtonAnimCoroutines;

	static int moduleIdCounter = 1;
	private float ButtonInitY;
	int moduleId;
	private bool moduleSolved;
	private bool isActivated;

	private int currentStationPosition;

	private List<Station> stations;
	private BottomDisplayInfo bottomInfo;

	private RTOPuzzle puzzle;

	void Awake()
    {
		moduleId = moduleIdCounter++;

		Module.OnActivate += Activate;

		ButtonAnimCoroutines = new Coroutine[Buttons.Length];
		ButtonInitY = Buttons[0].transform.localPosition.y;

        int i = 0;
		foreach (KMSelectable button in Buttons)
		{
			int x = i++;
			button.OnInteract += delegate () { ButtonPress(button, x); return false; };
		}
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

	void ButtonPress(KMSelectable button, int pos)
	{
		button.AddInteractionPunch();
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, button.transform);
		if (ButtonAnimCoroutines[pos] != null)
			StopCoroutine(ButtonAnimCoroutines[pos]);
		ButtonAnimCoroutines[pos] = StartCoroutine(ButtonAnim(button));


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

	private IEnumerator ButtonAnim(KMSelectable button, float duration = 0.075f)
	{
        button.transform.localPosition = new Vector3(button.transform.localPosition.x, ButtonInitY, button.transform.localPosition.z);
		float timer = 0;
		while (timer < duration)
		{
			yield return null;
			timer += Time.deltaTime;
        }
        button.transform.localPosition = new Vector3(button.transform.localPosition.x, ButtonInitY - ButtonDepression, button.transform.localPosition.z);
        timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        button.transform.localPosition = new Vector3(button.transform.localPosition.x, ButtonInitY, button.transform.localPosition.z);
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

	public void PlayBlip() => Audio.PlaySoundAtTransform("Blip", transform);

	IEnumerator Commit()
	{
		SmallDisplay.StartCommit(stations[currentStationPosition], this);
		BottomDisplay.ShowGoodLuck();
		LargeDisplay.SetDigits("---");

		yield return new WaitUntil(() => SmallDisplay.CommitCoroutine == null);

		List<Station> answerStations;

		puzzle.ProvideAnswer(bottomInfo, out answerStations);

		if (answerStations.Count == 1 && stations.Count(x => x.StationID == answerStations.First().StationID) == 1)
		{
			if (answerStations.Single().StationID == stations[currentStationPosition].StationID)
			{
				Audio.PlaySoundAtTransform("Solve", transform);
				BottomDisplay.SetCentredTexts("Congratulations!", "You won the JACKPOT!");
				moduleSolved = true;
				Module.HandlePass();
				BottomDisplay.StartFlash();
				yield return new WaitForSeconds(5);
				BottomDisplay.StopFlash();
			}
			else
			{
				Audio.PlaySoundAtTransform("Strike", transform);
				BottomDisplay.SetCentredTexts("You Lose!", $"Station {(answerStations.First().StationID + 1):00} wins JACKPOT");
				Module.HandleStrike();
				yield return new WaitForSeconds(3);
				Reset(false);
				currentlyCommitting = null;
			}
		}
		else
		{
			BottomDisplay.SetCentredTexts("No exact matches.");
			yield return new WaitForSeconds(1.5f);
			BottomDisplay.SetCentredTexts("No exact matches.", "Closest Station:");
			yield return new WaitForSeconds(1.5f);

			Coroutine useIfMoreThanThreeStations = null;

			if (answerStations.Any(x => x.StationID == stations[currentStationPosition].StationID))
			{
				Audio.PlaySoundAtTransform("Solve", transform);

				if (answerStations.Count > 3)
					useIfMoreThanThreeStations = StartCoroutine(ShowMoreThan3Stations(answerStations.OrderBy(x => x.StationID).Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3).Select(x => x.Select(v => v.Value).ToArray()).ToList(), true));
				else
					BottomDisplay.SetCentredTexts("You Win!", $"Closest Station: {answerStations.Select(x => (x.StationID + 1).ToString("00")).Join(",")}");

				moduleSolved = true;
				Module.HandlePass();
			}
			else
			{
				Audio.PlaySoundAtTransform("Strike", transform);

                if (answerStations.Count > 3)
                    useIfMoreThanThreeStations = StartCoroutine(ShowMoreThan3Stations(answerStations.OrderBy(x => x.StationID).Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / 3).Select(x => x.Select(v => v.Value).ToArray()).ToList(), false));
                else
                    BottomDisplay.SetCentredTexts("You Lose...", $"Closest Station: {answerStations.Select(x => (x.StationID + 1).ToString("00")).Join(",")}");

				Module.HandleStrike();

				yield return new WaitForSeconds(3);

				if (useIfMoreThanThreeStations != null)
					StopCoroutine(useIfMoreThanThreeStations);

				Reset(answerStations.Count() > 1);
				currentlyCommitting = null;
            }
		}
	}

	IEnumerator ShowMoreThan3Stations(List<Station[]> stationGroups, bool hasWon)
	{
		while (true)
			foreach (var stationGroup in stationGroups)
			{
                BottomDisplay.SetCentredTexts(hasWon ? "You Win!" : "You Lose...", $"Closest Station: {stationGroup.Select(x => (x.StationID + 1).ToString("00")).Join(",")}");
				yield return new WaitForSeconds(0.5f);
            }			
	}

	void Reset(bool moreThanOneInstance)
	{
		bottomInfo.PerformReset(moreThanOneInstance);

		stations.ForEach(x => x.IsAlreadySeen = false);
		stations.First(x => x.IsStartingStation).IsStartingStation = false;
		stations[currentStationPosition].IsStartingStation = true;
		puzzle = new RTOPuzzle(stations);
		puzzle.DetermineKeyStation();

		TopDisplay.SetStation(stations[currentStationPosition]);
		LargeDisplay.SetDigits(stations[currentStationPosition].Digits.Join(""));
		BottomDisplay.SetBuyIn(bottomInfo.BuyInAmount);
		BottomDisplay.SetJackpot(bottomInfo.JackpotValue);
		BottomDisplay.SetTimeOfDraw(bottomInfo.TimeOfDraw.Hour, bottomInfo.TimeOfDraw.Minutes, bottomInfo.TimeOfDraw.IsPM);
		SmallDisplay.ResetAllDisplays();

		increaseJP = StartCoroutine(IncreaseJackpot());
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





