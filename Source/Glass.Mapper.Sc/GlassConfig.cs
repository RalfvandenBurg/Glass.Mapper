﻿using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Glass.Mapper.Caching.CacheKeyResolving;
using Glass.Mapper.Caching.ObjectCaching;
using Glass.Mapper.CastleWindsor;
using Glass.Mapper.Configuration;
using Glass.Mapper.Pipelines.ConfigurationResolver;
using Glass.Mapper.Pipelines.ConfigurationResolver.Tasks.StandardResolver;
using Glass.Mapper.Pipelines.DataMapperResolver;
using Glass.Mapper.Pipelines.DataMapperResolver.Tasks;
using Glass.Mapper.Pipelines.ObjectConstruction;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.CreateConcrete;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.CreateInterface;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.ObjectCachingResolver;
using Glass.Mapper.Pipelines.ObjectConstruction.Tasks.ObjectCachingSaver;
using Glass.Mapper.Pipelines.ObjectSaving;
using Glass.Mapper.Pipelines.ObjectSaving.Tasks;
using Glass.Mapper.Pipelines.TypeResolver;
using Glass.Mapper.Pipelines.TypeResolver.Tasks.StandardResolver;
using Glass.Mapper.Sc.Caching;
using Glass.Mapper.Sc.Caching.CacheKeyResolving.Implementations;
using Glass.Mapper.Sc.DataMappers;
using Glass.Mapper.Caching.ObjectCaching.Implementations;
using Glass.Mapper.Sc.DataMappers.SitecoreQueryParameters;

namespace Glass.Mapper.Sc
{
    public class GlassConfig : GlassCastleConfigBase
    {
        public override void Configure(WindsorContainer container, string contextName)
        {

            //****** Data Mappers  ******//
            // Used to convert data to and from Sitecore
            container.Register(
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreChildrenMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldBooleanMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldDateTimeMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldDecimalMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldDoubleMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldEnumMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldFileMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldFloatMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldGuidMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldIEnumerableMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldImageMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldIntegerMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldLinkMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldLongMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNameValueCollectionMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableDateTimeMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableDoubleMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableDecimalMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableFloatMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableGuidMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldNullableIntMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldRulesMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldStreamMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldStringMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreFieldTypeMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreIdMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreInfoMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreItemMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreLinkedMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreParentMapper>().LifestyleTransient(),
                Component.For<AbstractDataMapper>().ImplementedBy<SitecoreQueryMapper>()
                    .DynamicParameters((k, d) =>
                                           {
                                               d["parameters"] = k.ResolveAll<ISitecoreQueryParameter>();
                                           })
                    .LifestyleTransient(),

            //****** SitecoreQueryMapper parameters ******//
            // Used by the SitecoreQueryMapper to replace placeholders in queries
                Component.For<ISitecoreQueryParameter>().ImplementedBy<ItemDateNowParameter>().LifestyleTransient(),
                Component.For<ISitecoreQueryParameter>().ImplementedBy<ItemEscapedPathParameter>().LifestyleTransient(),
                Component.For<ISitecoreQueryParameter>().ImplementedBy<ItemIdNoBracketsParameter>().LifestyleTransient(),
                Component.For<ISitecoreQueryParameter>().ImplementedBy<ItemIdParameter>().LifestyleTransient(),
                Component.For<ISitecoreQueryParameter>().ImplementedBy<ItemPathParameter>().LifestyleTransient(),

                //****** Data Mapper Resolver Tasks ******//
                // These tasks are run when Glass.Mapper tries to resolve which DataMapper should handle a given property, e.g. 
                // Tasks are called in the order they are specified below.
                // For more on component registration read: http://docs.castleproject.org/Windsor.Registering-components-one-by-one.ashx
                Component.For<IDataMapperResolverTask>()
                         .ImplementedBy<DataMapperStandardResolverTask>()
                         .LifestyleTransient(),

                //****** Type Resolver Tasks ******//
                // These tasks are run when Glass.Mapper tries to resolve the type a user has requested, e.g. 
                // if your code contained
                //       service.GetItem<MyClass>(id) 
                // the standard resolver will return MyClass as the type. You may want to specify your own tasks to custom type
                // inferring.
                // Tasks are called in the order they are specified below.
                // For more on component registration read: http://docs.castleproject.org/Windsor.Registering-components-one-by-one.ashx

                Component.For<ITypeResolverTask>().ImplementedBy<TypeStandardResolverTask>().LifestyleTransient(),

                //****** Configuration Resolver Tasks ******//
                // These tasks are run when Glass.Mapper tries to find the configration the user has requested based on the type passsed, e.g. 
                // if your code contained
                //       service.GetItem<MyClass>(id) 
                // the standard resolver will return the MyClass configuration. 
                // Tasks are called in the order they are specified below.
                // For more on component registration read: http://docs.castleproject.org/Windsor.Registering-components-one-by-one.ashx

                Component.For<IConfigurationResolverTask>()
                         .ImplementedBy<ConfigurationStandardResolverTask>()
                         .LifestyleTransient(),

                //****** Object Construction Tasks ******//
                // These tasks are run when an a class needs to be instantiated by Glass.Mapper.
                // Tasks are called in the order they are specified below.
                // For more on component registration read: http://docs.castleproject.org/Windsor.Registering-components-one-by-one.ashx

                Component.For<ObjectConstructionTask>()
                         .ImplementedBy<ObjectCachingResolverTask>()
                         .LifestyleTransient()
                         .OnCreate(task => task.Order = 0),

                Component.For<ObjectConstructionTask>()
                         .ImplementedBy<CreateConcreteTask>()
                         .LifestyleTransient()
                         .OnCreate(task => task.Order = 1),

                Component.For<ObjectConstructionTask>()
                         .ImplementedBy<CreateInterfaceTask>()
                         .LifestyleTransient()
                         .OnCreate(task => task.Order = 2),

                Component.For<ObjectConstructionTask>()
                         .ImplementedBy<ObjectCachingSaverTask>()
                         .LifestyleTransient()
                         .OnCreate(task => task.Order = 3),

                //****** Object Saving Tasks ******//
                // These tasks are run when an a class needs to be saved by Glass.Mapper.
                // Tasks are called in the order they are specified below.
                // For more on component registration read: http://docs.castleproject.org/Windsor.Registering-components-one-by-one.ashx

                Component.For<IObjectSavingTask>().ImplementedBy<StandardSavingTask>().LifestyleTransient(),


                Component.For<AbstractObjectCacheConfiguration>().ImplementedBy<ObjectCacheConfiguration>().LifestyleTransient(),
                Component.For<IAbstractObjectCache>().ImplementedBy<CacheTable<Guid>>().LifestyleTransient(),
                Component.For<AbstractCacheKeyResolver<Guid>>().ImplementedBy<SitecoreCacheKeyResolver>().LifestyleTransient()



                );
        }
    }
}
