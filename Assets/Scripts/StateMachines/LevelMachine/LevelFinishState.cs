using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class LevelFinishState : LevelSubState
{
    public override void Enter()
    {
        base.Enter();

        SaveLevelInfoToFile();
        //PLAY ANIM
        Debug.Log("Level Finished, going back to the selection screen");
        GameStateMachine.Instance.Return();
    }

    private void SaveLevelInfoToFile()
    {
        var levelInfo = DataBridge.TryGetData<LevelResult>(LevelProgressObserver.LEVEL_INFO_BUFFER_DATA_ID).EncodedData;
        XmlSerializer serializer = new XmlSerializer(typeof(LevelResult));
        using (TextWriter writer = new StreamWriter(@"LevelResult.xml"))
        {
            serializer.Serialize(writer, levelInfo);
        }
    }
}
