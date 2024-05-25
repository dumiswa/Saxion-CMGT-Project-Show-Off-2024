using System;
using System.Collections.Generic;

public struct Data<T> where T : new()
{
    public T EncodedData;
    public bool WasUpdated;
    public bool IsEmpty;

    public Data(T data)
    {
        EncodedData = data;
        WasUpdated = false;
        IsEmpty = false;
    }

    public Data(bool _ = false)
    {
        EncodedData = new T();
        WasUpdated = false;
        IsEmpty = true;
    }

    public static readonly Data<T> Empty = new();

    public static bool operator ==(Data<T> a, Data<T> b) 
        =>  a.Equals(b);
    public static bool operator !=(Data<T> a, Data<T> b) 
        => !a.Equals(b);

    public override readonly bool Equals(object obj) 
        => obj is Data<T> data &&
            EqualityComparer<T>.Default.Equals(EncodedData, data.EncodedData) &&
            WasUpdated == data.WasUpdated;

    public override readonly int GetHashCode() 
        => HashCode.Combine(EncodedData, WasUpdated);
}
