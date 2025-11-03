using System;
using System.Collections.Generic;
using System.Linq;

public class RTOPuzzle
{

    private struct DigitFinder
    {
        public int[] DigitsToCheck { get; private set; }
        public int PrimaryIndex { get; private set; }
        public int? AlternativeIndex { get; private set; }
        public int[] DigitsToCheckIxes { get; private set; }

        public DigitFinder(int[] digitsToCheck, int primaryIndex, int? alternativeIndex, int[] digitsToCheckIxes)
        {
            DigitsToCheck = digitsToCheck;
            PrimaryIndex = primaryIndex;
            AlternativeIndex = alternativeIndex;
            DigitsToCheckIxes = digitsToCheckIxes;
        }
    }

    private DigitFinder[] SetupPriority(int[] winningNumbers)
    {
        var ixBools = new[]
        {
            "xx.",
            "x.x",
            ".xx",
            "x..",
            ".x.",
            "..x"
        }.Select(x => x.Select(y => y == 'x').ToArray()).ToArray();

        var primaryIxes = new[] { 2, 1, 0, 1, 0, 0 };
        var alternativeIxes = new List<int?>();

        alternativeIxes.AddRange(Enumerable.Repeat((int?)null, 3));
        alternativeIxes.AddRange(new int?[] { 2, 2, 1 });

        return Enumerable.Range(0, 6).Select(x => new DigitFinder(Enumerable.Range(0, 3).Where(y => ixBools[x][y]).Select(y => winningNumbers[y]).ToArray(), primaryIxes[x], alternativeIxes[x], Enumerable.Range(0, 3).Where(y => ixBools[x][y]).ToArray())).ToArray();
    }

    private List<Station> _allStations;

    private readonly RiggingTheOddsScript _module;

    public bool HasExactMatch;

    public RTOPuzzle(List<Station> allStations, RiggingTheOddsScript module)
    {
        _allStations = allStations;
        _module = module;
    }

    private static readonly int?[] hexGridIxes =
    {
        28,
        10, 27,
        2, 27, 29,
        0, 13, 5, 22,
        15, 9, 23, 11, 20,
        21, 17, 3, 25,
        19, 8, 16, 26, 16,
        4, 5, 14, 1,
        13, 22, null, 1, 23,
        8, 3, 24, 25,
        2, 10, 26, 24, 4,
        21, 7, 11, 7,
        15, 6, 29, 0, 19,
        28, 18, 9, 12,
        18, 17, 14,
        20, 12,
        6
    };

    // I'll be honest, but I literally have no idea why the author of this manual wants to go north CCW instead of north CW, but it's fine. It's just gonna be weird as hell

    /*
     *           00        
     *         01  02      
     *       03  04  05    
     *     06  07  08  09  
     *   10  11  12  13  14
     *     15  16  17  18  
     *   19  20  21  22  23
     *     24  25  26  27  
     *   28  29  30  31  32
     *     33  34  35  36  
     *   37  38  39  40  41
     *     42  43  44  45  
     *   46  47  48  49  50
     *     51  52  53  54  
     *       55  56  57    
     *         58  59      
                 60        
    */

    private static readonly int[][] hexGrid =
    {
        new[] { 60, 14, 1, 4, 2, 10 },
        new[] { 58, 23, 3, 7, 4, 0 },
        new[] { 59, 0, 4, 8, 5, 19 },
        new[] { 55, 32, 6, 11, 7, 1 },
        new[] { 0, 1, 7, 12, 8, 2 },
        new[] { 57, 2, 8, 13, 9, 28 },
        new[] { 51, 41, 10, 15, 11, 3 },
        new[] { 1, 3, 11, 16, 12, 4 },
        new[] { 2, 4, 12, 17, 13, 5 },
        new[] { 54, 5, 13, 18, 14, 37 },
        new[] { 46, 50, 0, 19, 15, 6 },
        new[] { 3, 6, 15, 20, 16, 7 },
        new[] { 4, 7, 16, 21, 17, 8 },
        new[] { 5, 8, 17, 22, 18, 9 },
        new[] { 50, 9, 18, 23, 0, 46 },
        new[] { 6, 10, 19, 24, 20, 11 },
        new[] { 7, 11, 20, 25, 21, 12 },
        new[] { 8, 12, 21, 26, 22, 13 },
        new[] { 9, 13, 22, 27, 23, 14 },
        new[] { 10, 54, 2, 28, 24, 15 },
        new[] { 11, 15, 24, 29, 25, 16 },
        new[] { 12, 16, 25, 30, 26, 17 },
        new[] { 13, 17, 26, 31, 27, 18 },
        new[] { 14, 18, 27, 32, 1, 51 },
        new[] { 15, 19, 28, 33, 29, 20 },
        new[] { 16, 20, 29, 34, 30, 21 },
        new[] { 17, 21, 30, 35, 31, 22 },
        new[] { 18, 22, 31, 36, 32, 23 },
        new[] { 19, 57, 5, 37, 33, 24 },
        new[] { 20, 24, 33, 38, 34, 25 },
        new[] { 21, 25, 34, 39, 35, 26 },
        new[] { 22, 26, 35, 40, 36, 27 },
        new[] { 23, 27, 36, 41, 3, 55 },
        new[] { 24, 28, 37, 42, 38, 29 },
        new[] { 25, 29, 38, 43, 39, 30 },
        new[] { 26, 30, 39, 44, 40, 31 },
        new[] { 27, 31, 40, 45, 41, 32 },
        new[] { 28, 59, 9, 46, 42, 33 },
        new[] { 29, 33, 42, 47, 43, 34 },
        new[] { 30, 34, 43, 48, 44, 35 },
        new[] { 31, 35, 44, 49, 45, 36 },
        new[] { 32, 36, 45, 50, 6, 58 },
        new[] { 33, 37, 46, 51, 47, 38 },
        new[] { 34, 38, 47, 52, 48, 39 },
        new[] { 35, 39, 48, 53, 49, 40 },
        new[] { 36, 40, 49, 54, 50, 41 },
        new[] { 37, 60, 14, 10, 51, 42 },
        new[] { 38, 42, 51, 55, 52, 43 },
        new[] { 39, 43, 52, 56, 53, 44 },
        new[] { 40, 44, 53, 57, 54, 45 },
        new[] { 41, 45, 54, 14, 10, 60 },
        new[] { 42, 46, 23, 6, 55, 47 },
        new[] { 43, 47, 55, 58, 56, 48 },
        new[] { 44, 48, 56, 59, 57, 49 },
        new[] { 45, 49, 57, 9, 19, 50 },
        new[] { 47, 51, 32, 3, 58, 52 },
        new[] { 48, 52, 58, 60, 59, 53 },
        new[] { 49, 53, 59, 5, 28, 54 },
        new[] { 52, 55, 41, 1, 60, 56 },
        new[] { 53, 56, 60, 2, 37, 57 },
        new[] { 56, 58, 50, 0, 46, 59 }
    };

    private static WinningNumberCell[][,] WinningTables(List<int[]> usedStationNumbers, int[] kcn)
    {
        var result = new WinningNumberCell[3][,];

        var arithmeticTables = new[]
        {
            new[]
            {
                1, 0, 2, 0, 0, 0, 0, 1, 0, 1, 0, 1,
                0, 0, 2, 0, 1, 2, 0, 1, 1, 1, 1, 1,
                1, 0, 1, 1, 1, 1, 2, 0, 2, 0, 0, 0,
                2, 2, 0, 1, 1, 1, 1, 0, 1, 0, 2, 1,
                0, 2, 1, 2, 0, 2, 0, 0, 0, 0, 1, 0,
                1, 1, 0, 1, 0, 0, 1, 0, 1, 2, 0, 1
            },
            new[]
            {
                0, 0, 1, 1, 1, 2, 0, 2, 0, 0,
                1, 1, 1, 0, 1, 0, 2, 1, 0, 2,
                1, 0, 1, 0 ,0 ,0, 2, 2, 0, 0,
                1, 0, 1, 1, 0, 1, 1, 1, 1, 0,
                2, 0, 1, 2, 1, 0, 1, 0, 1, 0,
                1, 1, 0, 0, 1, 2, 0, 2, 0, 1
            },
            new[]
            {
                0, 2, 1, 0, 1, 0, 1, 2, 1, 1, 0, 1,
                0, 0, 2, 1, 2, 0, 0, 1, 0, 0, 0, 0,
                2, 2, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1,
                1, 0, 1, 1, 1, 0, 1, 0, 0, 1, 2, 1,
                0, 1, 0, 0, 2, 1, 0, 0, 1, 2, 2, 0,
                1, 0, 0, 0, 0, 2, 0, 0, 1, 1, 1, 2
            }
        }.Select(x => x.Select(y => (Arithmetic)y).ToArray()).ToArray();

        var arithmeticDigitTables = new[]
        {
            new[]
            {
                2, 3, 5, 2, usedStationNumbers[0][0], 2, usedStationNumbers[0][0], 3, 4, usedStationNumbers[0][0], 2, 4,
                4, 1, 0, 1, 2, 5, usedStationNumbers[2][0], usedStationNumbers[2][0], 1, 4, 4, 2,
                usedStationNumbers[0][0], 3, 1, 3, 4, 1, 0, 4, 0, 1, 4, 3,
                5, 5, 4, usedStationNumbers[2][0], usedStationNumbers[1][0], 3, 1, usedStationNumbers[2][0], 1, 2, 0, 3,
                usedStationNumbers[1][0], 0, 1, 0, usedStationNumbers[1][0], 5, 3, 2, 1, 1, 2, 4,
                4, 3, 2, usedStationNumbers[1][0], 3, 2, 2, 1, 3, 5, 3, 4
            },
            new[]
            {
                usedStationNumbers[1][1], 3, 3, 4, 4, 0, 2, 0, 4, 4,
                2, usedStationNumbers[2][1], 3, 1, 3, usedStationNumbers[0][1], 0, usedStationNumbers[1][1], usedStationNumbers[2][1], 0,
                2, usedStationNumbers[2][1], 4, 4, 2, 1, 5, 5, 3, usedStationNumbers[0][1],
                2, 1, 3, usedStationNumbers[0][1], 1, 4, 2, 4, 2, 2,
                5, 1, 1, 5, 4, 4, 1, 3, usedStationNumbers[2][1], 2,
                3, usedStationNumbers[1][1], 2, 4, 1, 0, 3, 5, 3, 1
            },
            new[]
            {
                2, 0, 1, 2, 4, usedStationNumbers[1][2], usedStationNumbers[1][2], 0, 2, 2, usedStationNumbers[2][2], 2,
                4, 4, 5, 1, 0, 1, 3, usedStationNumbers[1][2], 4, 1, 2, 3,
                0, 5, 4, 3, 1, 3, 1, 4, 3, usedStationNumbers[2][2], usedStationNumbers[0][2], 1,
                4, 2, 2, 3, 4, 1, 4, 3, 4, 4, 0, 1,
                2, 2, usedStationNumbers[2][2], 2, 0, usedStationNumbers[0][2], 1, usedStationNumbers[0][2], 3, 5, 5, 1,
                usedStationNumbers[2][2], usedStationNumbers[1][2], 3, usedStationNumbers[0][2], 3, 5, 4, 3, 1, 2, 3, 5
            }
        };

        for (int i = 0; i < 3; i++)
        {
            result[i] = new WinningNumberCell[6, (i % 2 == 0 ? 12 : 10)];

            for (int j = 0; j < 6; j++)
                for (int k = 0; k < (i % 2 == 0 ? 12 : 10); k++)
                    result[i][j, k] = new WinningNumberCell(arithmeticTables[i][j * (i % 2 == 0 ? 12 : 10) + k], arithmeticDigitTables[i][j * (i % 2 == 0 ? 12 : 10) + k], kcn[i]);
        }

        return result;

    }


    private Station kcnStation;
    private List<Station> usedStations = new List<Station>();

    public void DetermineKeyStation()
    {
        var startingStation = _allStations.First(x => x.IsStartingStation);
        var startingPositions = Enumerable.Range(0, 61).Where(x => hexGridIxes[x] != null).Where(x => hexGridIxes[x].Value == startingStation.StationID).ToArray();
        var startingPosition = startingPositions[startingStation.CombinedDigits() <= 500 ? 0 : 1];
        var currentPosition = startingPosition;

        HexDirection? previousDirection = null;

        _module.LogPuzzle(startingStation);
        _module.LogPuzzle($"The starting position is {(startingStation.StationID + 1):00} ({(startingStation.CombinedDigits() <= 500 ? "topmost/left" : "bottom-most/rightmost")} occurence) in the hexagonal grid");

        usedStations.Add(startingStation);

        for (int i = 0; i < 3; i++)
        {
            var hexDirections = DetermineHexDirections(i, usedStations);

            _module.LogPuzzle($"Stage {i + 1}:" + (hexDirections.Count == 0 ? "No conditions apply, move X once" : $"The following moves to be made {i + 1} time(s): {hexDirections.Join(", ")}"));

            if (hexDirections.Count == 0)
            {
                currentPosition = hexGrid[currentPosition][(int)HexDirection.X];

                if (hexGridIxes[currentPosition] == null)
                    currentPosition = hexGrid[currentPosition][(int)HexDirection.X];

                if (currentPosition == startingPosition)
                    currentPosition = hexGrid[currentPosition][(int)HexDirection.X];

                if (i == 2)
                {
                    kcnStation = _allStations[hexGridIxes[currentPosition].Value];
                    _module.LogPuzzle($"The KCN Station is: {kcnStation}");
                    break;
                }

                _module.LogPuzzle($"After moving, the next station is: {_allStations[hexGridIxes[currentPosition].Value]}");

                startingPosition = currentPosition;

                continue;
            }

            List<HexDirection> repeatDirections = new List<HexDirection>();

            if (i == 0)
                repeatDirections = hexDirections.ToList();
            else
                foreach (var dir in hexDirections)
                    repeatDirections.AddRange(Enumerable.Repeat(dir, i + 1));

            foreach (var direction in repeatDirections)
            {
                currentPosition = hexGrid[currentPosition][(int)direction];

                if (hexGridIxes[currentPosition] == null)
                    currentPosition = hexGrid[currentPosition][(int)direction];

                previousDirection = direction;
            }

            if (currentPosition == startingPosition)
                currentPosition = hexGrid[currentPosition][(int)previousDirection.Value];

            if (i == 2)
            {
                kcnStation = _allStations[hexGridIxes[currentPosition].Value];
                _module.LogPuzzle($"The KCN Station is: {kcnStation}");
                break;
            }

            usedStations.Add(_allStations[hexGridIxes[currentPosition].Value]);

            _module.LogPuzzle($"After moving, the next station is: {_allStations[hexGridIxes[currentPosition].Value]}");

            startingPosition = currentPosition;
        }
    }

    private List<HexDirection> DetermineHexDirections(int stage, List<Station> usedStations)
    {
        bool[] conditions;

        switch (stage)
        {
            case 0:
                conditions = new[]
                {
                    usedStations[0].Digits[0] % 2 == 0,
                    Math.Abs(usedStations[0].Digits[1] - usedStations[0].Digits[2]) < 5,
                    usedStations[0].Digits[0] > usedStations[0].Digits[2],
                    usedStations[0].CombinedDigits() > 499,
                    usedStations[0].Digits[1] + usedStations[0].Digits[0] > usedStations[0].Digits[2],
                    usedStations[0].Digits[2] % 2 == 1
                };
                break;
            case 1:
                conditions = new[]
                {
                    usedStations[0].Digits.Any(x => usedStations[1].Digits[0] == x),
                    usedStations[1].CombinedDigits() > usedStations[0].CombinedDigits(),
                    usedStations[1].Digits[1] > usedStations[0].Digits[1],
                    usedStations[1].Digits[2] + usedStations[1].Digits[0] > 9,
                    Math.Abs(usedStations[1].Digits[0] - usedStations[1].Digits[1]) < Math.Abs(usedStations[0].Digits[0] - usedStations[0].Digits[1]),
                    usedStations[1].Digits[2] <= usedStations[0].Digits[2]
                };
                break;
            case 2:
                conditions = new[]
                {
                    Math.Abs(usedStations[0].Digits[2] -  usedStations[1].Digits[2]) > usedStations[2].Digits[2],
                    usedStations[2].Digits[2] - usedStations[2].Digits[0] < 0,
                    usedStations[2].Digits[1] % 2 == usedStations[1].Digits[1] % 2,
                    usedStations[2].Digits[1] + usedStations[2].Digits[2] > usedStations[1].Digits[1] + usedStations[1].Digits[2],
                    usedStations[2].CombinedDigits() > usedStations[1].CombinedDigits(),
                    usedStations.Take(2).All(x => usedStations[2].Digits[0] >= x.Digits[0])
                };
                break;
            default:
                throw new ArgumentOutOfRangeException($"Stage {stage} is not an valid index. Make sure it is in the range of 0-2 inclusive.");
        }

        return Enumerable.Range(0, 6).Where(x => conditions[x]).Select(x => (HexDirection)x).ToList();
    }

    private bool CheckRange(float jp, float start, float end) => jp >= start && jp <= end;

    public int[] ObtainWinningNumber(BottomDisplayInfo info)
    {
        var minuteRef = Enumerable.Range(0, 12).Select(x => x * 5).ToArray();

        var jp = info.JackpotValue;

        var checkJackpotRange = new[]
        {
            CheckRange(jp, 0, 999.9f),
            CheckRange(jp, 1000, 4999.9f),
            CheckRange(jp, 5000, 99999.9f),
            CheckRange(jp, 10000, 24999.9f),
            CheckRange(jp, 25000, 49999.9f),
            jp >= 50000
        };

        var buyInAmountRange = new[]
        {
            info.BuyInAmount < 10,
            info.BuyInAmount < 25,
            info.BuyInAmount < 49,
            info.BuyInAmount < 74,
            info.BuyInAmount < 99,
            info.BuyInAmount == 99
        };

        var buyIn = info.BuyInAmount + 1;

        var buyInRange = new[]
        {
            10,
            20,
            30,
            40,
            50,
            60,
            70,
            80,
            90,
            100
        }.Select((x, i) => i == 9 ? buyIn <= x : buyIn < x).ToArray();

        var minuteIx = Enumerable.Range(0, 12).First(x => minuteRef[x] == info.TimeOfDraw.Minutes);
        var jpRangeIx = Enumerable.Range(0, 6).First(x => checkJackpotRange[x]);
        var buyInIx = Enumerable.Range(0, 10).First(x => buyInRange[x]);
        var firstDigitBuyInIx = Enumerable.Range(0, 6).First(x => buyInAmountRange[x]);

        var tables = WinningTables(usedStations.Select(x => x.Digits.ToArray()).ToList(), kcnStation.Digits.ToArray());

        var answerOutput = new int[3];

        for (int i = 0; i < 3; i++)
            answerOutput[i] = tables[i][(i == 0 ? firstDigitBuyInIx : jpRangeIx), (i % 2 == 0 ? minuteIx : buyInIx)].GetResultedValue;

        return answerOutput;
    }

    public void ProvideAnswer(BottomDisplayInfo info, bool showLog, out List<Station> foundStations)
    {

        HasExactMatch = false;

        var minuteRef = Enumerable.Range(0, 12).Select(x => x * 5).ToArray();

        var jp = info.JackpotValue;

        var checkJackpotRange = new[]
        {
            CheckRange(jp, 0, 999.9f),
            CheckRange(jp, 1000, 4999.9f),
            CheckRange(jp, 5000, 99999.9f),
            CheckRange(jp, 10000, 24999.9f),
            CheckRange(jp, 25000, 49999.9f),
            jp >= 50000
        };

        var buyInAmountRange = new[]
        {
            info.BuyInAmount < 10,
            info.BuyInAmount < 25,
            info.BuyInAmount < 49,
            info.BuyInAmount < 74,
            info.BuyInAmount < 99,
            info.BuyInAmount == 99
        };

        var buyIn = info.BuyInAmount + 1;

        var buyInRange = new[]
        {
            10,
            20,
            30,
            40,
            50,
            60,
            70,
            80,
            90,
            100
        }.Select((x, i) => i == 9 ? buyIn <= x : buyIn < x).ToArray();

        var minuteIx = Enumerable.Range(0, 12).First(x => minuteRef[x] == info.TimeOfDraw.Minutes);
        var jpRangeIx = Enumerable.Range(0, 6).First(x => checkJackpotRange[x]);
        var buyInIx = Enumerable.Range(0, 10).First(x => buyInRange[x]);
        var firstDigitBuyInIx = Enumerable.Range(0, 6).First(x => buyInAmountRange[x]);

        var tables = WinningTables(usedStations.Select(x => x.Digits.ToArray()).ToList(), kcnStation.Digits.ToArray());

        var answerOutput = new int[3];

        for (int i = 0; i < 3; i++)
            answerOutput[i] = tables[i][(i == 0 ? firstDigitBuyInIx : jpRangeIx), (i % 2 == 0 ? minuteIx : buyInIx)].GetResultedValue;

        foundStations = FindPrioritizedStations(answerOutput);

        if (showLog)
            _module.LogPuzzle($"The new answer(s) given with the current jackpot of {info.JackpotValue:N1} is/are: {foundStations.Select(x => (x.StationID + 1).ToString("00")).Join(", ")}");
    }

    public void PredetermineAnswerLog(BottomDisplayInfo info)
    {
        var minuteRef = Enumerable.Range(0, 12).Select(x => x * 5).ToArray();

        var jp = info.JackpotValue;

        var checkJackpotRange = new[]
        {
            CheckRange(jp, 0, 999.9f),
            CheckRange(jp, 1000, 4999.9f),
            CheckRange(jp, 5000, 99999.9f),
            CheckRange(jp, 10000, 24999.9f),
            CheckRange(jp, 25000, 49999.9f),
            jp >= 50000
        };

        var buyInAmountRange = new[]
        {
            info.BuyInAmount < 10,
            info.BuyInAmount < 25,
            info.BuyInAmount < 49,
            info.BuyInAmount < 74,
            info.BuyInAmount < 99,
            info.BuyInAmount == 99
        };

        var buyIn = info.BuyInAmount + 1;

        var buyInRange = new[]
        {
            10,
            20,
            30,
            40,
            50,
            60,
            70,
            80,
            90,
            100
        }.Select((x, i) => i == 9 ? buyIn <= x : buyIn < x).ToArray();

        var minuteIx = Enumerable.Range(0, 12).First(x => minuteRef[x] == info.TimeOfDraw.Minutes);
        var jpRangeIx = Enumerable.Range(0, 6).First(x => checkJackpotRange[x]);
        var buyInIx = Enumerable.Range(0, 10).First(x => buyInRange[x]);
        var firstDigitBuyInIx = Enumerable.Range(0, 6).First(x => buyInAmountRange[x]);

        var tables = WinningTables(usedStations.Select(x => x.Digits.ToArray()).ToList(), kcnStation.Digits.ToArray());

        var answerOutput = new int[3];

        for (int i = 0; i < 3; i++)
        {
            answerOutput[i] = tables[i][(i == 0 ? firstDigitBuyInIx : jpRangeIx), (i % 2 == 0 ? minuteIx : buyInIx)].GetResultedValue;
            _module.LogPuzzle($"{kcnStation.Digits[i]} {tables[i][(i == 0 ? firstDigitBuyInIx : jpRangeIx), (i % 2 == 0 ? minuteIx : buyInIx)]} = {tables[i][(i == 0 ? firstDigitBuyInIx : jpRangeIx), (i % 2 == 0 ? minuteIx : buyInIx)].GetResultedValue}");
        }

        _module.LogPuzzle($"The final winning number is: {answerOutput.Join("").PadLeft(3, '0')}");

        var finalStations = FindPrioritizedStations(answerOutput);

        if (HasExactMatch)
            _module.LogPuzzle($"There is one exact match with the winning number at station {(_allStations.First(x => x.Digits.SequenceEqual(answerOutput)).StationID + 1):00}");
        else
            _module.LogPuzzle($"The closest station(s) is/are: {finalStations.Select(x => (x.StationID + 1).ToString("00")).Join(", ")}");
    }

    private List<Station> FindPrioritizedStations(int[] winningNumbers)
    {
        if (_allStations.Any(x => x.Digits.SequenceEqual(winningNumbers)))
        {
            HasExactMatch = true;
            return new List<Station> { _allStations.First(x => x.Digits.SequenceEqual(winningNumbers)) };
        }

        List<Station> finalStations;

        var findDigits = SetupPriority(winningNumbers);

        if (!findDigits.All(x => _allStations.Count(y => x.DigitsToCheckIxes.Select(z => y.Digits[z]).SequenceEqual(x.DigitsToCheck)) >= 1))
        {
            var getSmallDifferences = _allStations.Select(x => Math.Abs(int.Parse(winningNumbers.Join("")) - x.CombinedDigits())).ToArray();
            var findMinValue = getSmallDifferences.Min();

            return _allStations.Where(x => Math.Abs(int.Parse(winningNumbers.Join("")) - x.CombinedDigits()) == findMinValue).ToList();
        }

        var findPrioritized = findDigits.First(x => _allStations.Count(y => x.DigitsToCheckIxes.Select(z => y.Digits[z]).SequenceEqual(x.DigitsToCheck)) >= 1);

        var getAllPossibleStations = _allStations.Where(x => findPrioritized.DigitsToCheckIxes.Select(y => x.Digits[y]).SequenceEqual(findPrioritized.DigitsToCheck)).ToList();

        var getStationResults = getAllPossibleStations.Select(x => Math.Abs(winningNumbers[findPrioritized.PrimaryIndex] - x.Digits[findPrioritized.PrimaryIndex])).ToList();
        var findMinimumOfStation = getStationResults.Min();

        if (getStationResults.Count(x => x == findMinimumOfStation) > 1 && findPrioritized.AlternativeIndex != null)
        {
            getStationResults = getAllPossibleStations.Select(x => Math.Abs(winningNumbers[findPrioritized.AlternativeIndex.Value] - x.Digits[findPrioritized.AlternativeIndex.Value])).ToList();
            findMinimumOfStation = getStationResults.Min();

            finalStations = getAllPossibleStations.Where(x => Math.Abs(winningNumbers[findPrioritized.AlternativeIndex.Value] - x.Digits[findPrioritized.AlternativeIndex.Value]) == findMinimumOfStation).ToList();
        }
        else
            finalStations = getAllPossibleStations.Where(x => Math.Abs(winningNumbers[findPrioritized.PrimaryIndex] - x.Digits[findPrioritized.PrimaryIndex]) == findMinimumOfStation).ToList();

        return finalStations;
    }
}