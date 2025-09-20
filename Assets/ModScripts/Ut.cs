using System.Collections.Generic;
using System.Linq;

public static class Ut
{
    public static T[,] To2DArray<T>(this IEnumerable<IEnumerable<T>> source)
    {
        var data = source.Select(x => x.ToArray()).ToArray();

        var res = new T[data.Length, data.Max(x => x.Length)];

        for (int i = 0; i < data.Length; i++)
            for (int j = 0; j < data[i].Length; j++)
                res[i, j] = data[i][j];

        return res;
    }
}
