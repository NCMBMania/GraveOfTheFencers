using NCMB;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataStoreManager : SingletonMonoBehaviour<DataStoreManager>
{

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveGraveInfo(string userName, string deathMessage, GraveInfo.CurseType curseType, Vector3 position)
    {
        //ユーザー名が空の場合"Unknown"に//
        userName = string.IsNullOrEmpty(userName) ? "Unknown" : userName;

        //プレイヤーが死んだ位置を加工//
        position = new Vector3(position.x, 0f, position.z);
        double[] positionDoubleArray = Utility.Vector3toDoubleArray(position);

        //データストアにGraveクラスを定義//
        NCMBObject ncmbObject = new NCMBObject("Grave");

        //Message・UserName・Position・CurseTypeをKeyに、それぞれValueを設定//
        ncmbObject.Add("Message", deathMessage);
        ncmbObject.Add("UserName", userName);
        ncmbObject.Add("Position", positionDoubleArray);
        ncmbObject.Add("CurseType", (int)curseType);

        //非同期でデータを保存する//
        ncmbObject.SaveAsync((NCMBException e) =>
        {
            if (e != null)
            {
                //エラー処理
            }
            else
            {
                Main.Instance.OnInGame();
            }
        });
    }

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
}