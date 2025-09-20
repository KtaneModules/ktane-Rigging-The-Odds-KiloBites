public class Station
{
    public int StationID { get; private set; }
    public int[] Digits { get; private set; }

    public bool IsAlreadySeen, IsStartingStation;

    public Station(int stationID, int[] digits)
    {
        StationID = stationID;
        Digits = digits;
    }

    public int GetThreeDigitNumber() => int.Parse(Digits.Join(""));
}
