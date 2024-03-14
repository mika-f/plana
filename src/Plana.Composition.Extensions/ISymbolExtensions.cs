// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Composition.Extensions;

// ReSharper disable once InconsistentNaming
public static class ISymbolExtensions
{
#pragma warning disable CS8619
    public static ISymbol? GetInterfaceSymbol(this ISymbol symbol)
    {
        if (symbol.Kind != SymbolKind.Method && symbol.Kind != SymbolKind.Property && symbol.Kind != SymbolKind.Event)
            return null;

        var containingType = symbol.ContainingType;
        var interfaces = containingType.AllInterfaces;
        var members = interfaces.SelectMany(w => w.GetMembers());
        var implementations = members.Select(w =>
                                     {
                                         var implementation = containingType.FindImplementationForInterfaceMember(w);
                                         if (implementation != null)
                                             return (Implementation: implementation, Interface: w);

                                         return (null, null);
                                     })
                                     .Where(w => w.Implementation != null);

        return implementations.FirstOrDefault(w => w.Implementation?.Equals(symbol, SymbolEqualityComparer.Default) == true).Interface;
    }
#pragma warning restore CS8619

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    public static bool IsInWorkspace(this ISymbol symbol, ISolution solution)
    {
        var sources = symbol.OriginalDefinition.Locations.Select(w => w.SourceTree?.FilePath).Where(w => w != null).Select(Path.GetFullPath);
        var targets = solution.Projects.SelectMany(w => w.Documents).Select(w => Path.GetFullPath(w.Path));

        return sources.All(w => targets.Contains(w));
    }
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).

    public static bool IsNotInWorkspace(this ISymbol symbol, ISolution solution)
    {
        return !IsInWorkspace(symbol, solution);
    }

    /*
    // Licensed to the .NET Foundation under one or more agreements.
    // The .NET Foundation licenses this file to you under the MIT license.
    // ref: https://sourceroslyn.io/#Microsoft.CodeAnalysis.Workspaces/J/s/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/Core/Extensions/ISymbolExtensions.cs/ISymbolExtensions.cs,88c7a12382fb60b6
    public static ImmutableArray<ISymbol> ExplicitOrImplicitInterfaceImplementations(this ISymbol symbol)
    {
        if (symbol.Kind != SymbolKind.Method && symbol.Kind != SymbolKind.Property && symbol.Kind != SymbolKind.Event)
            return [];

        var containingType = symbol.ContainingType;
        var interfaces = containingType.AllInterfaces;
        var members = interfaces.SelectMany(w => w.GetMembers());
        var implementations = members.Select(w =>
        {
            var implementation = containingType.FindImplementationForInterfaceMember(w);
            if (implementation != null)
                return implementation;

            return null;
        }).Where(w => w != null).ToList();

        if (implementations.Any())
            return implementations.Where(w => w.Equals(symbol)).ToImmutableArray();

        return [];
    }

    public static ImmutableArray<ISymbol> ExplicitInterfaceImplementations(this ISymbol symbol)
    {
        return symbol switch
        {
            IEventSymbol e => ImmutableArray<ISymbol>.CastUp(e.ExplicitInterfaceImplementations),
            IMethodSymbol m => ImmutableArray<ISymbol>.CastUp(m.ExplicitInterfaceImplementations),
            IPropertySymbol p => ImmutableArray<ISymbol>.CastUp(p.ExplicitInterfaceImplementations),
            _ => []
        };
    }

    public static ImmutableArray<ISymbol> ImplicitInterfaceImplementations(this ISymbol symbol)
    {
        return symbol.ExplicitOrImplicitInterfaceImplementations().Except(symbol.ExplicitInterfaceImplementations()).ToImmutableArray();
    }
    */
}