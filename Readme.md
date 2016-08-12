# コードの解説

[資料](https://speakerdeck.com/ncmb/akusiyongemuniyuru-isosiyaruxing-xie-li-ji-neng-wozuo-rimasiyou)では説明できなかった、ニフティクラウドmobile backendのコードを解説します。

## Grave情報の引き出し

Graveの情報をデータストアから引き出すのはScript>DataStoreManager.csの「FetchGraveData（）」にて行っています。
ニフティクラウド mobile backendの機能を使い、データストアから引き出す際にデータの作成日時順にソートし最新10件だけ取得するようにしています。

取得した情報は独自に設定したGraveInfo型に成型し、
GraveObjectsManager.Instance.InstallationGrave(）に引き渡しています。


```csharp:DataStoreManager.cs
using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoreManager : SingletonMonoBehaviour<DataStoreManager>
{
    ・
    ・一部省略
    ・

    public void FetchGraveData(Action callback)
    {
        //PinPositionクラスを検索するクエリを作成
        NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("Grave");

        //日付順にソート//
        query.OrderByDescending("createDate");

        //最新10件までを取得する//
        query.Limit = 10;

        query.FindAsync((List<NCMBObject> graveList, NCMBException e) =>
        {
            if (e != null)
            {
                //データは見つからなかった//
                callback();
            }
            else
            {
                List<GraveInfo> graveInfoList = new List<GraveInfo>();

                foreach (NCMBObject grave in graveList)
                {
                    //取得結果をGraveInfo構造体に格納//
                    GraveInfo graveInfo;
                    graveInfo.userName = grave["UserName"] as string;
                    graveInfo.deathMessage = grave["Message"] as string;
                    graveInfo.messageId = grave.ObjectId;
                    graveInfo.curseType = (GraveInfo.CurseType)Enum.ToObject(typeof(GraveInfo.CurseType), grave["CurseType"]);
                    graveInfo.position = Utility.DoubleArrayListToVector3(grave["Position"] as ArrayList);
                    graveInfo.isUsed = false;

                    graveInfoList.Add(graveInfo);
                }

                //GraveInfoListをGraveObjectsManager.Instance.InstallationGraves()に引き渡し//
                GraveObjectsManager.Instance.InstallationGraves(graveInfoList);
                callback();
            }
        });
    }
```

## ログイン・アカウント登録について

本プロジェクトではログイン・アカウント登録をScript>UserAuth.csで行っています。

それぞれ「login」「signUp」というメソッド名で実装されており
ニフティクラウド mobile backendの会員管理機能を利用しています。

### ログインについて

ログインはニフティクラウド mobile backendのメソッドを使っています。

```csharp:UserAuth.cs
using NCMB;
using System;
using UnityEngine;

public class UserAuth : SingletonMonoBehaviour<UserAuth>
{
    ・
    ・一部省略
    ・

    // mobile backendに接続してログイン ------------------------

    public void logIn(string id, string pw)
    {
        logIn(id, pw, null);
    }

    public void logIn(string id, string pw, Action callback)
    {
    	//ニフティクラウド mobile backendのメソッドを使用
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

    ・
    ・一部省略
    ・
}
```

### アカウント登録について

アカウント登録を使用しており、id,メールアドレス、パスワードを指定して登録を行っています

```csharp:UserAuth.cs
using NCMB;
using System;
using UnityEngine;

public class UserAuth : SingletonMonoBehaviour<UserAuth>
{
    ・
    ・一部省略
    ・
    // mobile backendに接続して新規会員登録 ------------------------

    public void signUp(string id, string mail, string pw)
    {
        signUp(id, mail, pw, null);
    }

    public void signUp(string id, string mail, string pw, Action callback)
    {
        NCMBUser user = new NCMBUser();
        user.UserName = id;
        user.Email = mail;
        user.Password = pw;
        //ニフティクラウド mobile backendのメソッドを使用
        user.SignUpAsync((NCMBException e) =>
        {
            if (e == null)
            {
                currentPlayerName = id;
                if (callback != null) callback();
            }
        });
    }

    ・
    ・一部省略
    ・
}
```
