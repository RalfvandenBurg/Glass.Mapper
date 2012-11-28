﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Mapper.Pipelines.ObjectConstruction
{
    public enum ObjectOrigin
    {
        CreateConcrete,
        CreateConcreteLazy,
        CreateInterface,
        ObjectCachingResolver
    }
}
