﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Mapper.Pipelines.ConfigurationResolver.Tasks.StandardResolver
{
    public class ConfigurationStandardResolverTask : IConfigurationResolverTask
    {
        public void Execute(ConfigurationResolverArgs args)
        {
            //TODO: ME - this needs to be made more efficent, maybe with a dictionary
            args.Result = args.Context[args.Type];
        }

        public int Order { get; set; }
    }
}
