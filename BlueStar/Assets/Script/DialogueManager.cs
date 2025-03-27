using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;

public class DialogueManager : MonoBehaviour
{
    public GameObject UI;
    public static int currentDialogueBeginID=0;
    public TextAsset dialogDataFile;
    public Image spriteLeft;
    public Image spriteRight;
    public TMP_Text dialogText;
    public TMP_Text nameText;
    public List<Sprite> sprites = new List<Sprite>();
    private Dictionary<string, Sprite> imageDic = new Dictionary<string, Sprite>();
    public int dialogIndex;
    public string[] dialogRows;
    public GameObject nextButton;
    public GameObject optionButton;
    public Transform buttonGroup;
    public Camera focusCamera;
    public Camera mainCamera;
    public GameObject UI_MR;
    private AudioSource clipSound;
    public GameObject UI_Front;
    public GameObject Canvas;
    public GameObject NPC_AI;
    private GameObject NPC_AI_inst;
    private int timer=0;
    public static PlayableDirector director;
    
    public void Awake()
    {
      
        imageDic["泰拉"] = sprites[0];
        imageDic["Mao"] = sprites[1];
        imageDic["伊达"] = sprites[2];
        imageDic["受伤的清理员"] = sprites[3];
        imageDic["朴义君"] = sprites[4];
        dialogIndex = currentDialogueBeginID;
        //Canvas.GetComponent<CanvasGroup>().alpha = 0f;
        NPC_AI = Resources.Load<GameObject>("Prefabs/Character/NPC/NPC_AI");

    }

    public void Start()
    {
      
        ReadText(dialogDataFile);
        clipSound = GetComponent<AudioSource>();
        ShowDialogRow();
        //Canvas.GetComponent<CanvasGroup>().DOFade(1, 3);
       // UI_Front.transform.DOMove(new Vector2(transform.position.x + 4000, transform.position.y), 1f).From();
       
        


    }

    private void Update()
    {
        if (dialogIndex==10)
        {
            mainCamera.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        
    }

    public void UpdateText(string _name, string _text)
    {
        nameText.text = _name;
        dialogText.text = _text;
    }

    public void UpdateImage(string _name, string _position)
    {
        if (!imageDic.ContainsKey(_name))
        {
            Debug.LogError($"Key '{_name}' not found in imageDic. Ensure it is initialized correctly.");
            return;
        }
        if (_position == "左")
        {
            
            spriteLeft.sprite = imageDic[_name];
        }
        
        else if(_position == "右")
        {
            spriteRight.sprite = imageDic[_name];
        }
    }

    public void ReadText(TextAsset _textAsset)
    {
        dialogRows = _textAsset.text.Split('\n');
    }

    public void ShowDialogRow()
    {
        UI_Front.SetActive(true);
        for(int i=0;i<dialogRows.Length;i++)
        {
            string[] cells = dialogRows[i].Split(',');
            if (cells[0] == "#" && int.Parse(cells[1]) == dialogIndex)
            {
                UpdateText(cells[2],cells[4]);
                UpdateImage(cells[2],cells[3]);
                if (cells[2] != "泰拉")
                {
                    spriteLeft.gameObject.SetActive(true);
                    spriteRight.gameObject.SetActive(false);
                }
                else
                {
                    spriteLeft.gameObject.SetActive(false);
                    spriteRight.gameObject.SetActive(true);

                }
                dialogIndex = int.Parse(cells[5]);
                nextButton.gameObject.SetActive(true);
                break;
            }
            
            else if (cells[0] == "&" && int.Parse(cells[1]) == dialogIndex)
            {
                nextButton.gameObject.SetActive(false);
                GenerateOption(i);
            }
            else if (cells[0]=="END"&& int.Parse(cells[1])==dialogIndex)
            {
                UI_Front.SetActive(false);
                if (director!=null)
                {
                    director.Play();
                    director = null;
                }

            }
        }
        //---------------有风险-----------
        //currentDialogueBeginID = dialogIndex;
    }

    public void onClickNext()
    {
        clipSound.Play();
        ShowDialogRow();
    }

    public void GenerateOption(int _index)
    {
        string[] cells = dialogRows[_index].Split(',');
        if (cells[0] == "&")
        {
            GameObject button = Instantiate(optionButton, buttonGroup);
            button.GetComponentInChildren<TMP_Text>().text = cells[4];
            button.GetComponent<Button>().onClick.AddListener(
                delegate
                    {
                        OnOptionClick(int.Parse(cells[5]));
                    });
            GenerateOption(_index+1);
            
        }

    }

    public void OnOptionClick(int _id)
    {
        dialogIndex = _id;
        ShowDialogRow();
        for (int i = 0; i < buttonGroup.childCount; i++)
        {
            Destroy(buttonGroup.GetChild(i).gameObject,0.4f);
        }
    }
}

