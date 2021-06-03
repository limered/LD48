using SystemBase;
using SystemBase.StateMachineBase;
using Systems.Movement;
using UniRx;
using UnityEngine;

namespace Systems.Tourist
{
    [RequireComponent(typeof(TwoDeeMovementComponent))]
    public class TouristBrainComponent : GameComponent
    {
        public StringReactiveProperty touristName = new StringReactiveProperty();
        public SphereCollider socialDistanceCollider;
        public IntReactiveProperty headPartIndex = new IntReactiveProperty();
        public IntReactiveProperty bodyPartIndex = new IntReactiveProperty();
        public float idleMinTimeWithoutMovementInSeconds = 3f;
        public string debugCurrentState;
        public Vector2 debugTargetDistance;

        public StateContext<TouristBrainComponent> StateContext { get; set; }

        public bool HasPaid { get; set; }
        public float StoprInterestPropability = 0.3f;
        public float IntrerestPropability = 0.3f;
        public float DistractionSearchRange = 2f;
        public float DistractionSearchRangeIncrement = 0.1f;

        protected override void OverwriteStart()
        {
            base.OverwriteStart();
            if (StateContext != null) return;
            StateContext = new StateContext<TouristBrainComponent>(this);
        }
    }

    public static class TouristNames
    {
        public static readonly string[] All =
        {
            "Jo Lawrence",
            "Brynn Murphy",
            "Ashton Evans",
            "Jesse Mason",
            "Jesse Dawson",
            "Harley Chaney",
            "Kai Jacobson",
            "Chris Petty",
            "Val Fox",
            "Gene Shaw",
            "Alexis Lloyd",
            "Dane Matthews",
            "Rowan Cunningham",
            "Raylee Fletcher",
            "Jude West",
            "Carmen West",
            "Haiden Puckett",
            "Angel Ratliff",
            "Gene Ballard",
            "Ashley Eaton",
            "Fran Hussain",
            "Avery Patel",
            "Kai Gardner",
            "Fran Cooper",
            "Gale Taylor",
            "Reed Tucker",
            "Franky Cox",
            "Dane Mays",
            "Phoenix Delaney",
            "Taylor Sims",
            "Tanner Hill",
            "Caden Reid",
            "River Morgan",
            "Rene Cooper",
            "Casey Bailey",
            "Kiran Crosby",
            "Frankie Campbell",
            "Sam Phillips",
            "Clem Marks",
            "Lane Knowles",
            "Lee Harper",
            "Sam Hudson",
            "Tyler Jenkins",
            "Drew Black",
            "Emerson Brown",
            "Kiran Rosario",
            "Leigh Maldonado",
            "Charlie Bryant",
            "Aiden Talley",
            "Carmen Ray",
            "Danny Morgan",
            "Eli Jenkins",
            "Eli Turner",
            "Gail Hill",
            "Nicky Lowe",
            "Rudy Rodgers",
            "Rory Mays",
            "Jody Mosley",
            "Ryan Shepherd",
            "Rudy Finley",
            "Clem Bates",
            "Danny Brooks",
            "Gabby Nicholson",
            "Riley Berry",
            "Gabby Robinson",
            "Denny Marquez",
            "Jamie Reid",
            "Mell Garrison",
            "Jude Cotton",
            "Clem Lang",
            "Steff Francis",
            "Justice Adams",
            "Gale Ross",
            "Skylar Ross",
            "Willy Green",
            "Angel Evans",
            "Marley Pace",
            "Shay Montgomery",
            "Denny Joyner",
            "Caden Hampton",
        };
    }
}