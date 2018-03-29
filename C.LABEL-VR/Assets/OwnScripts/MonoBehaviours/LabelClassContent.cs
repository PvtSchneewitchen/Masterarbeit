using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LabelClassContent : MonoBehaviour
{

    public GameObject _labelClassItemPrefab;

    public Button _currentSelectedItem { get; set; }

    private List<GameObject> LabelClassItems;

    private void Awake()
    {
        LabelClassItems = new List<GameObject>();
    }

    public void UpdateContent()
    {
        var classes = Labeling.GetAllLabelClassInfos();

        ResetLabelClassItemList();
        for (int i = 0; i < classes.Count; i++)
        {
            GameObject item = Instantiate(_labelClassItemPrefab, transform);
            ItemContent itemCont = item.GetComponent<ItemContent>();
            uint key = classes.ElementAt(i).Key;
            Tuple<string, Material> val = classes.ElementAt(i).Value;

            itemCont._className.text = val.Item1;
            itemCont._classID.text = Convert.ToString(key);
            itemCont._classImage.color = val.Item2.color;

            LabelClassItems.Add(item);
        }
    }

    public void OnLabelingButtonClick()
    {
        UpdateContent();
    }

    private void ResetLabelClassItemList()
    {
        foreach (var item in LabelClassItems)
        {
            Destroy(item);
        }
        LabelClassItems.Clear();
    }
}
