// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VerifySettings.cs" company="Eleon">
// Licensed under the MIT license.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Eleon.Tests.Common;

using System;
using System.Runtime.CompilerServices;

internal static class VerifySettingsBootstrap
{
    [ModuleInitializer]
    internal static void Init()
    {
        var type = Type.GetType("VerifyTests.VerifySourceGenerators, Verify.SourceGenerators");
        type?.GetMethod("Enable")?.Invoke(null, null);
    }
}