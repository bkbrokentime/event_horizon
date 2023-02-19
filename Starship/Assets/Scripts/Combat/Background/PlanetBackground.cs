using System;
using System.Reflection;
using Game.Exploration;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Combat.Background
{
    public class PlanetBackground : MonoBehaviour
    {
        [SerializeField] private float _size = 50f;
        [SerializeField] private Material _gasPlanetMaterial;
        [SerializeField] private Material _barrenPlanetMaterial;
        [SerializeField] private Material _infectedPlanetMaterial;
        [SerializeField] private Material _moltenPlanetMaterial;
        [SerializeField] private Material _frozenPlanetMaterial;

        [Inject]
        public void Initialize(IResourceLocator resourceLocator, Planet planet)
        {
            _width = _height = _size * Screen.width / Screen.height;
            _planet = planet;

            Primitives.CreatePlane(gameObject.GetMesh(), _width, _height, 6);

            switch (planet.Type)
            {
                case PlanetType.Gas:
                    InitializeGasMaterial();
                    break;
                case PlanetType.Infected:
                    InitializeInfectedMaterial(resourceLocator);
                    break;
                case PlanetType.Barren:
                case PlanetType.Terran:
                    InitializeBarrenMaterial(resourceLocator);
                    break;
                case PlanetType.Molten:
                    InitializeMoltenMaterial(resourceLocator);
                    break;
                case PlanetType.Frozen:
                    InitializeFrozenMaterial(resourceLocator);
                    break;
                default:
                    throw new ArgumentException("PlanetBackground: Wrong planet type - " + planet.Type);
            }
        }

        private void InitializeGasMaterial()
        {
            // Copy material to avoid modifying global material at runtime
            _gasPlanetMaterial = new Material(_gasPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _gasPlanetMaterial;

            //var random = new System.Random(planet.Seed);
            //_material.SetTexture("_DecalTex", resourceLocator.GetNebulaTexture(random.Next()));
            //_material.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _planet.TypeColor = _gasPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.75f);
        }

        private void InitializeBarrenMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _barrenPlanetMaterial = new Material(_barrenPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _barrenPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _barrenPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _planet.TypeColor = _barrenPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.3f);
        }

        private void InitializeMoltenMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _moltenPlanetMaterial = new Material(_moltenPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _moltenPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _moltenPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _planet.TypeColor = _moltenPlanetMaterial.color = Color.Lerp(_planet.Color, Color.red, 0.7f);
        }

        private void InitializeFrozenMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _frozenPlanetMaterial = new Material(_frozenPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _frozenPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _frozenPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _planet.TypeColor = _frozenPlanetMaterial.color = Color.Lerp(_planet.Color, Color.white, 0.5f);
        }

        private void InitializeInfectedMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _infectedPlanetMaterial = new Material(_infectedPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _infectedPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _infectedPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _planet.TypeColor = _infectedPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.3f);
        }

        private void LateUpdate()
        {
            switch (_planet.Type)
            {
                case PlanetType.Gas:
                    UpdateGasMaterial();
                    break;
                case PlanetType.Infected:
                    UpdateInfectedMaterial();
                    break;
                case PlanetType.Barren:
                case PlanetType.Terran:
                    UpdateBarrenMaterial();
                    break;
                case PlanetType.Molten:
                    UpdateMoltenMaterial();
                    break;
                case PlanetType.Frozen:
                    UpdateFrozenMaterial();
                    break;

            }
        }

        private void UpdateBarrenMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _barrenPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateMoltenMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _moltenPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateFrozenMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _frozenPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateInfectedMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _infectedPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateGasMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.mainTextureOffset = offset;

            var decalOffset = offset * 2;
            decalOffset.x -= Mathf.FloorToInt(offset.x);
            decalOffset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.SetTextureOffset("_DecalTex", decalOffset);

            var cloudOffset = offset * 3;
            cloudOffset.x -= Mathf.FloorToInt(offset.x);
            cloudOffset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.SetTextureOffset("_CloudsTex", cloudOffset);
        }

        private Planet _planet;
        private float _width;
        private float _height;
    }
}
