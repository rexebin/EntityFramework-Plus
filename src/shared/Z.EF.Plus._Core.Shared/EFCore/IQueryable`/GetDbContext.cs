﻿// Description: Entity Framework Bulk Operations & Utilities (EF Bulk SaveChanges, Insert, Update, Delete, Merge | LINQ Query Cache, Deferred, Filter, IncludeFilter, IncludeOptimize | Audit)
// Website & Documentation: https://github.com/zzzprojects/Entity-Framework-Plus
// Forum & Issues: https://github.com/zzzprojects/EntityFramework-Plus/issues
// License: https://github.com/zzzprojects/EntityFramework-Plus/blob/master/LICENSE
// More projects: http://www.zzzprojects.com/
// Copyright © ZZZ Projects Inc. 2014 - 2016. All rights reserved.

#if FULL || AUDIT || BATCH_DELETE || BATCH_UPDATE || QUERY_FUTURE
#if EFCORE
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Z.EntityFramework.Plus
{
    internal static partial class InternalExtensions
    {
        public static DbContext GetDbContext<T>(this IQueryable<T> source)
        {
            var compilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = (QueryCompiler) compilerField.GetValue(source.Provider);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
			var queryContextFactory = (RelationalQueryContextFactory)queryContextFactoryField.GetValue(compiler);

#if EFCORE_3X
            object stateManagerDynamic;

#if EFCORE_6X
            var dependenciesProperty = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            var dependencies = dependenciesProperty.GetValue(queryContextFactory);
#else
            var dependenciesField = typeof(RelationalQueryContextFactory).GetField("_dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            var dependencies = dependenciesField.GetValue(queryContextFactory);
#endif


            var stateManagerField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.QueryContextDependencies").GetProperty("StateManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            stateManagerDynamic = stateManagerField.GetValue(dependencies);

            IStateManager stateManager = stateManagerDynamic as IStateManager;

            if (stateManager == null)
            {
                stateManager = ((dynamic)stateManagerDynamic).Value;
            }
#else
#if EFCORE
			object stateManagerDynamic;

            var dependenciesProperty = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dependenciesProperty != null)
            {
                // EFCore 2.x
                var dependencies = dependenciesProperty.GetValue(queryContextFactory);

                var stateManagerField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.QueryContextDependencies").GetProperty("StateManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                stateManagerDynamic = stateManagerField.GetValue(dependencies);
            }
            else
            {
                // EFCore 1.x
                var stateManagerField = typeof(QueryContextFactory).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
                stateManagerDynamic = stateManagerField.GetValue(queryContextFactory);
            }

            IStateManager stateManager = stateManagerDynamic as IStateManager;

            if (stateManager == null)
            {
                stateManager = ((dynamic) stateManagerDynamic).Value;
            }
#else
            var stateManagerField = typeof (QueryContextFactory).GetField("_stateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (IStateManager) stateManagerField.GetValue(queryContextFactory);
#endif
#endif

            return stateManager.Context;
        }

        /// <summary>An IQueryable extension method that gets database context from the query.</summary>
        /// <param name="query">The query to act on.</param>
        /// <returns>The database context from the query.</returns>
        public static DbContext GetDbContext(this IQueryable query)
        {
            var compilerField = typeof (EntityQueryProvider).GetField("_queryCompiler", BindingFlags.NonPublic | BindingFlags.Instance);
            var compiler = (QueryCompiler) compilerField.GetValue(query.Provider);

            var queryContextFactoryField = compiler.GetType().GetField("_queryContextFactory", BindingFlags.NonPublic | BindingFlags.Instance);
            var queryContextFactory = (RelationalQueryContextFactory) queryContextFactoryField.GetValue(compiler);

#if EFCORE_3X
            object stateManagerDynamic;

#if EFCORE_6X
            var dependenciesProperty = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            var dependencies = dependenciesProperty.GetValue(queryContextFactory);
#else
            var dependenciesField = typeof(RelationalQueryContextFactory).GetField("_dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            var dependencies = dependenciesField.GetValue(queryContextFactory);
#endif

            var stateManagerField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.QueryContextDependencies").GetProperty("StateManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            stateManagerDynamic = stateManagerField.GetValue(dependencies);

            IStateManager stateManager = stateManagerDynamic as IStateManager;

            if (stateManager == null)
            {
                stateManager = ((dynamic)stateManagerDynamic).Value;
            }
#else
#if EFCORE
            object stateManagerDynamic;

            var dependenciesProperty = typeof(RelationalQueryContextFactory).GetProperty("Dependencies", BindingFlags.NonPublic | BindingFlags.Instance);
            if (dependenciesProperty != null)
            {
                // EFCore 2.x
                var dependencies = dependenciesProperty.GetValue(queryContextFactory);

                var stateManagerField = typeof(DbContext).GetTypeFromAssembly_Core("Microsoft.EntityFrameworkCore.Query.QueryContextDependencies").GetProperty("StateManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                stateManagerDynamic = stateManagerField.GetValue(dependencies);
            }
            else
            {
                // EFCore 1.x
                var stateManagerField = typeof(QueryContextFactory).GetProperty("StateManager", BindingFlags.NonPublic | BindingFlags.Instance);
                stateManagerDynamic = stateManagerField.GetValue(queryContextFactory);
            }

            IStateManager stateManager = stateManagerDynamic as IStateManager;

            if (stateManager == null)
            {
                stateManager = ((dynamic) stateManagerDynamic).Value;
            }
#else
            var stateManagerField = typeof (QueryContextFactory).GetField("_stateManager", BindingFlags.NonPublic | BindingFlags.Instance);
            var stateManager = (IStateManager) stateManagerField.GetValue(queryContextFactory);
#endif
#endif

            return stateManager.Context;
        }
    }
}

#endif
#endif