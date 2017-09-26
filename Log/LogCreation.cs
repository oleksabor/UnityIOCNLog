//using System;
//using System.Diagnostics;
//using System.Linq;
//using Microsoft.Practices.ObjectBuilder2;
//using Microsoft.Practices.Unity;
//using Microsoft.Practices.Unity.ObjectBuilder;
//using NLog;

//namespace Common.Log
//{
//	//http://blog.baltrinic.com/software-development/dotnet/log4net-integration-with-unity-ioc-container
//	public class LogCreation : UnityContainerExtension
//	{
//		protected override void Initialize()
//		{
//			Context.Strategies.AddNew<LogCreationStrategy>(UnityBuildStage.PreCreation);
//		}
//	}

//	public class LogCreationStrategy : BuilderStrategy
//	{

//		public bool IsPolicySet { get; private set; }

//		public override void PreBuildUp(IBuilderContext context)
//		{
//			Type typeToBuild = context.BuildKey.Type;
//			if (typeof(ILogger).Equals(typeToBuild))
//			{

//				if (context.Policies.Get<IBuildPlanPolicy>(context.BuildKey) == null)
//				{
//					Type typeForLog = LogCreationStrategy.GetLogType(context);
//					IBuildPlanPolicy policy = new LogBuildPlanPolicy(typeForLog);
//					context.Policies.Set<IBuildPlanPolicy>(policy, context.BuildKey);

//					IsPolicySet = true;
//				}
//			}
//		}

//		public override void PostBuildUp(IBuilderContext context)
//		{
//			if (IsPolicySet)
//			{
//				context.Policies.Clear<IBuildPlanPolicy>(context.BuildKey);
//				IsPolicySet = false;
//			}
//		}

//		private static Type GetLogType(IBuilderContext context)
//		{
//			Type logType = null;
//			IBuildTrackingPolicy buildTrackingPolicy = BuildTracking.GetPolicy(context);
//			if ((buildTrackingPolicy != null) && (buildTrackingPolicy.BuildKeys.Count >= 2))
//			{
//				logType = ((NamedTypeBuildKey)buildTrackingPolicy.BuildKeys.ElementAt(1)).Type;
//			}
//			else
//			{
//				StackTrace stackTrace = new StackTrace();
//				//first two are in the log creation strategy, can skip over them
//				for (int i = 2; i < stackTrace.FrameCount; i++)
//				{
//					StackFrame frame = stackTrace.GetFrame(i);
//					logType = frame.GetMethod().DeclaringType;
//					if (!logType.FullName.StartsWith("Microsoft.Practices"))
//					{
//						break;
//					}
//				}
//			}
//			return logType;
//		}
//	}

//	public class LogBuildPlanPolicy : IBuildPlanPolicy
//	{

//		public LogBuildPlanPolicy(Type logType)
//		{
//			LogType = logType;
//		}

//		public Type LogType { get; private set; }

//		public void BuildUp(IBuilderContext context)
//		{
//			if (context.Existing == null)
//			{
//				ILogger log = LogManager.GetLogger(LogType.FullName);
//				context.Existing = log;
//			}
//		}
//	}
//}
