using Combat.Component.Body;
using Combat.Component.Systems.Weapons;
using Combat.Effects;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Combat.Component.Triggers.EffectMove
{
    public class EffectMoveTarget: MonoBehaviour
    {
        public IBody targetbody;
        public float lifetime;
        private void Start()
        {
            _lifetime = lifetime;
        }
        private void Update()
        {
            var nowposition = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
            var dir = targetbody.WorldPosition().Direction(nowposition).normalized;
            var dis = targetbody.WorldPosition().Distance(nowposition) * (1 - _lifetime / lifetime);
            var newposition = nowposition - dir * dis;
            gameObject.transform.position = newposition;

            if (_lifetime <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                _lifetime -= Time.deltaTime;
            }

        }

        private float _lifetime;
    }
}
