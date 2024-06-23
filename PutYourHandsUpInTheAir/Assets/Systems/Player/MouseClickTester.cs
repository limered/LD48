using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Player
{
    public class MouseClickTester
    {
        private static readonly int floorLayer = LayerMask.NameToLayer("Floor");
        private static readonly int distractionLayer = LayerMask.NameToLayer("Distraction");
        public bool IsLeftMouseClicked()
        {
            return Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        }

        public ClickedObject CheckMouseClick(out RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << distractionLayer))
            {
                return ClickedObject.Distraction;
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << floorLayer))
            {
                return ClickedObject.Ground;
            }

            return ClickedObject.Nothing;
        }
    }
    public enum ClickedObject
    {
        Ground, Distraction, Nothing
    }
}
