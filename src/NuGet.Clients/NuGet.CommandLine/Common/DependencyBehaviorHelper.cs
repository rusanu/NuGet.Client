// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using NuGet.Commands;
using NuGet.Configuration;
using NuGet.Resolver;

namespace NuGet.CommandLine
{
    internal static class DependencyBehaviorHelper
    {
        private static DependencyBehavior TryGetDependencyBehavior(string behaviorStr)
        {
            DependencyBehavior dependencyBehavior;

            if (!Enum.TryParse<DependencyBehavior>(behaviorStr, ignoreCase: true, result: out dependencyBehavior) ||
                !Enum.IsDefined(typeof(DependencyBehavior), dependencyBehavior))
            {
                throw new CommandException(string.Format(CultureInfo.CurrentCulture,
                    LocalizedResourceManager.GetString("Error_UnknownDependencyVersion"), behaviorStr));
            }

            return dependencyBehavior;
        }

        public static DependencyBehavior GetDependencyBehavior(DependencyBehavior defaultBehavior, string dependencyVersion, Configuration.ISettings settings)
        {
            var ret = defaultBehavior;

            // Check to see if dependencyVersion parameter is set. Else check for dependencyVersion in .config.
            if (!string.IsNullOrEmpty(dependencyVersion))
            {
                ret = TryGetDependencyBehavior(dependencyVersion);
            }
            else
            {

                // If the dependencyVersion wasn't provided , try to get the dependencyBehavior from the .config.
                string settingsDependencyVersion =
                    SettingsUtility.GetConfigValue(settings, ConfigurationConstants.DependencyVersion);

                if (!string.IsNullOrEmpty(settingsDependencyVersion))
                {
                    ret = TryGetDependencyBehavior(settingsDependencyVersion);
                }
            }

#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                throw new Exception($"----------------------------------\n\tGetDependencyBehavior: {defaultBehavior} {dependencyVersion} => {ret}\n\n---------------------------------");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceInformation(ex.ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types

            return ret;
        }
    }
}
