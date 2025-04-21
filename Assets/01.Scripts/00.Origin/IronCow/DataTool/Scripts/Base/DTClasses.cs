public partial class UserInfo
{
    public static UserInfo myInfo { get => DataManager.Instance.userInfo; set => DataManager.Instance.userInfo = value; }
}
