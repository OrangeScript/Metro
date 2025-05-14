using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    public void LoadDialogueFromCSV(string csvPath)
    {
        try
        {
            string csvContent = File.ReadAllText(csvPath);
            List<string> lines = ReadCsvLines(csvContent);

            Debug.Log("开始加载对话数据，文件路径：" + csvPath);

            for (int i = 1; i < lines.Count; i++) // 跳过头部
            {
                string[] fields = SplitCsvLine(lines[i]);

                if (fields.Length < 10)
                {
                    Debug.LogWarning($"跳过不完整的行：{lines[i]}");
                    continue;
                }

                int npcId = int.Parse(fields[0]);
                int dialogueId = int.Parse(fields[1]);
                string dialogueText = fields[2];
                string option1 = fields[3];
                string option2 = fields[4];
                string option3 = fields[5];
                int nextDialogue1 = string.IsNullOrEmpty(fields[6]) ? -1 : int.Parse(fields[6]);
                int nextDialogue2 = string.IsNullOrEmpty(fields[7]) ? -1 : int.Parse(fields[7]);
                int nextDialogue3 = string.IsNullOrEmpty(fields[8]) ? -1 : int.Parse(fields[8]);
                GameObject itemRewardPrefab = null;

                // 解析物品奖励
                if (!string.IsNullOrEmpty(fields[9]))
                {
                    string path = fields[9].Trim(); // 防止意外的空格导致路径错误
                    itemRewardPrefab = Resources.Load<GameObject>(path);
                    if (itemRewardPrefab == null)
                    {
                        Debug.LogWarning($"无法加载预制体，路径：{path}，请检查路径是否正确，以及资源是否放在 Resources 文件夹内。");
                    }
                    else
                    {
                        Debug.Log($"成功加载物品预制体：{itemRewardPrefab.name}，路径：{path}");
                    }
                }

                // 创建对话节点
                string npcName = fields.Length > 10 ? fields[10] : "";
                DialogueNode node = new DialogueNode(
                    npcId, dialogueId, dialogueText,
                    option1, option2, option3,
                    nextDialogue1, nextDialogue2, nextDialogue3,
                    itemRewardPrefab, npcName
                );

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

    private List<string> ReadCsvLines(string csvContent)
    {
        List<string> lines = new List<string>();
        StringBuilder currentLine = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in csvContent)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }

            // 检测换行符（考虑不同系统的换行符）
            if ((c == '\n' && !inQuotes) || (c == '\r' && !inQuotes))
            {
                // 遇到非引号内的换行符时完成一行
                if (c == '\n')
                {
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear();
                }
            }
            else
            {
                // 忽略回车符（\r），只处理换行符（\n）
                if (c != '\r')
                {
                    currentLine.Append(c);
                }
            }
        }

        // 添加最后一行
        if (currentLine.Length > 0)
        {
            lines.Add(currentLine.ToString().Trim());
        }

        return lines;
    }

    private string[] SplitCsvLine(string line)
    {
        List<string> fields = new List<string>();
        StringBuilder currentField = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                // 处理双引号转义
                if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // 跳过下一个引号
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField.ToString().Trim());
                currentField.Clear();
            }
            else
            {
                currentField.Append(c);
            }
        }

        // 添加最后一个字段
        fields.Add(currentField.ToString().Trim());

        // 移除字段首尾的引号并处理转义
        for (int i = 0; i < fields.Count; i++)
        {
            if (fields[i].StartsWith("\"") && fields[i].EndsWith("\""))
            {
                fields[i] = fields[i].Substring(1, fields[i].Length - 2);
                fields[i] = fields[i].Replace("\"\"", "\"");
            }
        }

        return fields.ToArray();
    }
    // 开始对话
    public void StartDialogue(int npcId, int startDialogueId = 1)
    {
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
                    Debug.Log("2");
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
