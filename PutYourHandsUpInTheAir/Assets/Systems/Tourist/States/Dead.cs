using SystemBase.StateMachineBase;
using Systems.Distractions;
using Systems.GameMessages.Messages;
using Systems.Movement;
using UniRx;
using UnityEngine;

namespace Systems.Tourist.States
{
    [NextValidStates(/*none, you are dead...*/)]
    public class Dead : BaseState<TouristBrainComponent>
    {
        public DistractionType KilledByDistractionType;

        public Dead(DistractionType killedByDistractionType)
        {
            KilledByDistractionType = killedByDistractionType;
        }
        public override void Enter(StateContext<TouristBrainComponent> context)
        {
            MessageBroker.Default.Publish(new ShowDeadPersonMessageAction
            {
                TouristName = context.Owner.touristName.Value,
                TouristFaceIndex = context.Owner.headPartIndex.Value,
                DistractionType = KilledByDistractionType,
            });

            MessageBroker.Default.Publish(new ReducePotentialIncomeAction
            {
                IncomeVanished = 100
            });

            context.Owner.GetComponent<TwoDeeMovementComponent>().Stop();

            if (context.Owner.GetComponent<Collider>()) Object.Destroy(context.Owner.GetComponent<Collider>());
            var body = context.Owner.GetComponent<TouristBodyComponent>();
            if (!body) return;

            body.blood.SetActive(true);
            body.livingBody.transform.Rotate(new Vector3(0, 0, 1), 360 * Random.value);
        }
    }
}