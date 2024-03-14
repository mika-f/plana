// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

[assembly: InternalsVisibleTo("Plana.Composition.RenameSymbols.Tests")]

namespace Plana.Composition.RenameSymbols;

[PlanaPlugin("rename-symbols")]
public class RenameSymbolsPlugin : IPlanaPlugin2
{
    private static readonly PlanaPluginOption Namespace = new("rename-namespaces", "Rename Namespaces", "rename namespaces, not supports file-scoped namespaces", false);
    private static readonly PlanaPluginOption ClassName = new("rename-classes", "Rename Classes", "rename classes", false);
    private static readonly PlanaPluginOption Properties = new("rename-properties", "Rename Properties", "rename properties", false);
    private static readonly PlanaPluginOption Fields = new("rename-fields", "Rename Fields", "rename fields", false);
    private static readonly PlanaPluginOption Methods = new("rename-methods", "Rename Methods", "rename methods", false);
    private static readonly PlanaPluginOption Variables = new("rename-variables", "Rename Variables", "rename local variables", false);
    private static readonly PlanaPluginOption KeepNameOnInspector = new("enum-attributes", "Keep Inspector Name of Enums", "add UnityEngine.InspectorName to enum members, without already specified", false);

    private readonly Dictionary<ISymbol, string> _dict = new(MeaningEqualitySymbolComparator.Default);

    internal bool IsEnableClassNameRenaming;
    internal bool IsEnableFieldsRenaming;
    internal bool IsEnableMethodsRenaming;
    internal bool IsEnableNamespaceRenaming;
    internal bool IsEnablePropertiesRenaming;
    internal bool IsEnableVariablesRenaming;
    internal bool KeepOriginalNameInInspector;

    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption> { Namespace, ClassName, Properties, Fields, Methods, Variables, KeepNameOnInspector }.AsReadOnly();

    public string Name => "Rename Symbols";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        IsEnableNamespaceRenaming = binder.GetValue(Namespace);
        IsEnableClassNameRenaming = binder.GetValue(ClassName);
        IsEnablePropertiesRenaming = binder.GetValue(Properties);
        IsEnableFieldsRenaming = binder.GetValue(Fields);
        IsEnableMethodsRenaming = binder.GetValue(Methods);
        IsEnableVariablesRenaming = binder.GetValue(Variables);
        KeepOriginalNameInInspector = binder.GetValue(KeepNameOnInspector);
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var walker = new CSharpSymbolsWalker(
                context.Solution,
                document,
                context.SecureRandom,
                IsEnableNamespaceRenaming,
                IsEnableClassNameRenaming,
                IsEnablePropertiesRenaming,
                IsEnableFieldsRenaming,
                IsEnableMethodsRenaming,
                IsEnableVariablesRenaming,
                _dict
            );

            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            walker.Visit(oldNode);
        }

        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpSymbolsRewriter(document, KeepOriginalNameInInspector, _dict);

            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);

            await document.ApplyChangesWithoutNotificationAsync(newNode, context.CancellationToken);
        }

        await context.Solution.ApplyChangesAsync(context.CancellationToken);
    }
}