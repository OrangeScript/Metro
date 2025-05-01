using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    // 存储所有NPC的对话数据，字典格式，NPC ID -> 对话列表
    public Dictionary<int, List<DialogueNode>> dialogues = new Dictionary<int, List<DialogueNode>>();
    private HashSet<int> finishedNpcIds = new HashSet<int>();

    private DialogueNode currentDialogue;
    private PlayerController player;

    private void Awake()
    {
        // 确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //从CSV加载对话数据
    public void LoadDialogueFromCSV(string csvPath)
    {
        try
        {
            var lines = File.ReadAllLines(csvPath);
            Debug.Log("开始加载对话数据，文件路径：" + csvPath);

            for (int i = 1; i < lines.Length; i++)  // 跳过头部
            {
                var line = lines[i].Split(',');

                int npcId = int.Parse(line[0]);
                int dialogueId = int.Parse(line[1]);
                string dialogueText = line[2];
                string option1 = line[3];
                string option2 = line[4];
                string option3 = line[5];
                int nextDialogue1 = string.IsNullOrEmpty(line[6]) ? -1 : int.Parse(line[6]);
                int nextDialogue2 = string.IsNullOrEmpty(line[7]) ? -1 : int.Parse(line[7]);
                int nextDialogue3 = string.IsNullOrEmpty(line[8]) ? -1 : int.Parse(line[8]);
                GameObject itemRewardPrefab = null;

                // 解析物品奖励
                if (!string.IsNullOrEmpty(line[9]))
                {
                    // 假设物品Prefab路径存储在CSV的第10列
                    itemRewardPrefab = Resources.Load<GameObject>(line[9]);
                }

                //创建对话节点
                string npcName = line.Length > 10 ? line[10] : "";
                DialogueNode node = new DialogueNode(npcId, dialogueId, dialogueText, option1, option2, option3,
                                                      nextDialogue1, nextDialogue2, nextDialogue3, itemRewardPrefab, npcName);

                if (!dialogues.ContainsKey(npcId))
                {
                    dialogues[npcId] = new List<DialogueNode>();
                    Debug.Log("新增NPC对话数据，NPC ID：" + npcId);
                }

                dialogues[npcId].Add(node);
                Debug.Log("加载对话节点，NPC ID：" + npcId + "，对话ID：" + dialogueId);
            }

            Debug.Log("对话数据加载完成，共加载 " + dialogues.Count + " 个NPC的对话数据");
        }
        catch (Exception e)
        {
            Debug.LogError("加载对话数据失败，错误信息：" + e.Message);
        }
    }

    // 开始对话
    public void StartDialogue(int npcId, int startDialogueId = 1)
    {
        /*if (finishedNpcIds.Contains(npcId))
        {
            ShowOnlyText("...");
            return;
        }*/
        player = FindObjectOfType<PlayerController>();
        currentDialogue = GetDialogueNode(npcId, startDialogueId);
        if (currentDialogue != null)
        {
            ShowDialogue();
        }
    }

    // 显示当前对话内容和选项
    public void ShowDialogue()
    {
        if (currentDialogue == null) return;

        // 先显示对话文本
        string[] dialogueLines = new string[] { currentDialogue.dialogueText };
        UIManager.Instance.ShowDialogue(dialogueLines, OnDialogueComplete, currentDialogue.npcName);



    }
    private void OnDialogueComplete()
    {
        Debug.Log("显示按钮");
        List<string> options = new List<string>();

        if (!string.IsNullOrEmpty(currentDialogue.option1)) options.Add(currentDialogue.option1);
        if (!string.IsNullOrEmpty(currentDialogue.option2)) options.Add(currentDialogue.option2);
        if (!string.IsNullOrEmpty(currentDialogue.option3)) options.Add(currentDialogue.option3);

        if (options.Count > 0)
        {
            UIManager.Instance.ShowOptions(options.ToArray(), OnOptionSelected);
        }
        else
        {
            if (currentDialogue.nextDialogueId1 != -1)
            {
                currentDialogue = GetDialogueNode(currentDialogue.npcId, currentDialogue.nextDialogueId1);
                if (currentDialogue != null)
                {
                    ShowDialogue();
                }
                else
                {
                   EndDialogue(); 
                }
            }
            else
            {
                EndDialogue(); 
            }
        }
    }
    private void OnOptionSelected(int optionIndex)
    {
        Debug.Log("选项回调");
        int nextDialogueId = -1;

        switch (optionIndex)
        {
            case 0:
                nextDialogueId = currentDialogue.nextDialogueId1;
                break;
            case 1:
                nextDialogueId = currentDialogue.nextDialogueId2;
                break;
            case 2:
                nextDialogueId = currentDialogue.nextDialogueId3;
                break;
        }

        if (nextDialogueId != -1)
        {
            currentDialogue = GetDialogueNode(currentDialogue.npcId, nextDialogueId);

            if (currentDialogue != null)
            {
                if (currentDialogue.itemRewardPrefab != null)
                {
                    GiveCollectibleItem(currentDialogue.itemRewardPrefab);
                }

                ShowDialogue();
            }
            else
            {
                EndDialogue();
            }
        }    
    }


    private void EndDialogue()
    {
        if (currentDialogue != null)
        {
            finishedNpcIds.Add(currentDialogue.npcId);
        }

        Debug.Log("结束对话");
        UIManager.Instance.HideDialogue();
    }

    // 通过 npcId 和 dialogueId 获取对应的对话节点
    public DialogueNode GetDialogueNode(int npcId, int dialogueId)
    {
        if (dialogues.ContainsKey(npcId))
        {
            return dialogues[npcId].Find(d => d.dialogueId == dialogueId);
        }
        return null;
    }

    // 给玩家物品
    private void GiveCollectibleItem(GameObject itemPrefab)
    {
        if (itemPrefab == null || player == null) return;

        GameObject itemInstance = Instantiate(itemPrefab);

        player.inventory.AddItem(itemInstance.GetComponent<InteractableObject>());
        Debug.Log("物品已奖励给玩家！");
    }
}
