using UnityEngine;

namespace Systems.Tourist.Messages
{
    public class TouristMessages
    {
        public class FollowingGuide
        {
        }
        
        public class GotDistracted
        {
            public Vector3 WantToGoHere { get; set; }
        }

        public class StartContemplating
        {
            private GameObject What { get; set; }
        }
        
        public class FinishContemplating
        {
            private GameObject What { get; set; }
        }
        
        public class GatheredByGuide
        {
            
        }
    }
}