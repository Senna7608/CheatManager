﻿using SMLHelper.V2.Assets;
using SMLHelper.V2.MonoBehaviours;
using UnityEngine;

namespace Crowbar
{
    internal class CrowbarPrefab : ModPrefab
    {
        public CrowbarPrefab(string classId, string prefabFileName, TechType techType = TechType.None) : base(classId, prefabFileName, techType)
        {
            ClassID = classId;
            PrefabFileName = prefabFileName;
            TechType = techType;
        }
        
        public override GameObject GetGameObject()
        {
            GameObject gameObject = Main.assetBundle.LoadAsset<GameObject>("Crowbar");                      

            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
            {
                renderer.material.shader = Shader.Find("MarmosetUBER");
                renderer.material.SetColor("_Emission", new Color(1f, 1f, 1f));
            }
            
            gameObject.AddComponent<PrefabIdentifier>().ClassId = ClassID;
            gameObject.AddComponent<TechTag>().type = TechType;            
            gameObject.AddComponent<BoxCollider>().size = new Vector3(0.01f, 0.01f, 0.01f);
            //gameObject.AddComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Near;
            gameObject.AddComponent<Pickupable>().isPickupable = true;            
            
            Fixer fixer = gameObject.AddComponent<Fixer>();
            fixer.ClassId = ClassID;
            fixer.techType = TechType;

            SkyApplier skyApplier = gameObject.AddComponent<SkyApplier>();
            skyApplier.renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            skyApplier.anchorSky = Skies.Auto;

            WorldForces worldForces = gameObject.AddOrGetComponent<WorldForces>();
            Rigidbody rigidbody = gameObject.AddOrGetComponent<Rigidbody>();
            
            worldForces.underwaterGravity = 0;
            worldForces.useRigidbody = rigidbody;

            VFXFabricating vfxFabricating = gameObject.AddComponent<VFXFabricating>();
            vfxFabricating.localMinY = -3f;
            vfxFabricating.localMaxY = 3f;
            vfxFabricating.posOffset = new Vector3(0f, 0, 0f);
            vfxFabricating.eulerOffset = new Vector3(0f, 90f, -90f);
            vfxFabricating.scaleFactor = 1f;

            Crowbar component = gameObject.AddComponent<Crowbar>();

            var knifePrefab = Resources.Load<GameObject>("WorldEntities/Tools/Knife").GetComponent<Knife>();

            component.attackSound = Object.Instantiate(knifePrefab.attackSound, gameObject.transform);
            component.underwaterMissSound = Object.Instantiate(knifePrefab.underwaterMissSound, gameObject.transform);
            component.surfaceMissSound = Object.Instantiate(knifePrefab.surfaceMissSound, gameObject.transform);

            return gameObject;
        }        
    }
}
