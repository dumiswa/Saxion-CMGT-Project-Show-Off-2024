using Monoliths.Visualisators;
using UnityEngine;
public class SnowflakeVisualisator : AbstractVisualisator<byte>
{
    public const string SNOWFLAKE_AMOUNT_DATA_ID = "SnowflakeAmount";
    private Animator[] _bars;

    private byte _amount;
    private byte _current;

    private SnowflakeContainer _prefab;
    private SnowflakeContainer _managed;

    public override void Defaults()
    {
        base.Defaults();
    }

    public override bool Init()
    {
        _dataID = LevelProgressObserver.CURRENT_SNOWFLAKES_DATA_ID;
        _autoDisplay = true;

        _prefab = Resources.Load<SnowflakeContainer>("Prefabs/Visualisators/InGame GUI/Snowflakes/Container");

        return base.Init();
    }

    protected override void Update()
    {
        base.Update();

        var data = DataBridge.TryGetData<byte>(SNOWFLAKE_AMOUNT_DATA_ID);
        if (!data.IsEmpty)
        {
            Reset(data.EncodedData);
            DataBridge.MarkUpdateProcessed<byte>(SNOWFLAKE_AMOUNT_DATA_ID);
        }
    }

    private void Reset(byte amount)
    {
        if (_amount == amount)
            return;

        _amount = amount;
        _managed ??= Object.Instantiate(_prefab, _gui.GetChild((int)RenderingLayer.LAYER2));
        _bars = _managed.ScaleSnowflakesTo(amount);
        _current = 0;
    }

    protected override void Display(byte data)
    {
        if (_amount == 0)
            return;

        while (data > _current)
            _bars[_current++].SetTrigger("Toggle");
        while (data < _current)
            _bars[_current--].SetTrigger("Toggle");
    }
}
