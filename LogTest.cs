using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Test
{
	[TestClass]
	public class LogTest
	{
		[TestMethod]
		public void InMethodInitializationTest()
		{
			var container = new UnityContainer();
			container
				.AddNewExtension<BuildTracking>()
				.AddNewExtension<LogCreation<ILogger, NLogFactory>>();

			var log = container.Resolve<ILogger>();

			log.Debug("test debug");

			Assert.AreEqual("Common.Test.LogTest", log.Name);
		}

		[TestMethod]
		public void InConstructorInitializationTest()
		{
			var container = new UnityContainer();
			container
				.AddNewExtension<BuildTracking>()
				.AddNewExtension<LogCreation<ILogger, NLogFactory>>();

			var lcpt = container.Resolve<LogContructorParameterTest>();
		}
	}

	public class LogContructorParameterTest
	{
		public LogContructorParameterTest(ILogger logger)
		{
			logger.Info("asdf");
			Assert.AreEqual(this.GetType().FullName, logger.Name);
		}
	}

}
