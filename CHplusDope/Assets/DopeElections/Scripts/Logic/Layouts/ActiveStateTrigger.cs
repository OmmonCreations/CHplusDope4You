using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DopeElections.Layouts
{
    public class ActiveStateTrigger : MonoBehaviour
    {
        [SerializeField] private Condition[] _enableConditions = null;
        [SerializeField] private Condition[] _disableConditions = null;
        [SerializeField] private Condition[] _generalConditions = null;
        [SerializeField] private ConditionMode _conditionMode = ConditionMode.All;
        [SerializeField] private bool _conditionsInverted = false;
        [SerializeField] private UnityEvent _onEnable = new UnityEvent();
        [SerializeField] private UnityEvent _onDisable = new UnityEvent();

        private bool _enabled;
        
        public UnityEvent onEnable => _onEnable;
        public UnityEvent onDisable => _onDisable;

        private void OnEnable()
        {
            _enabled = true;
            var check = Check(_enableConditions.Concat(_generalConditions), _conditionMode);
            if (check != _conditionsInverted)
            {
                _onEnable.Invoke();
            }
        }

        private void OnDisable()
        {
            _enabled = false;
            var check = Check(_disableConditions.Concat(_generalConditions), _conditionMode);
            if (check != _conditionsInverted)
            {
                _onDisable.Invoke();
            }
        }

        public void TriggerUpdate()
        {
            if(_enabled) OnEnable();
            else OnDisable();
        }

        public enum ConditionMode
        {
            All,
            Any
        }

        private bool Check(IEnumerable<Condition> conditions, ConditionMode mode)
        {
            switch (mode)
            {
                case ConditionMode.All: return conditions.All(c => c.Check());
                case ConditionMode.Any: return conditions.Any(c => c.Check());
                default: return false;
            }
        }

        [Serializable]
        public class Condition
        {
            [SerializeField] private MonoBehaviour _target = null;
            [SerializeField] private Type _type = Type.Active;
            [SerializeField] private bool _expectedValue = true;

            public bool Check()
            {
                switch (_type)
                {
                    case Type.Enabled: return _target.enabled == _expectedValue;
                    case Type.ActiveAndEnabled: return _target.isActiveAndEnabled == _expectedValue;
                    case Type.Active: return _target.gameObject.activeSelf == _expectedValue;
                    case Type.ActiveInHierarchy: return _target.gameObject.activeInHierarchy == _expectedValue;
                    default: return false;
                }
            }

            public enum Type
            {
                Enabled,
                ActiveAndEnabled,
                Active,
                ActiveInHierarchy
            }
        }
    }
}