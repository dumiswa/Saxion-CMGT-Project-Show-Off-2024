using System;

public struct Data
{
    public Type DecodingType;
    public object EncodedData;
    public bool IsDirty;

    public Data (object data)
    {
        DecodingType = data.GetType();
        EncodedData = data;

        IsDirty = false;
    }

    public static readonly Data Empty = new(null);
}
