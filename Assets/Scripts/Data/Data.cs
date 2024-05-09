using System;
using System.Collections.Generic;

public struct Data<T> where T : new()
{
    public T EncodedData;
    public bool IsDirty;
    public bool IsEmpty;

    public Data(T data)
    {
        EncodedData = data;
        IsDirty = false;
        IsEmpty = false;
    }

    public Data(bool _ = false)
    {
        EncodedData = new T();
        IsDirty = false;
        IsEmpty = true;
    }

    public static readonly Data<T> Empty = new();

    public static bool operator ==(Data<T> a, Data<T> b) 
        =>  a.Equals(b);
    public static bool operator !=(Data<T> a, Data<T> b) 
        => !a.Equals(b);

    public override bool Equals(object obj) 
        => obj is Data<T> data &&
            EqualityComparer<T>.Default.Equals(EncodedData, data.EncodedData) &&
            IsDirty == data.IsDirty;

    public override int GetHashCode() 
        => HashCode.Combine(EncodedData, IsDirty);
}
