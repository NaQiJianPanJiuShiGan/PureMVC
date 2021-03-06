﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using SUIFW;

public class GuideManager : MonoSingletion<GuideManager> {

    private Transform maskTra;

    private string fileDir = "GuideFile/";
    private string nowCsvFile;
    private int nowIndex;
    private bool isFinish = false;//是否完成所有的新手引导
    private string[] nameArray;
    private GameObject _canvas;
    public void Init()
    {
        _canvas = GameObject.FindGameObjectWithTag(SysDefine.SYS_TAG_CANVAS);
        //读取进度
        string content = Resources.Load<TextAsset>(fileDir + "GuideProgress").ToString();
        string[] temp = content.Split(',');
        nowCsvFile = temp[0];
        nowIndex = int.Parse(temp[1]);
        isFinish = bool.Parse(temp[2]);

        //读取需要高亮的组件的Hierarchy路径
        if (!isFinish)
        {
            string s = Resources.Load<TextAsset>(fileDir + nowCsvFile).ToString();
            nameArray = s.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);//按换行划分   
        } 
    }

    void OnDestroy()
    {
        //退出游戏后的处理
        Debug.Log("OnDestroy");
    }

    public void Next()
    {
        if (nowIndex < nameArray.Length)
        {
            ShowHightLight(nameArray[nowIndex]);
            nowIndex++;
        }
        else//加载下一个文件
        {
            maskTra.gameObject.SetActive(false);
            Debug.Log(nowCsvFile.Substring(nowCsvFile.Length - 1));
            int index = int.Parse(nowCsvFile.Substring(nowCsvFile.Length - 1));
            index++;
            nowCsvFile = nowCsvFile.Substring(0, nowCsvFile.Length - 1) + index.ToString();
            string path = fileDir + nowCsvFile;//加载下一个文件的路径(Guid2e...)

            string content = null;
            try
            {
                content = Resources.Load<TextAsset>(path).ToString();
            }
            catch (Exception e) 
            {
                isFinish = true;
                Debug.Log("finish");
                return;
            }
            nowIndex = 0;
            nameArray = content.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);  
        }
    }

    void ShowHightLight(string name, bool checkIsClone = true)
    {
        if(checkIsClone && name.Contains("/"))
        {
            name = name.Insert(name.IndexOf('/'), "(Clone)");
        }
        StartCoroutine(FindUI(name));
    }

    void CancelHightLight(GameObject go)
    {
        Destroy(go.GetComponent<GraphicRaycaster>());
        Destroy(go.GetComponent<Canvas>());
        Next();
        EventTriggerListener.GetListener(go).onPointerClick -= CancelHightLight;
    }

    IEnumerator FindUI(string name)
    {
        //寻找目标
        GameObject go = UnityHelper.FindTheChildNode(_canvas, name).gameObject;
        while(go == null)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("wait");
            go = UnityHelper.FindTheChildNode(_canvas, name).gameObject;
        }
       
        //高亮
        maskTra = UnityHelper.FindTheChildNode(_canvas, "_UIMaskPanel").transform;
        maskTra.gameObject.SetActive(true);
        maskTra.SetAsLastSibling();
        Canvas canvas = go.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 1;
        go.AddComponent<GraphicRaycaster>();

        //设置监听
        EventTriggerListener.GetListener(go).onPointerClick += CancelHightLight;
    }

}
