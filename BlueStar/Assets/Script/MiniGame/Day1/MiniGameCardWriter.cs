using System.Collections;
using System.Collections.Generic;
using BlueStar.Inventory;
using TMPro;
using UnityEngine;

public class MiniGameCardWriter : MonoBehaviour
{
    [Header("UI组件")] [SerializeField] private TMP_Text description;
    private GameObject UI;
    private GameObject UIInst;
    public static TMP_Text codeShow;
    public static int code;
    public int answer;
    public static int answerStatic;
    public Vector3 pos;
    [Header("按钮")] [SerializeField] private GameObject[] buttons;
    private GameObject confirmButton;
    [Header("ID卡")] [SerializeField] private int itemNumber;

    public void InitiateUI()
    {
        UIInst=Instantiate(UI, GameObject.Find("------UI------/UI_3D").gameObject.transform);
        UIInst.transform.position = pos;
        description.text = "请输入身份ID";
        
    }
    
    
    

    int onClick()
    {
        MiniGameCardWriter.code =MiniGameCardWriter. code * 10 + int.Parse(this.name);
        MiniGameCardWriter.codeShow.text = MiniGameCardWriter.code.ToString();
        return code;
    }

    void Confirm()
    {
        if (MiniGameCardWriter.code==MiniGameCardWriter.answerStatic)
        {
            Instantiate(InventoryManager.Instance.GetItemDetails(itemNumber).itemObject, this.transform);
        }
    }
    

}
