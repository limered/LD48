using SystemBase;
using GameState.States.Tourist;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Tourist
{
    [GameSystem]
    public class TouristMovementSystem : GameSystem<TouristMovementComponent>
    {
        public static Vector3 forward = new Vector3(0, 1, 0);
        public static Vector3 back = -forward;
        public static Vector3 right = new Vector3(1, 0, 0);
        public static Vector3 left = -right;
        public static float wordWidthExtend = 3;
        
        
        public override void Register(TouristMovementComponent component)
        {
            component.States.Start(new FollowingGuide());
            
            component.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    if (component.States.CurrentState.Value is FollowingGuide)
                    {
                        MoveAlongGuide(component);
                    }
                    else if (component.States.CurrentState.Value is Distracted distracted)
                    {
                        MoveTowardAttraction(component, distracted.MovingToward);
                    }
                    else if (component.States.CurrentState.Value is Contemplating contemplating)
                    {
                        Contemplate(component, contemplating.Attraction);
                    }
                })
                .AddTo(component);
        }

        private void MoveAlongGuide(TouristMovementComponent tourist/*, GuideComponent guide*/)
        {
            tourist.transform.position += forward * tourist.speed * Time.deltaTime;

            if (tourist.transform.position.x > 0)
            {
                tourist.transform.position -= new Vector3(tourist.speed * Time.deltaTime, 0, 0);
            }
            else if (tourist.transform.position.x < 0)
            {
                tourist.transform.position += new Vector3(tourist.speed * Time.deltaTime, 0, 0);
            }
        }
        
        private void MoveTowardAttraction(TouristMovementComponent tourist, AttractionComponent attraction)
        {
            var delta = attraction.transform.position - tourist.transform.position;
            
            tourist.transform.position += delta.normalized * tourist.speed;
        }

        private void Contemplate(TouristMovementComponent tourist, AttractionComponent attraction)
        {
            
        }
    }
}