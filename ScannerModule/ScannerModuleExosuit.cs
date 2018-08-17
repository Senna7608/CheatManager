﻿using UnityEngine;

namespace ScannerModule
{
    public class ScannerModuleExosuit : MonoBehaviour
    {
        // Some code extracted with dnSpy from Assembly-CSharp.dll:ScannerTool 
        
        private EnergyMixin energyMixin;
        
        public const float powerConsumption = 0.5f;
        public const float scanDistance = 20f;               
        public FMOD_CustomLoopingEmitter scanSound;
        public FMODAsset completeSound;
        private bool isScanning = false;
        public bool toggle;
        private ScanState stateLast;
        private ScanState stateCurrent;
        private float idleTimer;
        private Exosuit exosuit;
        HandReticle main = HandReticle.main;

        
        public void Awake()
        {
            exosuit = gameObject.GetComponent<Exosuit>();            
        }        
        
        public enum ScanState
        {
            None,
            Scan
        }

        public void Start()
        {            
            energyMixin = GetComponent<EnergyMixin>();            
            var scanner = Resources.Load<GameObject>("WorldEntities/Tools/Scanner").GetComponent<ScannerTool>();

            scanSound = Instantiate(scanner.scanSound, gameObject.transform);            
            completeSound = Instantiate(scanner.completeSound, gameObject.transform);            
        }        

        private void Update()
        {
            if (toggle)
            {
                PDAScanner.Result result = Scan();
                Debug.Log($"Scanner: toggle: {toggle} isScanning: {isScanning}");

                if (!isScanning)
                {
                    if (result != PDAScanner.Result.Known)
                    {                        
                        main.SetInteractText("AutoScan", "Active", false, false, HandReticle.Hand.None);
                        main.SetIcon(HandReticle.IconType.Scan, 1.5f);
                    }
                }
                else
                {
                    main.SetInteractText(PDAScanner.scanTarget.techType.AsString(false), true, HandReticle.Hand.None);
                    main.SetIcon(HandReticle.IconType.Progress, 10f);
                    main.progressText.text = Mathf.RoundToInt(PDAScanner.scanTarget.progress * 100f) + "%";
                    main.progressText.color = new Color32(0, 226, 32, byte.MaxValue);
                    main.progressImage.color = new Color32(0, 226, 32, byte.MaxValue);
                    main.progressImage.fillAmount = Mathf.Clamp01(PDAScanner.scanTarget.progress);
                    main.SetProgress(PDAScanner.scanTarget.progress);                    
                }

                if (idleTimer > 0f)
                {
                    idleTimer = Mathf.Max(0f, idleTimer - Time.deltaTime);
                }

                
            }            
        }       

        private void LateUpdate()
        {
            if (toggle)
            {
                isScanning = stateCurrent == ScanState.Scan;                

                if (idleTimer <= 0f)
                {
                    OnHover();
                }                

                if (isScanning)
                {
                    scanSound.Play();
                }
                else
                {
                    scanSound.Stop();
                }
                stateLast = stateCurrent;
                stateCurrent = ScanState.None;
            }
        } 
                
        private PDAScanner.Result Scan()
        {
            if (stateCurrent != ScanState.None)
            {
                return PDAScanner.Result.None;
            }
            if (idleTimer > 0f)
            {
                return PDAScanner.Result.None;
            }

            PDAScanner.Result result = PDAScanner.Result.None;
            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;            

            if (scanTarget.isValid && energyMixin.charge > 0f)
            {
                result = PDAScanner.Scan();
                
                if (result == PDAScanner.Result.Scan)
                {
                    float amount = powerConsumption * Time.deltaTime;
                    energyMixin.ConsumeEnergy(amount);
                    stateCurrent = ScanState.Scan;
                    isScanning = true;
                }
                else if (result == PDAScanner.Result.Done || result == PDAScanner.Result.Researched)
                {                    
                    idleTimer = 0.5f;
                    PDASounds.queue.PlayIfFree(completeSound);                    
                }                
            }
            return result;
        }        

        private void OnHover()
        {
            if (energyMixin.charge <= 0f)
            {                
                return;
            }

            PDAScanner.ScanTarget scanTarget = PDAScanner.scanTarget;
            
            PDAScanner.UpdateTarget(scanDistance, false);

            if (!scanTarget.isValid)
            {                
                return;
            }

            PDAScanner.Result result = PDAScanner.CanScan();
            /*
            if (result == PDAScanner.Result.Scan)
            {                
                main.SetInteractText(scanTarget.techType.AsString(false), true, HandReticle.Hand.None);

                if (stateCurrent == ScanState.Scan)
                {                       
                    main.SetIcon(HandReticle.IconType.Progress, 10f);
                    main.progressText.text = Mathf.RoundToInt(PDAScanner.scanTarget.progress * 100f) + "%";
                    main.progressText.color = new Color32(0, 226, 32, byte.MaxValue);
                    main.progressImage.color = new Color32(0, 226, 32, byte.MaxValue);
                    main.progressImage.fillAmount = Mathf.Clamp01(PDAScanner.scanTarget.progress);                    
                    main.SetProgress(PDAScanner.scanTarget.progress);                    
                }
            }
            */           
        }       
    }
}
