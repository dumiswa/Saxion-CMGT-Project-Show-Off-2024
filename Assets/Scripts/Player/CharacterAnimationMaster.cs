using System;
using System.Collections.Generic;
using UnityEngine;

namespace Monoliths.Player
{

    public class CharacterAnimationMaster : Monolith
    {
        public const string CHARACTER_ANIMATOR_STACK_DATA_ID = "CharacterAnimatorStack";

        private CharacterAnimatorStackPacket _dataPacket;
        private Transform _cameraYaw;
       
        public override void Defaults()
        {
            base.Defaults();
            _priority = -2;
        }
        public override bool Init()
        {
            _cameraYaw = Camera.main.transform.parent.parent;

            var player = GameObject.FindGameObjectWithTag("Player")
                .transform.Find("Display").GetComponent<Animator>();

            _dataPacket = new CharacterAnimatorStackPacket();
            _dataPacket.RegisterPlayerSpecific(player);
            _dataPacket.TryRegisterStandardPack(player);

            DataBridge.UpdateData(CHARACTER_ANIMATOR_STACK_DATA_ID, _dataPacket);
            return base.Init();
        }

        private void LateUpdate()
        {
            List<string> toRemove = new();
            foreach (var pair in _dataPacket.StandardPack) 
            {
                try
                {
                    CacheAnimationData(pair.Key, pair.Value);
                    var turn = CalculateTurn(pair.Key, pair.Value);
                    var prevTurn = pair.Value.GetInteger("Turn");
                    if (prevTurn != turn)
                    {
                        pair.Value.SetFloat
                        (
                            "CycleOffset",
                            pair.Value.GetFloat("CycleOffsetBuffer")
                        );
                    }
                    pair.Value.SetInteger("Turn", turn);

                    var offset = pair.Value.GetFloat("CycleOffsetBuffer");
                    if (offset >= int.MaxValue - 1)
                        offset = 0;

                    offset += Time.deltaTime;
                    pair.Value.SetFloat("CycleOffsetBuffer", offset);
                }
                catch (MissingReferenceException)
                {
                    toRemove.Add(pair.Key);
                }
                catch (NullReferenceException)
                {
                    toRemove.Add(pair.Key);
                }
            }
            foreach (var key in toRemove)
                _dataPacket.StandardPack.Remove(key);   
        }

        private void CacheAnimationData(string key, Animator animator)
        {
            var data = DataBridge.TryGetData<CharacterAnimatorData>(key);
            if (data.IsEmpty)
                return;

            var cache = data.EncodedData;
            var motion = animator.transform.position - cache.Derivative;

            cache.Derivative = animator.transform.position;

            if (new Vector2(motion.x, motion.z).magnitude < 0.25f)
                return;

            cache.Motion = motion;

            DataBridge.UpdateData(key, cache);
        }
        private int CalculateTurn(string key, Animator animator)
        {
            const float OFFSET = 0.251f;
            const float INV_DOUBLE_PI = 1f / (2f * Mathf.PI);

            var cache = DataBridge.TryGetData<CharacterAnimatorData>(key).EncodedData;
            var normalizedMotion = cache.Motion.normalized;

            float signedMotionAngle = ((Mathf.Atan2(normalizedMotion.z, normalizedMotion.x) +
                                        animator.transform.eulerAngles.y * Mathf.Deg2Rad) *
                                        INV_DOUBLE_PI + OFFSET) % 1f;

            float turn = 9f - signedMotionAngle * 8f;
            return Mathf.RoundToInt(turn > 8f ? turn - 8f : turn);
        }

        private void Scan()
        {
            var suitables = GameObject.FindGameObjectsWithTag("2DCharacter");
            foreach (var suitable in suitables)
            {
                suitable.TryGetComponent(out Animator animator);
                if(animator is null)
                {
                    Debug.Log($"GameObject named \"{suitable.name}\" had no animator");
                    continue;
                }
                _dataPacket.TryRegisterStandardPack(animator);
            }
        }

        private void OnGameStateEnter(GameState state)
        {
            switch (state)
            {
                case LevelStartState:
                    Scan();
                    break;
                case LevelFinishState:
                    _dataPacket.Defaults();
                    break;
                default:
                    break;
            }
        }

        private void OnEnable() => GameState.OnEnter += OnGameStateEnter;
        private void OnDisable() => GameState.OnEnter -= OnGameStateEnter;
    }
}
