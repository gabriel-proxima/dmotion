﻿using System.Management.Instrumentation;
using DMotion.Authoring;
using Latios.Kinemation;
using NUnit.Framework;
using Unity.Entities;

namespace DMotion.Tests
{
    public static class StateMachineTestUtils
    {
        public static Entity CreateStateMachineEntity(this EntityManager manager, StateMachineAsset stateMachineAsset,
            BlobAssetReference<StateMachineBlob> stateMachineBlob)
        {
            var entity = manager.CreateEntity();
            AnimationStateMachineConversionUtils.AddStateMachineSystemComponents(manager, entity, stateMachineAsset,
                stateMachineBlob,
                BlobAssetReference<SkeletonClipSetBlob>.Null, BlobAssetReference<ClipEventsBlob>.Null);
            AnimationStateMachineConversionUtils.AddAnimationStateSystemComponents(manager, entity);
            return entity;
        }

        public static Entity InstantiateStateMachineEntity(this EntityManager manager, Entity prefab)
        {
            var newEntity = manager.Instantiate(prefab);
            Assert.IsTrue(manager.HasComponent<AnimationStateMachine>(newEntity));
            return newEntity;
        }

        public static void RequestTransitionToStateMachine(EntityManager manager, Entity entity)
        {
            manager.SetComponentData(entity, AnimationStateMachineTransitionRequest.New(0.15f));
        }

        public static bool ShouldStateMachineBeActive(EntityManager manager, Entity entity)
        {
            Assert.IsTrue(manager.HasComponent<AnimationCurrentState>(entity));
            Assert.IsTrue(manager.HasComponent<AnimationStateTransition>(entity));
            Assert.IsTrue(manager.HasComponent<AnimationStateTransitionRequest>(entity));
            Assert.IsTrue(manager.HasComponent<AnimationStateMachine>(entity));
            
            var animationCurrentState = manager.GetComponentData<AnimationCurrentState>(entity);
            var animationStateTransition = manager.GetComponentData<AnimationStateTransition>(entity);
            var stateMachineTransitionRequest = manager.GetComponentData<AnimationStateMachineTransitionRequest>(entity);
            var stateMachine = manager.GetComponentData<AnimationStateMachine>(entity);
            return UpdateStateMachineJob.ShouldStateMachineBeActive(animationCurrentState, animationStateTransition,
                stateMachineTransitionRequest, stateMachine.CurrentState);
        }
        internal static StateMachineStateRef GetCurrentState(EntityManager manager, Entity entity)
        {
            return manager.GetComponentData<AnimationStateMachine>(entity).CurrentState;
        }


        public static void SetBoolParameter(this EntityManager manager, Entity entity, int index, bool newValue)
        {
            Assert.IsTrue(manager.HasComponent<BoolParameter>(entity));
            var boolParameters = manager.GetBuffer<BoolParameter>(entity);
            Assert.IsTrue(boolParameters.Length > 0);
            var parameter = boolParameters[index];
            parameter.Value = newValue;
            boolParameters[index] = parameter;
        }

        public static void SetIntParameter(this EntityManager manager, Entity entity, int index, int newValue)
        {
            Assert.IsTrue(manager.HasComponent<IntParameter>(entity));
            var intParameters = manager.GetBuffer<IntParameter>(entity);
            Assert.IsTrue(intParameters.Length > 0);
            var parameter = intParameters[index];
            parameter.Value = newValue;
            intParameters[index] = parameter;
        }

        public static void SetParameter<TBuffer, TValue>(this EntityManager manager, Entity entity, int hash,
            TValue newValue)
            where TBuffer : struct, IBufferElementData, IStateMachineParameter<TValue>
            where TValue : struct
        {
            Assert.IsTrue(manager.HasComponent<TBuffer>(entity));
            var parameters = manager.GetBuffer<TBuffer>(entity);
            Assert.IsTrue(parameters.Length > 0);
            parameters.SetParameter(hash, newValue);
        }
    }
}