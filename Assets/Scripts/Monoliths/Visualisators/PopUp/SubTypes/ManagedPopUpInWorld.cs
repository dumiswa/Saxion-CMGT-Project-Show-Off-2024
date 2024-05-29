namespace Monoliths.Visualisators
{
    public class ManagedPopUpInWorld : PopUpInWorld, IManagedPopUp
    {
        protected ushort _id;
        public void SetID(ushort id) => _id = id;
        public void RemoveItself()
        {
            var stack = DataBridge.TryGetData<PopUpStackPacket>(PopUpVisualisator.POP_UP_STACK_DATA_ID);
            stack.EncodedData.Remove(_id);
        }
    }
}