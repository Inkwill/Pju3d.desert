using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using CreatorKitCode;
using Random = UnityEngine.Random;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "DropBox", menuName = "Data/DropBox", order = 10)]
public class DropBox : ScriptableObject
{
    [Serializable]
    public class DropItem
    {
        public Item item;
        public int itemNum;
        public int dropWeight;
        public int accWeight;

    }

    public int minNum;
    public int maxNum;
    public List<DropItem> BoxContent;

    [ContextMenu("Initialize")]
    private void Initialize()
    {
        BoxContent = AccumulateWeight(BoxContent);
    }
    public List<DropItem> GetDropItem()
    {
        int num = Random.Range(minNum, maxNum + 1);
        List<DropItem> result = new List<DropItem>();
        for (int i = 0; i < num; i++)
        {
            result.Add(GetRandomElement(BoxContent));
        }
        return result;
    }

    DropItem GetRandomElement(List<DropItem> box)
    {
        // 计算总权重
        int totalWeight = box.Sum(x => x.dropWeight);

        // 生成随机数
        int randomValue = Random.Range(1, totalWeight + 1);

        // 使用 LINQ 查询语法选择元素
        DropItem result = box.FirstOrDefault(x => randomValue <= x.accWeight);

        // 返回选中的元素
        return result;
    }

    List<DropItem> AccumulateWeight(List<DropItem> box)
    {
        int acc = 0;
        foreach (var item in box)
        {
            acc += item.dropWeight;
            item.accWeight = acc;
        }
        return box;
    }
}
