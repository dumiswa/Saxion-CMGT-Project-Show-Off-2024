public abstract class Monolith
{
    public bool IsActive { get; protected set; } = false;
    public string MonolithStatus { get; protected set; } = "Not Initialised";
    public virtual bool Init()
    {
        IsActive = true;
        MonolithStatus = "Successfully Initiated";
        return true;
    }
}
