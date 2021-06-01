using Systems.Distractions2;

namespace Systems.GameMessages.Messages
{
    public class ShowDeadPersonMessageAction
    {
        public string TouristName { get; set; }
        public int TouristFaceIndex { get; set; }
        public DistractionType DistractionType { get; set; }
    }
}
