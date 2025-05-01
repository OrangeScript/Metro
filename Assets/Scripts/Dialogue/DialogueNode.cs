using UnityEngine;

public class DialogueNode
{
    public int npcId;  // NPC��ID
    public int dialogueId;  // ��ǰ�Ի��ڵ��ID
    public string dialogueText;  // ��ǰ�Ի����ı�����

    // �Ի�ѡ��
    public string option1;
    public string option2;
    public string option3;

    // ��ת������һ���Ի�ID
    public int nextDialogueId1;
    public int nextDialogueId2;
    public int nextDialogueId3;

    // ��Ʒ����
    public GameObject itemRewardPrefab;  // �ýڵ��Ƿ�����Ʒ

    //npc����
    public string npcName;

    // ���캯��
    public DialogueNode(int npcId, int dialogueId, string dialogueText, string option1, string option2, string option3,
                    int nextDialogueId1, int nextDialogueId2, int nextDialogueId3, GameObject itemRewardPrefab = null, string npcName = "")
    {
        this.npcId = npcId;
        this.dialogueId = dialogueId;
        this.dialogueText = dialogueText;
        this.option1 = option1;
        this.option2 = option2;
        this.option3 = option3;
        this.nextDialogueId1 = nextDialogueId1;
        this.nextDialogueId2 = nextDialogueId2;
        this.nextDialogueId3 = nextDialogueId3;
        this.itemRewardPrefab = itemRewardPrefab;
        this.npcName = npcName;
    }
}
