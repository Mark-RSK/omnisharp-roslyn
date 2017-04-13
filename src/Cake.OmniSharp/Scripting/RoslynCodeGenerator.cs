﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Cake.Core.Scripting;
using Cake.Core.Scripting.CodeGen;

namespace Cake.OmniSharp.Scripting
{
    internal static class RoslynCodeGenerator
    {
        public static string Generate(Script script)
        {
            var usingDirectives = string.Join("\r\n", script.UsingAliasDirectives);
            var aliases = GetAliasCode(script);
            var code = string.Join("\r\n", script.Lines);
            return string.Join("\r\n", usingDirectives, aliases, code);
        }

        public static string Generate(Script script, string code)
        {
            var usingDirectives = string.Join("\r\n", script.UsingAliasDirectives);
            var aliases = GetAliasCode(script);
            code = script.Lines[0] + "\r\n" + code;
            return string.Join("\r\n", usingDirectives, aliases, code);
        }

        private static string GetAliasCode(Script context)
        {
            var result = new List<string>();
            foreach (var alias in context.Aliases)
            {
                result.Add(alias.Type == ScriptAliasType.Method
                    ? MethodAliasGenerator.Generate(alias.Method)
                    : PropertyAliasGenerator.Generate(alias.Method));
            }
            return string.Join("\r\n", result);
        }
    }
}