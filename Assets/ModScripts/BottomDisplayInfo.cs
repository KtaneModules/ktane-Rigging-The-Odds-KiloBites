using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Random;

public class BottomDisplayInfo
{
    public int BuyInAmount { get; set; }
    public float JackpotValue { get; set; }
    public TimeDraw TimeOfDraw { get; set; }

    public struct TimeDraw
    {
        public int Hour { get; private set; }
        public int Minutes { get; private set; }
        public bool IsPM { get; private set; }

        public TimeDraw(int hour, int minutes, bool isPM)
        {
            Hour = hour;
            Minutes = minutes;
            IsPM = isPM;
        }
    }

    public static TimeDraw[] GetDesiredTimes()
    {
        var hours = new[] { 7, 8, 9, 10, 11, 12, 1, 2, 3 };
        var minutes = Enumerable.Range(0, 12).Select(x => x * 5).ToArray();
        var isPMOrNot = "111110000".Select(x => x == '1').ToArray();

        return Enumerable.Range(0, 9).SelectMany(x => Enumerable.Range(0, 12).Select(y => new TimeDraw(hours[x], minutes[y], isPMOrNot[x]))).ToArray();
    }

    private static readonly IEnumerable<int>[] startingJackpotValues =
    {
        Enumerable.Range(0, 1000),
        Enumerable.Range(1000, 5000),
        Enumerable.Range(5000, 10000),
        Enumerable.Range(10000, 25000),
        Enumerable.Range(25000, 50000),
        Enumerable.Range(50000, 100000)
    };

    public BottomDisplayInfo()
    {
        BuyInAmount = Range(0, 8) == 0 ? 99 : Range(0, 99);
        JackpotValue = startingJackpotValues.ToList().Shuffle().PickRandom().PickRandom(); // I know this is weird as hell, but I don't care. If it ain't broke, don't fix it.
        TimeOfDraw = GetDesiredTimes().PickRandom();
    }

    public void PerformReset(bool moreThanOneMatch)
    {
        BuyInAmount = Range(0, 8) == 0 ? 99 : Range(0, 99);

        var hour = (TimeOfDraw.Hour + 1) % 12;
        var isPm = TimeOfDraw.IsPM;

        if (hour == 0)
        {
            hour = 12;
            isPm = !isPm;
        }

        TimeOfDraw = new TimeDraw(hour, Enumerable.Range(0, 12).Select(x => x * 5).PickRandom(), isPm);

        if (moreThanOneMatch)
            JackpotValue = startingJackpotValues[0].Contains((int)JackpotValue) ? 0 : startingJackpotValues[1].Contains((int)JackpotValue) ? 1000 :
                startingJackpotValues[2].Contains((int)JackpotValue) ? 5000 : startingJackpotValues[3].Contains((int)JackpotValue) ? 10000 :
                startingJackpotValues[4].Contains((int)JackpotValue) ? 25000 : 50000;
    }
}