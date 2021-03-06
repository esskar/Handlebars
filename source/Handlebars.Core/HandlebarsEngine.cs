﻿using System;
using System.IO;
using Handlebars.Core.Compiler;
using Handlebars.Core.Internals;

namespace Handlebars.Core
{
    public class HandlebarsEngine : IHandlebarsEngine
    {
        private readonly HandlebarsCompiler _compiler;

        public HandlebarsEngine()
            : this(new HandlebarsConfiguration()) { }

        public HandlebarsEngine(HandlebarsConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            Configuration = configuration;
            RegisterBuiltinHelpers();

            _compiler = new HandlebarsCompiler(this);
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
            Configuration.HandlebarsTemplateRegistry.RegisterTemplate(templateName, template);
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
            if (helperFunction == null)
                throw new ArgumentNullException(nameof(helperFunction));

            RegisterHelper(helperName, (configuration, output, context, arguments) =>
            {
                helperFunction(output, context, arguments);
            });
        }

        public void RegisterHelper(string helperName, HandlebarsHelperV2 helperFunction)
        {
            if (string.IsNullOrEmpty(helperName))
                throw new ArgumentNullException(nameof(helperName));
            if (helperFunction == null)
                throw new ArgumentNullException(nameof(helperFunction));

            Configuration.Helpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            if (helperFunction == null)
                throw new ArgumentNullException(nameof(helperFunction));

            RegisterHelper(helperName, (configuration, output, options, context, arguments) =>
            {
                helperFunction(output, options, context, arguments);
            });
        }

        public void RegisterHelper(string helperName, HandlebarsBlockHelperV2 helperFunction)
        {
            if (string.IsNullOrEmpty(helperName))
                throw new ArgumentNullException(nameof(helperName));
            if (helperFunction == null)
                throw new ArgumentNullException(nameof(helperFunction));

            Configuration.BlockHelpers.AddOrUpdate(helperName, n => helperFunction, (n, h) => helperFunction);
        }

        public void RegisterHelper(IHandlebarsHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            RegisterHelper(helper.Name, helper.Execute);
        }

        public void RegisterHelper(IHandlebarsBlockHelper helper)
        {
            if (helper == null)
                throw new ArgumentNullException(nameof(helper));

            RegisterHelper(helper.Name, helper.Execute);
        }

        private void RegisterBuiltinHelpers()
        {
            foreach (var helper in BuiltinHelpers.Helpers)
            {
                RegisterHelper(helper);
            }
            foreach (var helper in BuiltinHelpers.BlockHelpers)
            {
                RegisterHelper(helper);
            }
        }
    }
}
