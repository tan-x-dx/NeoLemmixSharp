using System.Numerics;

namespace NeoLemmixSharp.Common.Util;

public readonly struct DataChunk2 : IBitwiseOperators<DataChunk2, DataChunk2, DataChunk2>
{
    public readonly int Data1;
    public readonly int Data2;

    public DataChunk2(int data1, int data2)
    {
        Data1 = data1;
        Data2 = data2;
    }

    public static DataChunk2 operator &(DataChunk2 left, DataChunk2 right)
    {
        var data1 = left.Data1 & right.Data1;
        var data2 = left.Data2 & right.Data2;

        return new DataChunk2(data1, data2);
    }

    public static DataChunk2 operator |(DataChunk2 left, DataChunk2 right)
    {
        var data1 = left.Data1 | right.Data1;
        var data2 = left.Data2 | right.Data2;

        return new DataChunk2(data1, data2);
    }

    public static DataChunk2 operator ^(DataChunk2 left, DataChunk2 right)
    {
        var data1 = left.Data1 ^ right.Data1;
        var data2 = left.Data2 ^ right.Data2;

        return new DataChunk2(data1, data2);
    }

    public static DataChunk2 operator ~(DataChunk2 value)
    {
        var data1 = ~value.Data1;
        var data2 = ~value.Data2;

        return new DataChunk2(data1, data2);
    }
}

public readonly struct DataChunk3 : IBitwiseOperators<DataChunk3, DataChunk3, DataChunk3>
{
    public readonly int Data1;
    public readonly int Data2;
    public readonly int Data3;

    public DataChunk3(int data1, int data2, int data3)
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
    }

    public static DataChunk3 BitwiseAnd(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 & right.Data1;
        var data2 = left.Data2 & right.Data2;
        var data3 = left.Data3 & right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 BitwiseOr(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 | right.Data1;
        var data2 = left.Data2 | right.Data2;
        var data3 = left.Data3 | right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 BitWiseNotAnd(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 & ~right.Data1;
        var data2 = left.Data2 & ~right.Data2;
        var data3 = left.Data3 & ~right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 operator &(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 & right.Data1;
        var data2 = left.Data2 & right.Data2;
        var data3 = left.Data3 & right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 operator |(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 | right.Data1;
        var data2 = left.Data2 | right.Data2;
        var data3 = left.Data3 | right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 operator ^(DataChunk3 left, DataChunk3 right)
    {
        var data1 = left.Data1 ^ right.Data1;
        var data2 = left.Data2 ^ right.Data2;
        var data3 = left.Data3 ^ right.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 operator ~(DataChunk3 value)
    {
        var data1 = ~value.Data1;
        var data2 = ~value.Data2;
        var data3 = ~value.Data3;

        return new DataChunk3(data1, data2, data3);
    }
}

public readonly struct DataChunk4 : IBitwiseOperators<DataChunk4, DataChunk4, DataChunk4>
{
    public readonly int Data1;
    public readonly int Data2;
    public readonly int Data3;
    public readonly int Data4;

    public DataChunk4(int data1, int data2, int data3, int data4)
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
    }

    public static DataChunk4 BitwiseAnd(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 & right.Data1;
        var data2 = left.Data2 & right.Data2;
        var data3 = left.Data3 & right.Data3;
        var data4 = left.Data4 & right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 BitwiseOr(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 | right.Data1;
        var data2 = left.Data2 | right.Data2;
        var data3 = left.Data3 | right.Data3;
        var data4 = left.Data4 | right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 BitWiseNotAnd(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 & ~right.Data1;
        var data2 = left.Data2 & ~right.Data2;
        var data3 = left.Data3 & ~right.Data3;
        var data4 = left.Data4 & ~right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 operator &(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 & right.Data1;
        var data2 = left.Data2 & right.Data2;
        var data3 = left.Data3 & right.Data3;
        var data4 = left.Data4 & right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 operator |(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 | right.Data1;
        var data2 = left.Data2 | right.Data2;
        var data3 = left.Data3 | right.Data3;
        var data4 = left.Data4 | right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 operator ^(DataChunk4 left, DataChunk4 right)
    {
        var data1 = left.Data1 ^ right.Data1;
        var data2 = left.Data2 ^ right.Data2;
        var data3 = left.Data3 ^ right.Data3;
        var data4 = left.Data4 ^ right.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 operator ~(DataChunk4 value)
    {
        var data1 = ~value.Data1;
        var data2 = ~value.Data2;
        var data3 = ~value.Data3;
        var data4 = ~value.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }
}
