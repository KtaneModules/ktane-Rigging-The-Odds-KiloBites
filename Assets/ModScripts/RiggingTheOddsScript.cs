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

	static int moduleIdCounter = 1;
	int moduleId;
	private bool moduleSolved;

	private int currentStationPosition;

	private List<Station> stations;

	void Awake()
    {

		moduleId = moduleIdCounter++;

		/*
		foreach (KMSelectable button in Buttons)
			button.OnInteract() += delegate () { ButtonPress(button); return false; };
		*/

		//Button.OnInteract += delegate () { ButtonPress(); return false; };
    }

	
	void Start()
    {
		stations = StationGenerator.GenerateStations();
		currentStationPosition = stations.IndexOf(x => x.IsStartingStation);

		TopDisplay.SetStation(stations[currentStationPosition]);
    }

	public void PlayBlip()
	{

	}


	
	
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





