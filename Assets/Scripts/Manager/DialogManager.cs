using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    // �洢����NPC�ĶԻ����ݣ��ֵ��ʽ��NPC ID -> �Ի��б�
    public Dictionary<int, List<DialogueNode>> dialogues = new Dictionary<int, List<DialogueNode>>();
    private HashSet<int> finishedNpcIds = new HashSet<int>();

    private DialogueNode currentDialogue;
    private PlayerController player;

    private void Awake()
    {
        // ȷ��ֻ��һ��ʵ��
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

            Debug.Log("��ʼ���ضԻ����ݣ��ļ�·����" + csvPath);

            for (int i = 1; i < lines.Count; i++) // ����ͷ��
            {
                string[] fields = SplitCsvLine(lines[i]);

                if (fields.Length < 10)
                {
                    Debug.LogWarning($"�������������У�{lines[i]}");
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

                // ������Ʒ����
                if (!string.IsNullOrEmpty(fields[9]))
                {
                    string path = fields[9].Trim(); // ��ֹ����Ŀո���·������
                    itemRewardPrefab = Resources.Load<GameObject>(path);
                    if (itemRewardPrefab == null)
                    {
                        Debug.LogWarning($"�޷�����Ԥ���壬·����{path}������·���Ƿ���ȷ���Լ���Դ�Ƿ���� Resources �ļ����ڡ�");
                    }
                    else
                    {
                        Debug.Log($"�ɹ�������ƷԤ���壺{itemRewardPrefab.name}��·����{path}");
                    }
                }

                // �����Ի��ڵ�
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
                    Debug.Log("����NPC�Ի����ݣ�NPC ID��" + npcId);
                }

                dialogues[npcId].Add(node);
                Debug.Log("���ضԻ��ڵ㣬NPC ID��" + npcId + "���Ի�ID��" + dialogueId);
            }

            Debug.Log("�Ի����ݼ�����ɣ������� " + dialogues.Count + " ��NPC�ĶԻ�����");
        }
        catch (Exception e)
        {
            Debug.LogError("���ضԻ�����ʧ�ܣ�������Ϣ��" + e.Message);
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

            // ��⻻�з������ǲ�ͬϵͳ�Ļ��з���
            if ((c == '\n' && !inQuotes) || (c == '\r' && !inQuotes))
            {
                // �����������ڵĻ��з�ʱ���һ��
                if (c == '\n')
                {
                    lines.Add(currentLine.ToString().Trim());
                    currentLine.Clear();
                }
            }
            else
            {
                // ���Իس�����\r����ֻ�����з���\n��
                if (c != '\r')
                {
                    currentLine.Append(c);
                }
            }
        }

        // ������һ��
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
                // ����˫����ת��
                if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++; // ������һ������
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

        // ������һ���ֶ�
        fields.Add(currentField.ToString().Trim());

        // �Ƴ��ֶ���β�����Ų�����ת��
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
    // ��ʼ�Ի�
    public void StartDialogue(int npcId, int startDialogueId = 1)
    {
        player = FindObjectOfType<PlayerController>();
        currentDialogue = GetDialogueNode(npcId, startDialogueId);
        if (currentDialogue != null)
        {
            ShowDialogue();
        }
    }

    // ��ʾ��ǰ�Ի����ݺ�ѡ��
    public void ShowDialogue()
    {
        if (currentDialogue == null) return;

        // ����ʾ�Ի��ı�
        string[] dialogueLines = new string[] { currentDialogue.dialogueText };
        UIManager.Instance.ShowDialogue(dialogueLines, OnDialogueComplete, currentDialogue.npcName);

    }
    private void OnDialogueComplete()
    {
        Debug.Log("��ʾ��ť");
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
        Debug.Log("ѡ��ص�");
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

        Debug.Log("�����Ի�");
        UIManager.Instance.HideDialogue();
    }

    // ͨ�� npcId �� dialogueId ��ȡ��Ӧ�ĶԻ��ڵ�
    public DialogueNode GetDialogueNode(int npcId, int dialogueId)
    {
        if (dialogues.ContainsKey(npcId))
        {
            return dialogues[npcId].Find(d => d.dialogueId == dialogueId);
        }
        return null;
    }

    // �������Ʒ
    private void GiveCollectibleItem(GameObject itemPrefab)
    {
        if (itemPrefab == null || player == null) return;

        GameObject itemInstance = Instantiate(itemPrefab);

        player.inventory.AddItem(itemInstance.GetComponent<InteractableObject>());
        Debug.Log("��Ʒ�ѽ�������ң�");
    }
}
