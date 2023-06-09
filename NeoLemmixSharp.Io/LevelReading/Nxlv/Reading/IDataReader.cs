﻿namespace NeoLemmixSharp.Io.LevelReading.Nxlv.Reading;

public interface IDataReader
{
    bool FinishedReading { get; }
    string IdentifierToken { get; }

    void BeginReading(string[] tokens);
    void ReadNextLine(string[] tokens);
}