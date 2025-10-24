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

	static int moduleIdCounter = 1, rtoIdCounter = 1;
	private float ButtonInitY;
	int moduleId, rtoId;
	private bool moduleSolved;
	private bool isActivated;

	private int currentStationPosition;

	private List<Station> stations;
	private BottomDisplayInfo bottomInfo;

	private KMAudio.KMAudioRef sound;

	private RTOPuzzle puzzle;

	void Awake()
    {
		moduleId = moduleIdCounter++;
		rtoId = rtoIdCounter++;

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

		Log($"[Rigging the Odds #{moduleId}] {bottomInfo}");

		puzzle = new RTOPuzzle(stations, this);

		puzzle.DetermineKeyStation();
		puzzle.PredetermineAnswerLog(bottomInfo);
    }

	void OnDestroy()
	{
        sound?.StopSound();
		rtoIdCounter = 1;
    }

	public void LogPuzzle(object args) => Log($"[Rigging the Odds #{moduleId}] {args}");

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
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, Mathf.Lerp(ButtonInitY, ButtonInitY - ButtonDepression, timer / duration), button.transform.localPosition.z);
        }
        button.transform.localPosition = new Vector3(button.transform.localPosition.x, ButtonInitY - ButtonDepression, button.transform.localPosition.z);
        timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            button.transform.localPosition = new Vector3(button.transform.localPosition.x, Mathf.Lerp(ButtonInitY - ButtonDepression, ButtonInitY, timer / duration), button.transform.localPosition.z);
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
		LargeDisplay.IdIsOne = rtoId == 1;
		LargeDisplay.SetDigits(stations[currentStationPosition].Digits.Join(""));
		increaseJP = StartCoroutine(IncreaseJackpot());
	}

	public void PlayBlip() => Audio.PlaySoundAtTransform("Blip", transform);

	IEnumerator Commit()
	{
        sound = Audio.PlaySoundAtTransformWithRef("commit music", transform);
        SmallDisplay.StartCommit(puzzle.ObtainWinningNumber(bottomInfo), this);
		BottomDisplay.ShowGoodLuck();

		if (LargeDisplay.DigitString == "qqq")
			LargeDisplay.SetDigits(stations[currentStationPosition].Digits.Join(""));

		yield return new WaitUntil(() => SmallDisplay.CommitCoroutine == null);

		sound?.StopSound();

        List<Station> answerStations;

		puzzle.ProvideAnswer(bottomInfo, true, out answerStations);

		if (puzzle.HasExactMatch)
		{
			if (answerStations.Single().StationID == stations[currentStationPosition].StationID)
			{
				Audio.PlaySoundAtTransform("Solve", transform);
				Log($"[Rigging the Odds #{moduleId}] You won the jackpot! Solved!");
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
                Log($"[Rigging the Odds #{moduleId}] You lost the jackpot! Strike!");
                BottomDisplay.SetCentredTexts("You Lose...", $"Station {(answerStations.First().StationID + 1):00} wins JACKPOT");
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

                Log($"[Rigging the Odds #{moduleId}] The station selected won! Solved!");

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

                Log($"[Rigging the Odds #{moduleId}] The station selected lost! Strike!");

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
        Log($"[Rigging the Odds #{moduleId}] Reset has been made.");
        Log($"[Rigging the Odds #{moduleId}] {bottomInfo}");

		
		BottomDisplay.SetBuyIn(bottomInfo.BuyInAmount);
		BottomDisplay.SetJackpot(bottomInfo.JackpotValue);
		BottomDisplay.SetTimeOfDraw(bottomInfo.TimeOfDraw.Hour, bottomInfo.TimeOfDraw.Minutes, bottomInfo.TimeOfDraw.IsPM);
		SmallDisplay.ResetAllDisplays();

        stations.ForEach(x => x.IsAlreadySeen = false);
        stations.First(x => x.IsStartingStation).IsStartingStation = false;
        stations[currentStationPosition].IsStartingStation = true;
        TopDisplay.SetStation(stations[currentStationPosition]);
        puzzle = new RTOPuzzle(stations, this);
        puzzle.DetermineKeyStation();
		puzzle.PredetermineAnswerLog(bottomInfo);

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

	private bool CheckRange(int start, int end, int jp) => jp >= start && jp <= end;

	private float GetSeconds(int jp) => CheckRange(0, 1000, jp) ? 0.0625f : CheckRange(1000, 5000, jp) ? 0.0152f :
		CheckRange(5000, 10000, jp) ? 0.12f : CheckRange(10000, 25000, jp) ? 0.004f : CheckRange(25000, 50000, jp) ? 0.0024f : 0.0013f;

	// Twitch Plays


#pragma warning disable 414
	private readonly string TwitchHelpMessage = @"!{0} station 25 [tunes to that specified station] || !{0} commit [commits to the station selected]";
#pragma warning restore 414

	IEnumerator ProcessTwitchCommand(string command)
    {
		string[] split = command.ToUpperInvariant().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

		if (currentlyCommitting != null)
		{
			yield return "sendtochaterror You cannot interact with the module while a commit is occurring!";
			yield break;
		}

		switch (split[0])
		{
			case "STATION":
				if (split.Length == 1)
				{
					yield return "sendtochaterror Please specify what station to tune into!";
					yield break;
				}

				if (split.Length > 2)
				{
					yield return "sendtochaterror Too many parameters!";
					yield break;
				}

				if (split[1].Length > 2 || !split[1].All(char.IsDigit))
				{
					yield return $"sendtochaterror {split[1]} isn't a valid station number!";
					yield break;
				}

				if (!Enumerable.Range(0, 30).Contains(int.Parse(split[1]) - 1))
				{
					yield return "sendtochaterror Make sure the station you're tuning into is in the range of 1-30 inclusive!";
					yield break;
				}

				

				yield return null;

                var target = int.Parse(split[1]) - 1;

				var presses = CalculatePressesForTP(currentStationPosition, target);

				foreach (var ix in presses)
				{
					Buttons[ix].OnInteract();
					yield return new WaitForSeconds(0.8f);
				}
                yield break;
			case "COMMIT":
				if (split.Length > 1)
				{
					yield return "sendtochaterror Too many parameters!";
					yield break;
				}

				yield return null;

				Buttons[2].OnInteract();
				yield return new WaitForSeconds(0.1f);

				yield return "solve";

				yield break;
			default:
				yield return "sendtochaterror The command you inputted is invalid!";
				yield break;
		}
    }

	private List<int> CalculatePressesForTP(int start, int target)
	{
		var currentStationNumber = start;
		var list = new List<int>();

		while (target != currentStationNumber)
		{
			var current = currentStationNumber;
			var distance = (Math.Abs(current - target) + 10) % 30 - 10;

			if (current > target)
				distance *= -1;

			if (distance > 4)
			{
				list.Add(4);
				currentStationNumber += 5;
			}
			else if (distance > 0)
			{
				list.Add(3);
				currentStationNumber++;
			}
			else if (distance < -4)
			{
				list.Add(0);
				currentStationNumber -= 5;
			}
			else if (distance < 0)
			{
				list.Add(1);
				currentStationNumber--;
			}
			currentStationNumber = (currentStationNumber + 30) % 30;
		}

		return list;
	}

	IEnumerator TwitchHandleForcedSolve()
    {
		while (!isActivated || currentlyCommitting != null)
		{
			if (moduleSolved)
				yield break;

			yield return true;
		}

		if (increaseJP != null)
		{
			StopCoroutine(increaseJP);
			increaseJP = null;
		}

		yield return null;

		List<Station> possibleStations;

		puzzle.ProvideAnswer(bottomInfo, false, out possibleStations);

		var determinedTarget = possibleStations.Select(x => x.StationID).PickRandom();

		var path = CalculatePressesForTP(currentStationPosition, determinedTarget);

		foreach (var ix in path)
		{
			Buttons[ix].OnInteract();
			yield return new WaitForSeconds(0.5f);
		}

		Buttons[2].OnInteract();
		yield return new WaitForSeconds(0.1f);

		while (!moduleSolved)
			yield return true;
    }


}