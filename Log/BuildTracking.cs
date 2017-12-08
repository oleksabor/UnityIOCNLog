using System;
using System.Collections.Generic;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Extension;
using Unity.Policy;
using Unity.Strategy;

namespace Common.Log
{

	//http://blog.baltrinic.com/software-development/dotnet/log4net-integration-with-unity-ioc-container
	//http://unity.codeplex.com/discussions/203744
	public class BuildTracking : UnityContainerExtension
	{

		protected override void Initialize()
		{
			Context.Strategies.AddNew<BuildTrackingStrategy>(UnityBuildStage.TypeMapping);
		}

		public static IBuildTrackingPolicy GetPolicy(IBuilderContext context)
		{
			return context.Policies.Get<IBuildTrackingPolicy>(context.BuildKey);
		}

		public static IBuildTrackingPolicy SetPolicy(IBuilderContext context)
		{
			IBuildTrackingPolicy policy = new BuildTrackingPolicy();
			context.Policies.SetDefault(policy);
			return policy;
		}
	}

	public class BuildTrackingStrategy : BuilderStrategy
	{

		public override void PreBuildUp(IBuilderContext context)
		{
			var policy = BuildTracking.GetPolicy(context)
				?? BuildTracking.SetPolicy(context);

			policy.BuildKeys.Push(context.BuildKey);
		}

		public override void PostBuildUp(IBuilderContext context)
		{
			IBuildTrackingPolicy policy = BuildTracking.GetPolicy(context);
			if ((policy != null) && (policy.BuildKeys.Count > 0))
			{
				policy.BuildKeys.Pop();
			}
		}
	}

	public interface IBuildTrackingPolicy : IBuilderPolicy
	{
		Stack<object> BuildKeys { get; }
	}

	public class BuildTrackingPolicy : IBuildTrackingPolicy
	{

		public BuildTrackingPolicy()
		{
			BuildKeys = new Stack<object>();
		}

		public Stack<object> BuildKeys { get; private set; }
	}
}
