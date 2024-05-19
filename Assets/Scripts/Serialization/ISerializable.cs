public interface ISerializable
{
    public byte[] Serialize();
    public void Deserialize(byte[] bytes);
}

