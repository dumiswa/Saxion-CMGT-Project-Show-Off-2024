using Monoliths.Visualisators;
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
        Debug.Log("Level Finished!");

        //FOR TESTING PURPOSES ONLY
        DataBridge.UpdateData("TestDataBridgeUniqueDataIDBasedOnTesterPacket", new TesterPacket() { text = "Level is finished!"});
        Time.timeScale = 0.0f;
        //FOR TESTING PURPOSES ONLY

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
