﻿using System;
using System.IO;
using System.Linq;
using Handlebars.Core.Compiler.Translation.Expressions;

namespace Handlebars.Core.Logging
{
    internal class LogHelper : IHandlebarsHelper
    {
        private const LogLevel DefaultLogLevel = LogLevel.Info;

        public string Name => "log";

        public void Execute(HandlebarsConfiguration configuration, TextWriter output, dynamic context, params object[] arguments)
        {
            var logger = configuration.Logger;
            if (logger == null)
                return;

            var parameters = arguments.LastOrDefault() as HashParameterDictionary;
            var logLevel = GetLogLevel(parameters);

            if (configuration.LogLevel < logLevel)
                return;

            var count = parameters != null ? arguments.Length - 1 : arguments.Length;
            for (var i = 0; i < count; i++)
                logger.Log(arguments[i].ToString(), logLevel);
        }

        private static LogLevel GetLogLevel(HashParameterDictionary parameters)
        {
            return parameters?.GetConvertedValue("level",
                o => Enum.TryParse(o.ToString(), true, out LogLevel loglevel) ? loglevel : DefaultLogLevel,
                DefaultLogLevel) ?? DefaultLogLevel;
        }
    }
}
