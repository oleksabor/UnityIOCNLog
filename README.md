** sample how to use Unity dependency injection with logging framework (NLog)

As I've found in the codeplex conversation there is a possiblity to use Unity extensions for log DI and initialize logger with object type 
This allows to get logger name to be the same as class where it is used

So generic NLog sample looks like

	class LogSample
	{
		static Logger _log = LogManager.GetCurrentClassLogger();
	}

This creates static logger per class with name initialized to namespace.LogSample

However there is a possiblity to use Unity (and its lifetime management system) to resolve logger instances using container. 
There is codeplex conversation http://unity.codeplex.com/discussions/203744 that contains code sample to combine log4net and Unity. However the same approach can be used to inject NLog to the constructor parameters
New abstraction ILogFactory is added to the LogCreation<T,U>

Please note that source code listed here was written by marcoerni@codeplex and improved by SCDeveloper@codeplex

Using custom ILogFactory implementation it is possible to register any typename logging framework with Unity.

	public class NLogFactory : ILogFactory
	{
		public object GetLogger(Type parentType)
		{
			return LogManager.GetLogger(parentType.FullName);
		}
	}

and how it can be used
        {
			var container = new UnityContainer();
			container
				.AddNewExtension<BuildTracking>()
				.AddNewExtension<LogCreation<ILogger, NLogFactory>>();

			var log = container.Resolve<ILogger>();

			log.Debug("test debug");

			Assert.AreEqual("Common.Test.LogTest", log.Name);
	}

or constructor parameter DI

	public class LogContructorParameterTest
	{
		public LogContructorParameterTest(ILogger logger)
		{
			logger.Info("asdf");
			Assert.AreEqual(this.GetType().FullName, logger.Name);
		}
	}

	{
			var container = new UnityContainer();
			container
				.AddNewExtension<BuildTracking>()
				.AddNewExtension<LogCreation<ILogger, NLogFactory>>();

			var lcpt = container.Resolve<LogContructorParameterTest>();
        }

As you can see LogCreation source code was decoupled from logging framework 