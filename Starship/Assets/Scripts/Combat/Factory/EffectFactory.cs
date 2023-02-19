﻿using Combat.Component.Body;
using Combat.Effects;
using Combat.Helpers;
using Combat.Scene;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.ObjectPool;
using System;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class EffectFactory
    {
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly PrefabCache _prefabCache;
        [Inject] private readonly IScene _scene;

        public IEffect CreateEffect(string name, Sprite sprite, IBody parent = null)
        {
            var gameObject = CreateGameObject(name);
            if (gameObject == null)
                return new EmptyEffect();

            var renderer = gameObject.GetComponent<SpriteRenderer>();
            if (renderer)
                renderer.sprite = sprite;

            return CreateEffect(gameObject, parent);
        }

        public IEffect CreateEffect(PrefabId prefabId, IBody parent = null)
        {
            var gameObject = CreateGameObject(prefabId);
            return gameObject != null ? CreateEffect(gameObject, parent) : new EmptyEffect();
        }

        public IEffect CreateEffect(GameDatabase.DataModel.VisualEffectElement effectData, Transform parent)
        {
            if (effectData.Type == VisualEffectType.Shake)
                return new ShakeEffect(_scene, parent);

            var prefab = _prefabCache.GetEffectPrefab(effectData);
            if (!prefab) return new EmptyEffect();

            var gameObject = new GameObjectHolder(prefab, _objectPool, false);

            gameObject.Transform.parent = parent;
            gameObject.Transform.localPosition = Vector3.zero;
            return CreateEffect(gameObject, null);
        }

        public IEffect CreateEffect(string name, IBody parent = null)
        {
            var gameObject = CreateGameObject(name);
            return gameObject != null ? CreateEffect(gameObject, parent) : new EmptyEffect();
        }

        public IEffect CreateDamageTextEffect(float damage, Color color, Vector2 position, Vector2 velocity, IBody parent = null)
        {
            var gameObject = CreateGameObject("DamageText");

            var direction = RotationHelpers.Direction(UnityEngine.Random.Range(0f, 360f));

            string damageText;
            if (damage < 1e3f)
                damageText = ((int)damage).ToString();
            else if (damage < 1e6f)
                damageText = (int)(damage / 1e3f) + "K";
            else if (damage < 1e9f)
                damageText = (int)(damage / 1e6f) + "M";
            else if (damage < 1e12f)
                damageText = (int)(damage / 1e9f) + "B";
            else
                damageText = (int)(damage / 1e12f) + "T";
            
            gameObject.Name = damageText;
            var effect = CreateEffect(gameObject, parent);
            effect.Position = position;
            effect.Color = color;
            effect.Run(2.0f, velocity + direction, 0);

            return effect;
        }

        public void CreateDisturbance(Vector2 position, float power)
        {
            var distance = Vector2.Distance(position, _scene.ViewPoint) + 20;
            _scene.Shake(Mathf.Min(2.0f * power / Mathf.Sqrt(distance), 5f));
        }

        private GameObjectHolder CreateGameObject(string name)
        {
            var prefab = _prefabCache.LoadResourcePrefab("Combat/Effects/" + name);
            if (prefab == null)
            {
                UnityEngine.Debug.Log("Effect not found: Combat/Effects/" + name);
                return null;
            }

            return new GameObjectHolder(prefab, _objectPool);
        }

        private GameObjectHolder CreateGameObject(PrefabId prefabId)
        {
            var prefab = _prefabCache.LoadPrefab(prefabId);
            if (prefab == null)
            {
                UnityEngine.Debug.Log("Effect not found: " + prefabId);
                return null;
            }

            return new GameObjectHolder(prefab, _objectPool, false);
        }

        public IEffect CreateEffect(GameObjectHolder gameObject, IBody parent)
        {
            if (parent != null)
                parent.AddChild(gameObject.Transform);

            var effect = gameObject.GetComponent<IEffectComponent>();
            if (effect == null) return new EmptyEffect();

            effect.Initialize(gameObject);
            return effect;
        }
    }
}
