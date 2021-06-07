using Systems.Distractions;

namespace Systems.GameMessages.Messages
{
    public class ShowDeadPersonMessageAction
    {
        public string TouristName { get; set; }
        public int TouristFaceIndex { get; set; }
        public DistractionType DistractionType { get; set; }
    }
}
