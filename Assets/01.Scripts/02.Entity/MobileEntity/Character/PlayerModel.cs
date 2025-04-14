using System.Collections.Generic;

public class PlayerModel: BaseModel
{
    public RxModFloat Hp;
    public RxModFloat MoveSpeed;
    public PlayerModel()
    {
        Hp = new(100f, nameof(Hp), this);
        MoveSpeed = new(4f, nameof(MoveSpeed), this);
    }
    public override IEnumerable<IModifiable> GetModifiables()
    {
        yield return Hp;
        yield return MoveSpeed;
    }
}