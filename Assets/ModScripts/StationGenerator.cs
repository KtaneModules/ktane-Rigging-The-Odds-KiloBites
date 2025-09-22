using System.Collections.Generic;
using System.Linq;

public static class StationGenerator
{
    public static List<Station> GenerateStations()
    {
        var finalList = new List<Station>();

        var usedNumbers = new List<int>();

        for (int i = 0; i < 30; i++)
        {
            var uniqueNumber = Enumerable.Range(0, 1000).Where(x => !usedNumbers.Contains(x)).PickRandom();

            usedNumbers.Add(uniqueNumber);

            finalList.Add(new Station(i, uniqueNumber.ToString().PadLeft(3, '0').Select(x => x - '0').ToArray()));
        }

        finalList.PickRandom().IsStartingStation = true;

        return finalList;
    }
}
