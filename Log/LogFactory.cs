using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Log
{
	public class NLogFactory : ILogFactory
	{
		public object GetLogger(Type parentType)
		{
			return LogManager.GetLogger(parentType.FullName);
		}
	}
}
