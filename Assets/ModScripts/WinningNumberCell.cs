using static UnityEngine.Random;

public class WinningNumberCell
{
    public Arithmetic Arithmetic { get; private set; }
    public int GetResultedValue { get; private set; }

    private readonly int _arithmeticValue;

    public WinningNumberCell(Arithmetic arithmetic, int arithmeticValue, int currentValue)
    {
        Arithmetic = arithmetic;
        _arithmeticValue = arithmeticValue;

        var modifiedValue = currentValue;

        switch (arithmetic)
        {
            case Arithmetic.Add:
                modifiedValue += arithmeticValue;
                break;
            case Arithmetic.Subtract:
                modifiedValue -= arithmeticValue;
                break;
            default:
                modifiedValue = Range(0, 2) == 0 ? modifiedValue + arithmeticValue : modifiedValue - arithmeticValue;
                break;
                
        }

        modifiedValue %= 10;

        GetResultedValue = modifiedValue;
    }

    public override string ToString() => $"{"+-±"[(int)Arithmetic]}{_arithmeticValue}";
}