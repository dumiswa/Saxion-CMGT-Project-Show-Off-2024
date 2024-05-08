using System.Collections.Generic;

public static class DataBridge
{
    private static Dictionary<string, Data> _data = new();
    public static void UpdateData(string key, object data)
    {
        if (_data.ContainsKey(key))
            _data[key] = new(data) { IsDirty = true };
        else
            _data.Add(key, new(data));
    }
    public static Data TryGetData(string key) 
        => _data.ContainsKey(key)? _data[key] : Data.Empty;
}
