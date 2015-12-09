using System;

namespace log4net
{
    internal interface ILog
    {
        void Debug(string v);
        void Error(string v, Exception ex);
        void Error(string v);
    }
}