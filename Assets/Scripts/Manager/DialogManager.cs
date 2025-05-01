using System;
using System.Collections.Generic;
using System.IO;
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

    //��CSV���ضԻ�����
    public void LoadDialogueFromCSV(string csvPath)
    {
        try
        {
            var lines = File.ReadAllLines(csvPath);
            Debug.Log("��ʼ���ضԻ����ݣ��ļ�·����" + csvPath);

            for (int i = 1; i < lines.Length; i++)  // ����ͷ��
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

                // ������Ʒ����
                if (!string.IsNullOrEmpty(line[9]))
                {
                    // ������ƷPrefab·���洢��CSV�ĵ�10��
                    itemRewardPrefab = Resources.Load<GameObject>(line[9]);
                }

                //�����Ի��ڵ�
                string npcName = line.Length > 10 ? line[10] : "";
                DialogueNode node = new DialogueNode(npcId, dialogueId, dialogueText, option1, option2, option3,
                                                      nextDialogue1, nextDialogue2, nextDialogue3, itemRewardPrefab, npcName);

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

    // ��ʼ�Ի�
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
