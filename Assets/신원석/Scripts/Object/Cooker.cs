using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SoundManager;

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
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Cooking);
                soundcheck = false;
                animator.SetBool("Cooking", false);
                return;
            }
            else if(currentCook.FoodImage.fillAmount >= 1.0f)
            {
                SoundManager.GetInstance().Sfx_Stop(SoundManager.sfx.Cooking);


                if(soundcheck ==false)
                {
                    SoundManager.GetInstance().SfxPlay(SoundManager.sfx.FoodCompleted, false);
                    soundcheck = true;
                }
             
                animator.SetBool("Cooking", false);
            }
            else 
            {
                soundcheck = false;
                animator.SetBool("Cooking", true);

                if ( !SoundManager.GetInstance().AnySfxPlaying(SoundManager.sfx.Cooking))
                {
                    SoundManager.GetInstance().SfxPlay(SoundManager.sfx.Cooking,false);
                }
            }
        }

    }

    private bool soundcheck = false;

    Animator animator; 
}

