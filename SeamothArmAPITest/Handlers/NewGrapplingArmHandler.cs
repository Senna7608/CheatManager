﻿using ModdedArmsHelper.API;
using ModdedArmsHelper.API.ArmHandlers;
using ModdedArmsHelper.API.Interfaces;
using UnityEngine;

namespace SeamothArmAPITest.Handlers
{
    public class NewGrapplingArmHandler : SeamothGrapplingArmHandler, ISeamothArm
    {
        public override void Start()
        {
        }

        GameObject ISeamothArm.GetGameObject()
        {
            return gameObject;
        }

        void ISeamothArm.SetSide(SeamothArm arm)
        {
            if (arm == SeamothArm.Right)
            {
                transform.localScale = new Vector3(-0.80f, 0.80f, 0.80f);
            }
            else
            {
                transform.localScale = new Vector3(0.80f, 0.80f, 0.80f);
            }
        }

        bool ISeamothArm.OnUseDown(out float cooldownDuration)
        {
            Animator.SetBool("use_tool", true);

            if (!rope.isLaunching)
            {
                rope.LaunchHook(maxDistance);
            }

            cooldownDuration = 2f;

            return true;
        }

        bool ISeamothArm.OnUseHeld(out float cooldownDuration)
        {
            cooldownDuration = 0f;
            return false;
        }

        bool ISeamothArm.OnUseUp(out float cooldownDuration)
        {
            Animator.SetBool("use_tool", false);
            ResetHook();
            cooldownDuration = 0f;
            return true;
        }

        bool ISeamothArm.OnAltDown()
        {
            return false;
        }

        void ISeamothArm.Update(ref Quaternion aimDirection)
        {
        }

        void ISeamothArm.Reset()
        {
            Animator.SetBool("use_tool", false);
            ResetHook();
        }

        private void OnDestroy()
        {
            if (hook)
            {
                Destroy(hook.gameObject);
            }
            if (rope != null)
            {
                Destroy(rope.gameObject);
            }

        }

        private void ResetHook()
        {
            rope.Release();
            hook.Release();
            hook.SetFlying(false);
            hook.transform.parent = front;
            hook.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        public void OnHit()
        {
            hook.transform.parent = null;
            hook.transform.position = front.transform.position;
            hook.SetFlying(true);
            GameObject x = null;
            Vector3 a = default(Vector3);

            UWE.Utils.TraceFPSTargetPosition(Seamoth.gameObject, 100f, ref x, ref a, false);

            if (x == null || x == hook.gameObject)
            {
                a = MainCamera.camera.transform.position + MainCamera.camera.transform.forward * 25f;
            }

            Vector3 a2 = Vector3.Normalize(a - hook.transform.position);

            hook.rb.velocity = a2 * 25f;
            Utils.PlayFMODAsset(shootSound, front, 15f);

            grapplingStartPos = Seamoth.transform.position;
        }

        public void FixedUpdate()
        {
            if (hook.attached)
            {
                grapplingLoopSound.Play();

                Vector3 value = hook.transform.position - front.position;
                Vector3 a = Vector3.Normalize(value);
                float magnitude = value.magnitude;

                if (magnitude > 1f)
                {
                    if (!IsUnderwater() && Seamoth.transform.position.y + 0.2f >= grapplingStartPos.y)
                    {
                        a.y = Mathf.Min(a.y, 0f);
                    }

                    Seamoth.GetComponent<Rigidbody>().AddForce(a * seamothGrapplingAccel, ForceMode.Acceleration);
                    hook.GetComponent<Rigidbody>().AddForce(-a * 400f, ForceMode.Force);
                }

                rope.SetIsHooked();
            }
            else if (hook.flying)
            {
                if ((hook.transform.position - front.position).magnitude > maxDistance)
                {
                    ResetHook();
                }
                grapplingLoopSound.Play();
            }
            else
            {
                grapplingLoopSound.Stop();
            }
        }

        bool ISeamothArm.HasClaw()
        {
            return false;
        }

        bool ISeamothArm.HasDrill()
        {
            return false;
        }

        bool ISeamothArm.HasPropCannon()
        {
            return false;
        }

        void ISeamothArm.SetRotation(SeamothArm arm, bool isDocked)
        {
            if (Seamoth == null)
            {
                return;
            }

            if (isDocked)
            {
                SubRoot subRoot = Seamoth.GetComponentInParent<SubRoot>();

                if (subRoot.isCyclops)
                {
                    if (arm == SeamothArm.Right)
                    {
                        transform.localRotation = Quaternion.Euler(32, 336, 310);
                    }
                    else
                    {
                        transform.localRotation = Quaternion.Euler(32, 24, 50);
                    }
                }
                else
                {
                    transform.localRotation = Quaternion.Euler(346, 0, 0);
                }
            }
            else
            {

                transform.localRotation = Quaternion.Euler(346, 0, 0);
            }
        }

        float ISeamothArm.GetEnergyCost()
        {
            return energyCost;
        }

        private bool IsUnderwater()
        {
            return Seamoth.transform.position.y < worldForces.waterDepth + 2f && !Seamoth.precursorOutOfWater;
        }

        public bool GetIsGrappling()
        {
            return hook != null && hook.attached;
        }
    }
}
