using System;
using System.Collections.Generic;
using System.Linq;

public class RTOPuzzle
{
    private List<Station> _allStations;

    public RTOPuzzle(List<Station> allStations)
    {
        _allStations = allStations;
    }

    private static readonly int?[] hexGridIxes =
    {
        58,
        40, 27,
        2, 57, 59,
        30, 43, 35, 22,
        15, 9, 23, 41, 50,
        21, 47, 33, 25,
        49, 38, 16, 56, 46,
        4, 5, 14, 31,
        13, 52, null, 1, 53,
        8, 3, 24, 55,
        32, 10, 26, 54, 34,
        51, 7, 11, 37,
        45, 6, 29, 0, 19,
        28, 48, 39, 42,
        18, 17, 44,
        20, 12,
        36
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

        return Enumerable.Range(0, 3).Select(_ => Enumerable.Range(0, 3).Select(x => Enumerable.Range(0, arithmeticTables[x].Length).Select(y => new WinningNumberCell(arithmeticTables[x][y], arithmeticDigitTables[x][y], kcn[x]))).To2DArray()).ToArray();
    }
}