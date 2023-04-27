using System;
using Unity.VisualScripting;
using UnityEngine;

namespace _3C.Player
{
    public class PlayerStateBehavior
    {
        protected IStateHandler m_StateHandler;

        public void Awake(IStateHandler _stateHandler)
        {
            m_StateHandler = _stateHandler;
            Init(_stateHandler);
        }

        protected virtual void Init(IStateHandler _stateHandler) {}
        
        public virtual void StartState(PlayerState _previousState) {}

        public virtual void StopState() {}
        
        public virtual void Update() {}

        public virtual void OnValidate() {}

        public virtual void OnInput(InputType inputType) { }
    }
}