using UnityEngine;

public class DialogueNode
{
    public int npcId;  // NPC的ID
    public int dialogueId;  // 当前对话节点的ID
    public string dialogueText;  // 当前对话的文本内容

    // 对话选项
    public string option1;
    public string option2;
    public string option3;

    // 跳转到的下一个对话ID
    public int nextDialogueId1;
    public int nextDialogueId2;
    public int nextDialogueId3;

    // 物品奖励
    public GameObject itemRewardPrefab;  // 该节点是否奖励物品

    //npc名字
    public string npcName;

    // 构造函数
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
