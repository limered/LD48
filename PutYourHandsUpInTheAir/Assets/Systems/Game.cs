using GameState.States;
using SystemBase;
using SystemBase.StateMachineBase;
using GameState.Messages;
using StrongSystems.Audio.Helper;
using UniRx;
using UnityEngine;
using Utils;
using UnityEngine.SceneManagement;

namespace Systems
{
    public class Game : GameBase
    {
        public readonly StateContext<Game> GameStateContext = new StateContext<Game>();

        private void Awake()
        {
            IoC.RegisterSingleton(this);

            GameStateContext.Start(new Loading());

            InstantiateSystems();

            Init();

            MessageBroker.Default.Publish(new GameMsgFinishedLoading());
            MessageBroker.Default.Publish(new GameMsgStart());
        }

        private void Start()
        {
           // MessageBroker.Default.Publish(new GameMsgStart());
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
        }

        public override void Init()
        {
            base.Init();

            IoC.RegisterSingleton<ISFXComparer>(()=> new SFXComparer());
        }

        public void StartInstruction()
        {
            SceneManager.LoadScene("Instruction");
        }

        public void StartGame()
        {
            MessageBroker.Default.Publish(new GameMsgStart());
            //SceneManager.LoadScene("Jungle");
        }

        public void EndGame()
        {
            Application.Quit();
        }
    }
}