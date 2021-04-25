﻿using SystemBase;
using UniRx;
using UnityEngine;

namespace Systems.DistractionControl
{
    public class DistractionComponent : GameComponent
    {
        public DistractionType DistractionType;
        public float DistractionInteractionDuration;
        public Color DistractionProgressColor;
        public Transform InteractionPosition;

        public ReactiveCommand StartDistraction = new ReactiveCommand();

    }
}