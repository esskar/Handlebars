﻿using System;
using System.IO;
using Handlebars.Core.Compiler;

namespace Handlebars.Core
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public class HandlebarsEngine : IHandlebarsEngine
    {
        private readonly HandlebarsCompiler _compiler;

        public HandlebarsEngine()
            : this(new HandlebarsConfiguration()) { }

        public HandlebarsEngine(HandlebarsConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _compiler = new HandlebarsCompiler(configuration);

            Configuration = configuration;
            RegisterBuiltinHelpers();
        }

        public HandlebarsConfiguration Configuration { get; }

        public HandlebarsTemplate CompileView(string templateName, string parentTemplateName = null, bool throwOnErrors = true)
        {
            return _compiler.CompileView(templateName, parentTemplateName, throwOnErrors);
        }

        public HandlebarsTemplate Compile(TextReader template)
        {
            return _compiler.Compile(template);
        }

        public HandlebarsTemplate Compile(string template)
        {
            using (var reader = new StringReader(template))
            {
                return Compile(reader);
            }
        }

        public void RegisterTemplate(string templateName, HandlebarsTemplate template)
        {
            Configuration.TemplateRegistration.RegisterTemplate(templateName, template);
        }

        public void RegisterTemplate(string templateName, string template)
        {
            using (var reader = new StringReader(template))
            {
                RegisterTemplate(templateName, Compile(reader));
            }
        }

        public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Configuration.Helpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Configuration.BlockHelpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        private void RegisterBuiltinHelpers()
        {
            foreach (var helperDefinition in BuiltinHelpers.Helpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
            foreach (var helperDefinition in BuiltinHelpers.BlockHelpers)
            {
                RegisterHelper(helperDefinition.Key, helperDefinition.Value);
            }
        }
    }
}