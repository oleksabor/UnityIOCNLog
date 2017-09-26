using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.ObjectBuilder;

namespace Common.Log
{
	/// <summary>
	/// This class decouples the following classes from any specific logging framework.
	/// </summary>
	public interface ILogFactory
	{
		/// 
		/// This method must return an instance of a Logger.
		/// 
		/// The type of the class that is using the logger.
		/// An instance of a logger.
		object GetLogger(Type parentType);
	}

	//http://unity.codeplex.com/discussions/203744 SCDeveloper
	public class LogCreation<T, U> : UnityContainerExtension where U : ILogFactory, new()
	{

		protected override void Initialize()
		{
			this.Context.Strategies.AddNew<LogCreationStrategy<T, U>>(UnityBuildStage.PreCreation);
		}
	}

	public class LogCreationStrategy<T, U> : BuilderStrategy where U : ILogFactory, new()
	{

		public bool IsPolicySet { get; private set; }

		public override void PreBuildUp(IBuilderContext context)
		{
			Type typeToBuild = context.BuildKey.Type;
			if (typeof(T).Equals(typeToBuild))
			{

				if (context.Policies.Get<LogBuildPlanPolicy>(context.BuildKey) == null)
				{
					Type typeForLog = GetLogType(context);
					ILogFactory factory = new U();
					IBuildPlanPolicy policy = new LogBuildPlanPolicy(typeForLog, factory);
					context.Policies.Set(policy, context.BuildKey);

					this.IsPolicySet = true;
				}
			}
		}

		public override void PostBuildUp(IBuilderContext context)
		{
			if (this.IsPolicySet)
			{
				context.Policies.Clear<LogBuildPlanPolicy>(context.BuildKey);
				this.IsPolicySet = false;
			}
		}

		private static Type GetLogType(IBuilderContext context)
		{
			Type logType = typeof(T);

			IBuildTrackingPolicy buildTrackingPolicy = BuildTracking.GetPolicy(context);
			if ((buildTrackingPolicy != null) && (buildTrackingPolicy.BuildKeys.Count >= 2))
			{
				logType = ((NamedTypeBuildKey)buildTrackingPolicy.BuildKeys.ElementAt(1)).Type;
			}
			else
			{
				StackTrace stackTrace = new StackTrace();
				//first two are in the log creation strategy, can skip over them
				for (int i = 2; i < stackTrace.FrameCount; i++)
				{
					StackFrame frame = stackTrace.GetFrame(i);
					logType = frame.GetMethod().DeclaringType;
					//Console.WriteLine(logType.FullName);
					if (!logType.FullName.StartsWith("Microsoft.Practices"))
					{
						break;
					}
				}
			}

			return logType;
		}
	}

	public class LogBuildPlanPolicy : IBuildPlanPolicy
	{
		private ILogFactory logFactory;
		public LogBuildPlanPolicy(Type logType, ILogFactory factory)
		{
			this.LogType = logType;
			logFactory = factory;
		}

		public Type LogType { get; private set; }

		public void BuildUp(IBuilderContext context)
		{
			if (context.Existing == null)
			{
				context.Existing = logFactory.GetLogger(this.LogType);
			}
		}
	}
}
