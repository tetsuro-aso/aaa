using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    ///<summary>選択中のぶにゃんこ</summary>
    private List<Bunyanko> _SelectBunyanko = new List<Bunyanko>();
    ///<summary>選択中のぶにゃんこID</summary>
    private string _SelectID = "";

    ///<summary>シングルトンインスタンス</summary>
    public static LevelManager Instance { get; private set; }

    ///<summary>ぶにゃんこPrefabリスト</summary>
    public GameObject[] BunyankoPrefabs;
    ///<summary>ぶにゃんこを消すために必要な数</summary>
    public int BunyankoDestroyCount = 3;
    ///<summary>ぶにゃんこをつなぐ範囲</summary>
    public float  BunyankoConnectRange = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        BunyankoSpawn(56);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    ///<summary>ぶにゃんこ生成</summary>
    ///<param name="count">生成数</param>
    private void BunyankoSpawn(int count)
    {
        var StartX = -2;
        var StartY = 5;
        var X = 0;
        var Y = 0;
        var MaxX = 5;

        for(int i = 0; i < count; i++)
        {
            var Position = new Vector3(StartX + X, StartY + Y, 0);
            Instantiate(BunyankoPrefabs[Random.Range(0,BunyankoPrefabs.Length)], Position, Quaternion.identity);

            X++;
            if(X==MaxX)
            {
                X = 0;
                Y++;
            }
        }
    }
    ///<summary>ぶにゃんこDownイベント</summary>
    ///＜param name="bunyanko">ぶにゃんこ</param>
    public void BunyankoDown(Bunyanko bunyanko)
    {
        _SelectBunyanko.Add(bunyanko);
        bunyanko.SetIsSelect(true);

        _SelectID = bunyanko.ID;
    }

    ///<summary>ぶにゃんこEnterイベント</summary>
    ///＜param name="bunyanko">ぶにゃんこ</param>
    public void BunyankoEnter(Bunyanko bunyanko)
    {
        // 選択しようとしているぶにゃんこのIDをログに出力
        Debug.Log("BunyankoEnter called with Bunyanko ID: " + bunyanko.ID);

        if (_SelectID != bunyanko.ID)
        {
            Debug.Log("Different ID. Current Select ID: " + _SelectID + ", Bunyanko ID: " + bunyanko.ID);
            return;
        }

        if(bunyanko.IsSelect)
        {
            Debug.Log("Bunyanko is already selected. ID: " + bunyanko.ID);
        }
        else
        {
            var LastSelectedBunyankoPosition = _SelectBunyanko[_SelectBunyanko.Count - 1].transform.position;
            var CurrentBunyankoPosition = bunyanko.transform.position;
            var Length = (LastSelectedBunyankoPosition - CurrentBunyankoPosition).magnitude;

            // 計算された距離をログに出力
            Debug.Log("Distance between last selected and current Bunyanko: " + Length);

            if(Length < BunyankoConnectRange)
            {
                _SelectBunyanko.Add(bunyanko);
                bunyanko.SetIsSelect(true);
                Debug.Log("Bunyanko connected. ID: " + bunyanko.ID);
            }
            else
            {
                Debug.Log("Bunyanko out of range. Cannot connect. ID: " + bunyanko.ID);
            }
        }
    }


    ///<summary>ぶにゃんこUpイベント</summary>
    public void BunyankoUp()
    {
        if(_SelectBunyanko.Count >= BunyankoDestroyCount)
        {
            DestroyBunyanko(_SelectBunyanko);
        }
        else
        {
            foreach (var BunyankoItem in _SelectBunyanko)
                BunyankoItem.SetIsSelect(false);
        }

        _SelectID = "";
        _SelectBunyanko.Clear();
    }

    ///<summary>ぶにゃんこを消す</summary>
    ///＜param name="bunyanko">消すぶにゃんこ</param>
    private void DestroyBunyanko(List<Bunyanko> bunyanko)
    {
        foreach(var BunyankoItem in bunyanko)
        {
            Destroy(BunyankoItem.gameObject);
        }
    }
}
