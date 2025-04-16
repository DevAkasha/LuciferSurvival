using System.Collections.Generic;

public class PlayerModel: BaseModel
{
    public RxModFloat Health;
    public RxModFloat MoveSpeed;
    public PlayerModel()
    {
        Health = new(100f, nameof(Health), this);
        MoveSpeed = new(4f, nameof(MoveSpeed), this);
    }
    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Health;
        yield return MoveSpeed;
    }
}