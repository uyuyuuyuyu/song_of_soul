﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractLoad : MonoBehaviour
{
    public InteractiveContainerSO InteractiveContainer;
    public GameObject ItemPrefab;
    //public DialogContainerSO DialogContainer;

    // Use this for initialization
    void Start()
    {
        
        foreach (InteractiveSO interactiveItem in InteractiveContainer.InteractiveItemList)
        {
            GameObject go = Instantiate(ItemPrefab, transform);
            go.transform.position = interactiveItem.Coord;

            SpriteRenderer npcSprite = go.GetComponent<SpriteRenderer>();
            NPCController npcController = go.AddComponent<NPCController>();
            //TalkController npcTalkController = go.AddComponent<TalkController>();


            foreach (DialogueSectionSO DialogueItem in TalkSOManager.Instance.DialogueSectionList)
            {
                int i = 0;//每添加一次闲聊对话这个i+1
                if (DialogueItem.NPCID == interactiveItem.ID)
                {
                    TalkManager.Instance.NPCAllContent[interactiveItem.ID] = new Dictionary<int, List<string>>();
                    TalkManager.Instance.Name[interactiveItem.ID] = DialogueItem.NPCName;
                    TalkManager.Instance.NPCAllCondition[interactiveItem.ID] = new Dictionary<int, string>();
                    TalkManager.Instance.NPCAllGossip[interactiveItem.ID] = new Dictionary<int, List<string>>();
                    TalkManager.Instance.gossipRand[interactiveItem.ID] = new List<int>();

                    for (int num = 0; num < DialogueItem.DialogueList.Count; num++) //存储对话内容
                    {
                        List<string> TalkContent = new List<string>();
                        for (int j = 0; j < DialogueItem.DialogueList[num].Content.ToArray().Length; j++)
                        {
                            TalkContent.Add(DialogueItem.DialogueList[num].Content[j]);
                        }
                        if (DialogueItem.DialogueList[num].Type.Equals("plot"))
                        {
                            TalkManager.Instance.NPCAllContent[interactiveItem.ID][num-i] = TalkContent;
                        }
                        else
                        {
                            TalkManager.Instance.NPCAllGossip[interactiveItem.ID][i] = TalkContent;
                            TalkManager.Instance.gossipRand[interactiveItem.ID].Add(i);
                            i += 1;
                        }
                    }

                    //储存条件列表，同时把条件的Name和是否达成装入TalkManager的TalkStatusJudge，这里装，改变条件在别的地方改变
                    for (int num = 0; num < DialogueItem.DialogueList.ToArray().Length; num++)
                    {
                        if (DialogueItem.DialogueList[num].StatusList.ToArray().Length != 0) //如果这段对话有条件控制，把控制这段话的条件的Name装入TalkStatus字典
                        {
                            for (int j = 0; j < DialogueItem.DialogueStatusList.ToArray().Length; j++)
                            {
                                TalkManager.Instance.NPCAllCondition[interactiveItem.ID][num] = DialogueItem.DialogueList[num].StatusList[j];
                                //如果字典里还没有这个条件则写入，默认未达成
                                if (!TalkManager.Instance.TalkStatusJudge.ContainsKey(DialogueItem.DialogueStatusList[j].ConditionName))
                                {
                                    TalkManager.Instance.TalkStatusJudge[DialogueItem.DialogueStatusList[j].ConditionName] = DialogueItem.DialogueStatusList[j].Judge;
                                }
                            }
                        }
                    }
                    //把条件的Name和是否达成装入TalkManager的TalkStatusJudge，这里装，改变条件在别的地方改变
                }
                
            }
            npcSprite.sprite = interactiveItem.Icon;
            npcSprite.flipX = !interactiveItem.IsFaceRight;
            npcController.InteractiveItem = interactiveItem;
            //Debug.Log(npcTalkController.DialogueSection);
            //TalkManager.Instance.NPCAllCondition[interactiveItem.ID] = TalkManager.Instance.TalkStatus;
        }
    }
}
