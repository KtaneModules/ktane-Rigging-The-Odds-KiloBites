using System;
using System.Collections.Generic;
using System.Linq;
using static UnityEngine.Random;

public class BottomDisplayInfo
{
    public int BuyInAmount { get; set; }
    public float JackpotValue { get; set; }


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

    public BottomDisplayInfo()
    {
        // Todo: Set up values based off of the percentage chance.


    }
}