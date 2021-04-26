using SystemBase;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Systems.Cursor
{
    [GameSystem]
    public class CursorSystem : GameSystem<CursorComponent>
    {
        public override void Register(CursorComponent component)
        {
            UnityEngine.Cursor.visible = false;
            component.SpriteRenderer = component.GetComponent<SpriteRenderer>();

            var floorLayer = LayerMask.NameToLayer("Floor");
            var touristLayer = LayerMask.NameToLayer("Tourist");
            component.UpdateAsObservable().Subscribe(_ => UpdateCursorIcon(component, floorLayer, touristLayer))
                .AddTo(component);
        }

        private void UpdateCursorIcon(CursorComponent component, int floorLayer, int touristLayer)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out _, Mathf.Infinity, 1 << touristLayer))
            {
                SetCustomCursor(component, component.interactionCursor);
            }
            else if (Physics.Raycast(ray, out _, Mathf.Infinity, 1 << floorLayer))
            {
                SetCustomCursor(component, component.moveCursor);
            }
            else
            {
                SetCustomCursor(component, component.defaultCursor);
            }
        }

        private static void SetCustomCursor(CursorComponent component, Sprite spriteRendererSprite)
        {
            component.SpriteRenderer.sprite = spriteRendererSprite;
            component.transform.position = (Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
