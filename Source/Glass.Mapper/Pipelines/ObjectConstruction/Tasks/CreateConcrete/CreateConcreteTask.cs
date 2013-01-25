/*
   Copyright 2012 Michael Edwards
 
   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 
*/ 
//-CRE-

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Glass.Mapper.Caching;
using Glass.Mapper.Profilers;

namespace Glass.Mapper.Pipelines.ObjectConstruction.Tasks.CreateConcrete
{
    public class CreateConcreteTask : ObjectConstructionTask
    {
        private const string ConstructorErrorMessage = "No constructor for class {0} with parameters {1}";

        private static volatile  ProxyGenerator _generator;
        private static volatile  ProxyGenerationOptions _options;

        static CreateConcreteTask()
        {
            _generator = new ProxyGenerator();
            var hook = new LazyObjectProxyHook();
            _options = new ProxyGenerationOptions(hook);
        }

        public override void Execute(ObjectConstructionArgs args)
        {
            if (args.Result != null)
                return;

            var type = args.Configuration.Type;

            if(type.IsInterface)
            {
                return;
            }

           
            if(args.AbstractTypeCreationContext.IsLazy)
            {
                //here we create a lazy loaded version of the class
                args.Result = CreateLazyObject(args);
                args.ObjectOrigin = ObjectOrigin.CreateConcreteLazy;
            }
            else
            {
                //here we create a concrete version of the class
                args.Result = CreateObject(args);
                args.ObjectOrigin = ObjectOrigin.CreateConcrete;                
            }

            if (CacheDisabler.CacheDisabled || args.Context.ObjectCacheConfiguration == null)
            {
                args.AbortPipeline();
            }
        }

        protected virtual object CreateLazyObject(ObjectConstructionArgs args)
        {
            return  _generator.CreateClassProxy(args.Configuration.Type, new LazyObjectInterceptor(args));
        }

        protected virtual object CreateObject(ObjectConstructionArgs args)
        {

            var constructorParameters = args.AbstractTypeCreationContext.ConstructorParameters;

            var parameters =
                constructorParameters == null || !constructorParameters.Any()
                    ? Type.EmptyTypes
                    : constructorParameters.Select(x => x.GetType()).ToArray();

            var constructorInfo = args.Configuration.Type.GetConstructor(parameters);

            Delegate conMethod = args.Configuration.ConstructorMethods[constructorInfo];

            var obj = conMethod.DynamicInvoke(constructorParameters);

            args.Configuration.MapPropertiesToObject(obj, args.Service, args.AbstractTypeCreationContext);

            return obj;
        }
    }
}



