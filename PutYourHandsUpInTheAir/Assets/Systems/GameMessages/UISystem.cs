using System;
using SystemBase;
using Systems.Distractions;
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

            MessageBroker.Default.Receive<ReducePotentialIncomeAction>()
                .Subscribe(msg =>
                {
                    PreparePotentialIncome(msg, component);
                    "coin-drop".Play();
                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowDeadPersonMessageAction>()
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

        private void PreparePotentialIncome(ReducePotentialIncomeAction msg, UIComponent component)
        {
            var potentialIncome = component.PotentialIncome;
            var incomeAmount = Int32.Parse(potentialIncome.PotentialIncomeAmount.text);
            potentialIncome.PotentialIncomeAmount.text = (incomeAmount - msg.IncomeVanished).ToString();
            var incomeVanished = potentialIncome.IncomeVanished;
            incomeVanished.text = "-" + msg.IncomeVanished.ToString();
            incomeVanished.gameObject.SetActive(true);

            Animator animator = potentialIncome.GetComponent<Animator>();
            animator.SetBool("show", true);

            component.UpdateAsObservable().Subscribe(_ =>
            {
                if (sec > 0)
                {
                    sec--;
                }
                else
                {
                    animator.SetBool("show", false);
                    incomeVanished.gameObject.SetActive(false);
                    ResetTime();
                }
            });
        }

        private void PrepareMessage(ShowDeadPersonMessageAction msg, UIComponent component)
        {
            var text = component.Message.Text;
            text.text = msg.TouristName + " died.";

            var faces = component.GetComponentInParent<TouristConfigComponent>().topParts;
            var face = component.Message.Image;
            face.sprite = faces[msg.TouristFaceIndex];

            var distractions = component.Message.Distractions;
            var distraction = component.Message.Distraction;
            distraction.sprite = distractions[MapDistractionTypeTiIndex(msg.DistractionType)];
        }

        private int MapDistractionTypeTiIndex(DistractionType type)
        {
            return type switch
            {
                DistractionType.None => 0,
                DistractionType.Tiger => 0,
                DistractionType.Butterfly => 0,
                DistractionType.Camera => 0,
                DistractionType.Spider => 1,
                DistractionType.Swamp => 0,
                DistractionType.Money => 0,
                DistractionType.Bus => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private void ShowMessage(GameObject gameObject, bool show)
        {
            showing = show;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.SetBool("show", show);
        }
    }
}
