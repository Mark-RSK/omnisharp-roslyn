﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cake.Core;

namespace Cake.OmniSharp
{
    class CakeArguments : ICakeArguments
    {
        public bool HasArgument(string name)
        {
            return false;
        }

        public string GetArgument(string name)
        {
            return string.Empty;
        }
    }
}