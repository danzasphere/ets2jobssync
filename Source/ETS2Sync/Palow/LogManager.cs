using log4net;
using System;

namespace Palow.Library
{
    internal class LogManager
    {
        internal static ILog GetLogger(Type declaringType)
        {
            return new Logger();

        }
    }

    public class Logger : ILog
    {
        public void Debug(string v)
        {
            
        }

        public void Error(string v)
        {
            
        }

        public void Error(string v, Exception ex)
        {
            
        }
    }
}