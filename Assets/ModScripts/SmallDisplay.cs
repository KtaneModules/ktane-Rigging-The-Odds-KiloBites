using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;


public class SmallDisplay : MonoBehaviour
{
    public SmallDisplaySub[] SmallSubDisplays;

    private readonly Coroutine[] smallDisplayCoroutines = new Coroutine[3];

    public Coroutine CommitCoroutine;

    void Awake()
    {
        foreach (var smallSub in SmallSubDisplays)
            smallSub.SetCharacter();
    }

    public void ResetAllDisplays()
    {
        foreach (var smallSub in SmallSubDisplays)
            smallSub.SetCharacter();
    }

    public void SetNumber(int ix, int n) => SmallSubDisplays[ix].SetCharacter(n);

    public void StartCommit(Station station, RiggingTheOddsScript module)
    {
        for (int i = 0; i < 3; i++)
            smallDisplayCoroutines[i] = StartCoroutine(CycleRandomDigits(SmallSubDisplays[i]));

        CommitCoroutine = StartCoroutine(Commit(station, module));
    }

    private IEnumerator Commit(Station station, RiggingTheOddsScript module)
    {
        var secondsPerDisplay = new[] { 1f, 1.5f, 2f };

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(secondsPerDisplay[i]);
            StopCoroutine(smallDisplayCoroutines[i]);
            SmallSubDisplays[i].SetCharacter(station.Digits[i]);
            module.PlayBlip();
        }

        CommitCoroutine = null;
    }

    private IEnumerator CycleRandomDigits(SmallDisplaySub subDisplay)
    {
        while (true)
        {
            subDisplay.SetCharacter(Range(0, 10));
            yield return new WaitForSeconds(0.05f);
        }
    }
}