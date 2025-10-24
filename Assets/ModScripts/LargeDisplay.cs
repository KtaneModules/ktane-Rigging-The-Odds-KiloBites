using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LargeDisplay : MonoBehaviour
{
    public Image[] ImageRends;
    public Sprite[] AllSprites;
    public float HighestPosition;
    public KMAudio Audio;

    [NonSerialized]
    public string DigitString;

    [NonSerialized]
    public bool IdIsOne;

    private List<Image> CopyRends = new List<Image>();
    private Coroutine ImageShwoopCoroutine;
    private List<Coroutine> ImageMoveCoroutines = new List<Coroutine>();

    private bool[] allDigitsSet = new bool[3];
    private bool isActivated;
    private Coroutine activate;

    private void Awake()
    {
        foreach (var rend in ImageRends)
            rend.sprite = FindDigitSprite('-');
    }

    private Sprite FindDigitSprite(char digit)
    {
        var name = "number " + digit;
        var results = AllSprites.Where(x => x.name == name);
        if (results.Count() == 0) return null;
        return results.First();
    }

    public void SetDigits(string digits)
    {
        if (ImageShwoopCoroutine != null)
        {
            StopCoroutine(ImageShwoopCoroutine);
            foreach (var coroutine in ImageMoveCoroutines)
                if (coroutine != null)
                    StopCoroutine(coroutine);
            ImageMoveCoroutines = new List<Coroutine>();
            foreach (var rend in CopyRends)
                if (rend != null)
                    Destroy(rend.gameObject);
            CopyRends = new List<Image>();
        }

        for (int i = 0; i < ImageRends.Length; i++)
        {
            CopyRends.Add(Instantiate(ImageRends[i], ImageRends[i].transform.parent));
            SetImagePosition(CopyRends.Last(), 0);
            SetImagePosition(ImageRends[i], 1);
            ImageRends[i].sprite = FindDigitSprite(digits[i]);
        }

        DigitString = digits;

        ImageShwoopCoroutine = StartCoroutine(TellDigitsToMove());

        if (isActivated)
            return;

        activate = StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => allDigitsSet.All(x => x));

        isActivated = true;
        activate = null;
    }

    private IEnumerator TellDigitsToMove(float interval = 0.1f)
    {
        for (int i = 0; i < ImageRends.Length; i++)
        {
            ImageMoveCoroutines.Add(StartCoroutine(MoveDigit(i)));
            float timer = 0;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
        yield return null;
    }

    private IEnumerator MoveDigit(int ix, float duration = 0.3f)
    {
        if (IdIsOne || isActivated)
            Audio.PlaySoundAtTransform("reel fall", ImageRends[ix].transform);


        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            SetImagePosition(ImageRends[ix], Easing.InSine(timer, 1, 0, duration));
            SetImagePosition(CopyRends[ix], Easing.InSine(timer, 0, -1, duration));
        }

        if (IdIsOne || isActivated)
            Audio.PlaySoundAtTransform("reel set", ImageRends[ix].transform);

        SetImagePosition(ImageRends[ix], 0);
        Destroy(CopyRends[ix].gameObject);

        if (allDigitsSet[ix])
            yield break;

        allDigitsSet[ix] = true;

    }

    private void SetImagePosition(Image rend, float height)
    {
        rend.transform.localPosition = new Vector3(rend.transform.localPosition.x, height * HighestPosition, 0);
    }
}
