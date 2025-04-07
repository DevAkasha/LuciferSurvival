public partial class UserInfo
{
    public static UserInfo myInfo { get => DataManager.instance.userInfo; set => DataManager.instance.userInfo = value; }
}