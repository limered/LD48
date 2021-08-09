using System;
using System.Globalization;
using SystemBase;
using Systems.Distractions;
using Systems.GameMessages.Messages;
using Systems.Income;
using Systems.Tourist;
using StrongSystems.Audio;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Systems.Room.Events;
using Unity.VisualScripting;
using ColorUtility = UnityEngine.ColorUtility;
using Object = UnityEngine.Object;

namespace Systems.GameMessages
{
    [GameSystem]
    public class UISystem : GameSystem<UIComponent>
    {
        private float deathMessageSec = 120f;
        private bool showing;
        private float _animationTime = 5;

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

            MessageBroker.Default
                .Receive<RoomEverybodyDied>()
                .Subscribe(_ => ShowRestartMessage(component));
        }

        private void PreparePotentialIncome(ShowInitialPotentialIncome msg, UIComponent component)
        {
            var income = component.PotentialIncome.PotentialIncomeAmount;
            income.text = msg.InitialPotentialIncome.ToString();
        }

        private void PreparePotentialIncomeVanished(ReducePotentialIncomeAction msg, UIComponent component)
        {
            
            var potentialIncome = component.PotentialIncome;
            CreateMoneyVanished(potentialIncome, msg.IncomeVanished);
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
            distraction.sprite = distractions[MapDistractionTypeToIndex(msg.DistractionType)];
        }

        private int MapDistractionTypeToIndex(DistractionType type)
        {
            return type switch
            {
                DistractionType.None => 0,
                DistractionType.Tiger => 0,
                DistractionType.Butterfly => 0,
                DistractionType.Camera => 0,
                DistractionType.Spider => 1,
                DistractionType.Swamp => 0,
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

        private void CreateMoneyVanished(PotentialIncomeComponent potentialIncome, float change)
        {
            var colorText = "#BC2D2DFF";
            if (change >= 0)
            {
                colorText = "#00FF00FF";
            }
            else
            {
                "coin-drop".Play();
            }

            if (!ColorUtility.TryParseHtmlString(colorText, out var color)) return;

            var incomeVanished = Object.Instantiate(potentialIncome.VanishedIncome,
                new Vector3(0, -17, 0),
                Quaternion.identity,
                potentialIncome.transform
            );
            
            var text = incomeVanished.GetComponent<Text>();
            text.text = "-" + change;
            text.color = color;
        }

        private void ShowRestartMessage(UIComponent component)
        {
            Observable.Timer(TimeSpan.FromSeconds(_animationTime))
                .Subscribe(_ =>
                {
                    component.EverybodyDiesMessage.SetActive(true);
                });
        }
    }
}
