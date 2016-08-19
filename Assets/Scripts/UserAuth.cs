using NCMB;
using System;
using UnityEngine;

public class UserAuth : SingletonMonoBehaviour<UserAuth>
{
    //public bool isGuestMode;
    private string currentPlayerName;
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // mobile backendに接続してログイン ------------------------

    public void LogIn(string id, string pw, Action callback)
    {
        NCMBUser.LogInAsync(id, pw, (NCMBException e) =>
        {
            // 接続成功したら
            if (e == null)
            {
                currentPlayerName = id;
                if (callback != null) callback();
            }
        });
    }

    // mobile backendに接続して新規会員登録 ------------------------

    public void SignUp(string id, string mail, string pw, Action callback)
    {
        NCMBUser user = new NCMBUser();
        user.UserName = id;
        user.Email = mail;
        user.Password = pw;
        user.SignUpAsync((NCMBException e) =>
        {
            if (e == null)
            {
                currentPlayerName = id;
                if (callback != null) callback();
            }
        });
    }

    // mobile backendに接続してログアウト ------------------------

    public void LogOut()
    {
        NCMBUser.LogOutAsync((NCMBException e) =>
        {
            if (e == null)
            {
                currentPlayerName = null;
            }
        });
    }

    // 現在のプレイヤー名を返す --------------------
    public string CurrentPlayerName()
    {
        return currentPlayerName;
    }

    //ゲストモードでユーザー名を保存する//
    public void SetGuestName(string name, Action callback)
    {
        currentPlayerName = name;
        if (callback != null) callback();
    }
}