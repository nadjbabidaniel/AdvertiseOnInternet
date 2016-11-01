﻿using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace Common
{
    public class LogHandler
    {
        static LogHandler()
        {
            if (!EventLog.SourceExists("Repairmen"))
            {
                EventLog.CreateEventSource("Repairmen", "Application");
            }
        }
        //private readonly Logger logger = LogManager.GetCurrentClassLogger();

        public void LogError(Exception exception)
        {
            //logger.Log(LogLevel.Error, "Message: " + exception.Message + "\nStackTrace: " + exception.StackTrace);
        }

        public void LogError(string message)
        {
            //logger.Log(LogLevel.Error, message);
        }
    }
}