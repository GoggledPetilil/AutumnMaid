using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hilda : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator m_Anim;

    [Header("Dialogue")]
    public Dialogue m_StartDialogue;
    public Dialogue[] m_PizzaDialogue;

    void OnEnable()
    {
        m_Anim.SetBool("isSweeping", GameManager.instance.m_DeliveryStage < 2);
    }

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
