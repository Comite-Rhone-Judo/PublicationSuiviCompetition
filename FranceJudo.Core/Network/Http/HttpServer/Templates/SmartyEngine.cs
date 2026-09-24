using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FranceJudo.Core.Network.Http.HttpServer.Templates
{
    /// <summary>
    /// Simple template engine.
    /// This engine turns templates into C# code and compiles them at runtime
    /// to .NET assemblies. This makes the templates blazing fast. The templates
    /// are only recompiled if they have been changed on disk.
    /// </summary>
    public class SmartyEngine : ITemplateEngine
    {
        /// <summary>
        /// Compiles a Smarty-style template file into HTML output
        /// </summary>
        /// <param name="fileName">Path to the Smarty template to compile</param>
        /// <param name="variables">Key/value collection of template variable names and values</param>
        /// <returns>A compiled HTML document</returns>
        public string Render(string fileName, IDictionary<string, object> variables)
        {
            throw new NotImplementedException();
        }
    }
}
