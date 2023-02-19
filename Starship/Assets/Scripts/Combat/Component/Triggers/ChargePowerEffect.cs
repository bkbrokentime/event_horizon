using Combat.Component.Body;
using Combat.Component.Systems.Weapons;
using Combat.Effects;
using ModestTree;
using UnityEngine;
using Zenject;
using Combat.Component.Triggers.EffectMove;

namespace Combat.Component.Triggers
{
    public class ChargePowerEffect : IUnitEffect
    {

        public ChargePowerEffect(IWeapon weapon, IEffect effect, Vector2 position, float lifetime, ConditionType conditionType,PrefabCache prefabCache)
        {
            _effect = effect;
            _effect.Life = 0;
            _position = position;
            _weapon = weapon;
            _lifetime = lifetime;
            TriggerCondition = conditionType;

            _nexttime = 0;
            _prefabCache = prefabCache;
        }

        public ConditionType TriggerCondition { get; private set; }

        public bool TryUpdateEffect(float elapsedTime)
        {
            if (_weapon.PowerLevel <= 0 && _effect.Life <= 0)
                return false;

            var life = _weapon.PowerLevel;
            _effect.Life = _effect.Life < life ? life : Mathf.MoveTowards(_effect.Life, life, elapsedTime / _lifetime);

            var body = _weapon.Platform.Body;
            _effect.Rotation = body.WorldRotation();
            var position = body.WorldPosition() + RotationHelpers.Transform(_position, body.WorldRotation()) * body.WorldScale();

            _effect.Position = position;


/*
            for (int i = 0; i < _effects.Length; i++)
            {
                var nowposition = new Vector2(_effects[i].effect.transform.position.x, _effects[i].effect.transform.position.y);
                var dir = body.WorldPosition().Direction(nowposition).normalized;
                var dis = body.WorldPosition().Distance(nowposition) * elapsedTime;
                var newposition = nowposition + dir * dis;
                _effects[i].effect.transform.position = newposition;
                _effects[i].updatelifetime(elapsedTime);
                if (_effects[i].lifetime <= 0)
                {
                    GameObject.Destroy(_effects[i].effect);
                    _effects[i] = new effectstruct();
                }
            }

            if (_effects.Length > 0)
                if (_effects[0].effect == null)
                {
                    for (int i = 1; i < _effects.Length; i++)
                        _effects[i - 1] = _effects[i];
                    _effects[_effects.Length - 1] = new effectstruct();
                }*/
            _nexttime -= elapsedTime;

            if (_nexttime <= 0)
            {
                for (int i = 0; i < 1 + (int)(_weapon.PowerLevel * 4); i++)
                {
                    var centerposition = new Vector3(body.WorldPosition().x, body.WorldPosition().y, 0);
                    var effectposition = centerposition + new Vector3((Random.value - 0.5f) * _effect.Size * (4 + _weapon.PowerLevel * 4), (Random.value - 0.5f) * _effect.Size * (4 + _weapon.PowerLevel * 4), 0);
                    var obj = GameObject.Instantiate(_prefabCache.LoadResourcePrefab("Combat/Effects/OrbAdditive"), effectposition, Quaternion.Euler(0, 0, 0));
                    obj.GetComponent<SpriteRenderer>().color = _effect.Color;
                    obj.transform.localScale = new Vector2(_effect.Size / 2, _effect.Size / 2);
                    obj.AddComponent<EffectMoveTarget>();
                    obj.GetComponent<EffectMoveTarget>().targetbody = body;
                    obj.GetComponent<EffectMoveTarget>().lifetime = 1 + _effect.Life * 2;
                }
                /*
                var objstruct = new effectstruct() { effect = obj, lifetime = _effecttime };
                if (_effects.Length > 0)
                {
                    if (_effects[_effects.Length - 1].effect == null)
                        _effects[_effects.Length - 1] = objstruct;
                    else
                        _effects.Add(objstruct);
                }
                else
                    _effects.Add(objstruct);
                */
                _nexttime = 0.25f / (1 + 4 * _weapon.PowerLevel);
            }


            return true;
        }

        public bool TryInvokeEffect(ConditionType condition)
        {
            _effect.Life = _weapon.PowerLevel;
            return true;
        }

        public void Dispose()
        {
            _effect.Dispose();
        }

        private float _nexttime;

        private readonly PrefabCache _prefabCache;

        private readonly Vector2 _position;
        private readonly float _lifetime;
        private readonly IEffect _effect;
        private readonly IWeapon _weapon;
    }
}
