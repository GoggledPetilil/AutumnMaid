using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hilda : MonoBehaviour
{
    public Dialogue m_StartDialogue;
    public Dialogue[] m_PizzaDialogue;

    public void TriggerDialogue()
    {
        Dialogue dialogue = null;

        if(GameManager.instance.m_FlagMetHilda)
        {
            dialogue = m_PizzaDialogue[1];
        }
        else if(GameManager.instance.m_DeliveryStage >= 2)
        {
            dialogue = m_PizzaDialogue[0];
            GameManager.instance.m_FlagMetHilda = true;
        }
        else
        {
            dialogue = m_StartDialogue;
        }

        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
