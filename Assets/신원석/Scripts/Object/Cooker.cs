using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cooker : MonoBehaviour
{
    public Queue<Cook> Cooks { get; set; } = new Queue<Cook>();

    void Start()
    {
        EventBus<GetFirstCookEvent>.Raise(new GetFirstCookEvent(this));
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < Cooks.Count; i++)
        {
            Cook currentCook = Cooks.Peek();

            if (Cooks.Count == 0)
            {
                animator.SetBool("Cooking", false);
                return;
            }
            else if(currentCook.FoodImage.fillAmount >= 1.0f)
            {
                animator.SetBool("Cooking", false);
            }
            else 
            {
                animator.SetBool("Cooking", true);
            }
        }

    }

    Animator animator; 
}

