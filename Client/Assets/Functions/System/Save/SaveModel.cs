
using UnityEngine;
[System.Serializable]
public class SaveModel
{
    /// <summary>
    /// The name of data type
    /// </summary>
    public string DataName { get; protected set; }

    public SaveModel()
    {
        DataName = GetType().Name;
    }
    public virtual void SetDefault() { }
}
[System.Serializable]
public class SaveLoginModel : SaveModel
{
    public string UserName;
    public string Password;
    public override void SetDefault()
    {
        base.SetDefault();
        UserName = "";
        Password = "";
    }
}