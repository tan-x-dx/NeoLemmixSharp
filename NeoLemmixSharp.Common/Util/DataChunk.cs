namespace NeoLemmixSharp.Common.Util;

public readonly struct DataChunk2
{
    public readonly int Data1;
    public readonly int Data2;

    public DataChunk2(int data1, int data2)
    {
        Data1 = data1;
        Data2 = data2;
    }

    public static DataChunk2 BitwiseAnd(DataChunk2 firstChunk, DataChunk2 secondChunk)
    {
        var data1 = firstChunk.Data1 & secondChunk.Data1;
        var data2 = firstChunk.Data2 & secondChunk.Data2;

        return new DataChunk2(data1, data2);
    }

    public static DataChunk2 BitwiseOr(DataChunk2 firstChunk, DataChunk2 secondChunk)
    {
        var data1 = firstChunk.Data1 | secondChunk.Data1;
        var data2 = firstChunk.Data2 | secondChunk.Data2;

        return new DataChunk2(data1, data2);
    }

    public static DataChunk2 BitWiseNotAnd(DataChunk2 firstChunk, DataChunk2 secondChunk)
    {
        var data1 = firstChunk.Data1 & ~secondChunk.Data1;
        var data2 = firstChunk.Data2 & ~secondChunk.Data2;

        return new DataChunk2(data1, data2);
    }
}

public readonly struct DataChunk3
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

    public static DataChunk3 BitwiseAnd(DataChunk3 firstChunk, DataChunk3 secondChunk)
    {
        var data1 = firstChunk.Data1 & secondChunk.Data1;
        var data2 = firstChunk.Data2 & secondChunk.Data2;
        var data3 = firstChunk.Data3 & secondChunk.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 BitwiseOr(DataChunk3 firstChunk, DataChunk3 secondChunk)
    {
        var data1 = firstChunk.Data1 | secondChunk.Data1;
        var data2 = firstChunk.Data2 | secondChunk.Data2;
        var data3 = firstChunk.Data3 | secondChunk.Data3;

        return new DataChunk3(data1, data2, data3);
    }

    public static DataChunk3 BitWiseNotAnd(DataChunk3 firstChunk, DataChunk3 secondChunk)
    {
        var data1 = firstChunk.Data1 & ~secondChunk.Data1;
        var data2 = firstChunk.Data2 & ~secondChunk.Data2;
        var data3 = firstChunk.Data3 & ~secondChunk.Data3;

        return new DataChunk3(data1, data2, data3);
    }
}

public readonly struct DataChunk4
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

    public static DataChunk4 BitwiseAnd(DataChunk4 firstChunk, DataChunk4 secondChunk)
    {
        var data1 = firstChunk.Data1 & secondChunk.Data1;
        var data2 = firstChunk.Data2 & secondChunk.Data2;
        var data3 = firstChunk.Data3 & secondChunk.Data3;
        var data4 = firstChunk.Data4 & secondChunk.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 BitwiseOr(DataChunk4 firstChunk, DataChunk4 secondChunk)
    {
        var data1 = firstChunk.Data1 | secondChunk.Data1;
        var data2 = firstChunk.Data2 | secondChunk.Data2;
        var data3 = firstChunk.Data3 | secondChunk.Data3;
        var data4 = firstChunk.Data4 | secondChunk.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }

    public static DataChunk4 BitWiseNotAnd(DataChunk4 firstChunk, DataChunk4 secondChunk)
    {
        var data1 = firstChunk.Data1 & ~secondChunk.Data1;
        var data2 = firstChunk.Data2 & ~secondChunk.Data2;
        var data3 = firstChunk.Data3 & ~secondChunk.Data3;
        var data4 = firstChunk.Data4 & ~secondChunk.Data4;

        return new DataChunk4(data1, data2, data3, data4);
    }
}
