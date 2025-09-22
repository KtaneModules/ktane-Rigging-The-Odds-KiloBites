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

    public int CombinedDigits() => int.Parse(Digits.Join(""));
}
