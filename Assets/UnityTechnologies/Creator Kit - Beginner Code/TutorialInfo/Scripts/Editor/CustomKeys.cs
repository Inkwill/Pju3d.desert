using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomKeys : Editor
{

 [MenuItem("Tools/CustomKeys/切换物体显隐状态 %e")]
    public static void SetObjActive()
    {
        GameObject[] selectObjs = Selection.gameObjects;
        int objCtn = selectObjs.Length;
        for (int i = 0; i < objCtn; i++)
        {
            bool isAcitve = selectObjs[i].activeSelf;
            selectObjs[i].SetActive(!isAcitve);

			EditorUtility.SetDirty(selectObjs[i]);//这样才能保存状态
        }
    }
}
