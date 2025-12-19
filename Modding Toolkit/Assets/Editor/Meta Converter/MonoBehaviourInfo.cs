using Sirenix.OdinInspector;

public class MonoBehaviourInfo
{
    [ShowInInspector] public readonly string name;
    [ShowInInspector] public readonly string guid;
    [ShowInInspector] public readonly string fileId;
        
    public MonoBehaviourInfo(string name, string guid, string fileId)
    {
        this.name = name;
        this.guid = guid;
        this.fileId = fileId;
    }
}