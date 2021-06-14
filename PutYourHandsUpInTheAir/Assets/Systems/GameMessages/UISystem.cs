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
        private float deathMessageSec = 120f;
        private float vanishMoneySec = 120f;
        private bool showing = false;

        public override void Register(UIComponent component)
        {

            MessageBroker.Default.Receive<ShowInitialPotentialIncome>()
                .Subscribe(msg =>
                {
                    PreparePotentialIncome(msg, component);

                })
                .AddTo(component);

            MessageBroker.Default.Receive<ReducePotentialIncomeAction>()
                .Subscribe(msg =>
                {
                    vanishMoneySec = 80f;
                    PreparePotentialIncomeVanished(msg, component);
                })
                .AddTo(component);

            MessageBroker.Default.Receive<ShowDeadPersonMessageAction>()
                .Subscribe(msg =>
                {
                    deathMessageSec = 80f;
                    PrepareMessage(msg, component);
                    "man-dying".Play();

                    if (!showing)
                    {
                        ShowDeathMessage(component.Message.gameObject, true);
                    }

                })
                .AddTo(component);

            component.UpdateAsObservable().Subscribe(_ =>
            {
                if(deathMessageSec > 0)
                {
                    deathMessageSec--;
                } else
                {
                    ShowDeathMessage(component.Message.gameObject, false);
                }
            });
        }

        private void PreparePotentialIncome(ShowInitialPotentialIncome msg, UIComponent component)
        {
            var income = component.PotentialIncome.PotentialIncomeAmount;
            income.text = msg.InitialPotentialIncome.ToString();
        }

        private void PreparePotentialIncomeVanished(ReducePotentialIncomeAction msg, UIComponent component)
        {
            "coin-drop".Play();
            var potentialIncome = component.PotentialIncome;
            var incomeAmount = Int32.Parse(potentialIncome.PotentialIncomeAmount.text);
            potentialIncome.PotentialIncomeAmount.text = (incomeAmount - msg.IncomeVanished).ToString();
            
            CreateMoneyVanished(potentialIncome, msg.IncomeVanished.ToString());
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

        private void ShowDeathMessage(GameObject gameObject, bool show)
        {
            showing = show;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.SetBool("show", show);
            deathMessageSec = 80f;
        }

        private void CreateMoneyVanished(PotentialIncomeComponent potentialIncome, String text) {
            var incomeVanished = GameObject.Instantiate(potentialIncome.VanishedIncome,
                new Vector3(0, -17, 0),
                Quaternion.identity,
                potentialIncome.transform
            );
            
            //TODO incomeVanished.GetComponent<Text>() = "-" + text;
            vanishMoneySec = 80f;
        }
    }
}
