using SystemBase;
using UnityEngine;

namespace Systems.Cursor
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CursorComponent : GameComponent
    {
        public Sprite defaultCursor;
        public Sprite interactionCursor;
        public Sprite moveCursor;

        public SpriteRenderer SpriteRenderer { get; set; }
    }
}