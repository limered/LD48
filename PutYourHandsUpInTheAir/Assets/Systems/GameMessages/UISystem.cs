using System;
using SystemBase;
using Systems.GameMessages.Messages;
using Systems.Tourist;
using StrongSystems.Audio;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.GameMessages
{
    [GameSystem]
    public class UISystem : GameSystem<UIComponent>
    {
        private float sec = 120f;
        private bool showing = false;

        public override void Register(UIComponent component)
        {
            component.PotentialIncome.IncomeVanished.gameObject.SetActive(false);

            MessageBroker.Default.Receive<ShowInitialPotentialIncome>()
                .Subscribe(msg =>
                {
                    PreparePotentialIncome(msg, component);

                })
                .AddTo(component);

            MessageBroker.Default.Receive<ReducePotentialIncome>()
                .Subscribe(msg =>
                {
                    PreparePotentialIncome(msg, component);

                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowDeadPersonAction>()
                .Subscribe(msg =>
                {
                    ResetTime();
                    PrepareMessage(msg, component);
                    "man-dying".Play();

                    if (!showing)
                    {
                        ShowMessage(component.Message.gameObject, true);
                    }

                })
                .AddTo(component);

            component.UpdateAsObservable().Subscribe(_ =>
            {
                if(sec > 0)
                {
                    sec--;
                } else
                {
                    ShowMessage(component.Message.gameObject, false);
                    ResetTime();
                }
            });
        }

        private void ResetTime()
        {
            sec = 80f;
        }

        private void PreparePotentialIncome(ShowInitialPotentialIncome msg, UIComponent component)
        {
            var income = component.PotentialIncome.PotentialIncomeAmount;
            income.text = msg.InitialPotentialIncome.ToString();

        }

        private void PreparePotentialIncome(ReducePotentialIncome msg, UIComponent component)
        {
            var potentialIncome = component.PotentialIncome;
            var incomeAmount = Int32.Parse(potentialIncome.PotentialIncomeAmount.text);
            potentialIncome.PotentialIncomeAmount.text = (incomeAmount - msg.IncomeVanished).ToString();
            var incomeVanished = potentialIncome.IncomeVanished;
            incomeVanished.text = "-" + msg.IncomeVanished.ToString();
            incomeVanished.gameObject.SetActive(true);

            component.UpdateAsObservable().Subscribe(_ =>
            {
                if (sec > 0)
                {
                    sec--;
                }
                else
                {
                    incomeVanished.gameObject.SetActive(false);
                    ResetTime();
                }
            });
        }

        private void PrepareMessage(ShowDeadPersonAction msg, UIComponent component)
        {
            var text = component.Message.Text;
            text.text = msg.TouristName + " died.";

            var faces = component.GetComponentInParent<TouristConfigComponent>().topParts;
            var face = component.Message.Image;
            face.sprite = faces[msg.TouristFaceIndex];

            var distractions = component.Message.Distractions;
            var distraction = component.Message.Distraction;
            distraction.sprite = distractions[msg.DistractionIndex];
        }

        private void ShowMessage(GameObject gameObject, bool show)
        {
            showing = show;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.SetBool("show", show);
        }
    }
}
