// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

namespace Plana.Composition.RenameSymbols;

[PlanaPlugin("rename-symbols")]
public class RenameSymbolsPlugin : IPlanaPlugin2
{
    private static readonly PlanaPluginOption Namespace = new("rename-namespaces", "Rename Namespaces", "rename namespaces, not supports file-scoped namespaces", false);
    private static readonly PlanaPluginOption ClassName = new("rename-classes", "Rename Classes", "rename classes", false);
    private static readonly PlanaPluginOption Properties = new("rename-properties", "Rename Properties", "rename properties", false);
    private static readonly PlanaPluginOption Fields = new("rename-fields", "rename fields", false);
    private static readonly PlanaPluginOption Methods = new("rename-methods", "rename methods", false);
    private static readonly PlanaPluginOption WithSendCustomEvent = new("with-send-custom-event", "rename all methods", false);
    private static readonly PlanaPluginOption Variables = new("rename-variables", "rename local variables", false);
    private static readonly PlanaPluginOption KeepNameOnInspector = new("enum-attributes", "add UnityEngine.InspectorName to enum members, without already specified", false);

    private readonly Dictionary<ISymbol, string> _dict = new(MeaningEqualitySymbolComparator.Default);

    private bool _isEnableClassNameRenaming;
    private bool _isEnableFieldsRenaming;
    private bool _isEnableMethodsRenaming;
    private bool _isEnableNamespaceRenaming;
    private bool _isEnablePropertiesRenaming;
    private bool _isEnableVariablesRenaming;
    private bool _keepNameOnInspector;
    private bool _withSendCustomEvent;

    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption> { Namespace, ClassName, Properties, Fields, Methods, WithSendCustomEvent, Variables, KeepNameOnInspector }.AsReadOnly();

    public string Name => "Rename Symbols";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        _isEnableNamespaceRenaming = binder.GetValue(Namespace);
        _isEnableClassNameRenaming = binder.GetValue(ClassName);
        _isEnablePropertiesRenaming = binder.GetValue(Properties);
        _isEnableFieldsRenaming = binder.GetValue(Fields);
        _isEnableMethodsRenaming = binder.GetValue(Methods);
        _withSendCustomEvent = binder.GetValue(WithSendCustomEvent);
        _isEnableVariablesRenaming = binder.GetValue(Variables);
        _keepNameOnInspector = binder.GetValue(KeepNameOnInspector);
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var walker = new CSharpSymbolsWalker(
                document,
                context.SecureRandom,
                _isEnableNamespaceRenaming,
                _isEnableClassNameRenaming,
                _isEnablePropertiesRenaming,
                _isEnableFieldsRenaming,
                _isEnableMethodsRenaming,
                _withSendCustomEvent,
                _isEnableVariablesRenaming,
                _dict
            );

            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            walker.Visit(oldNode);
        }

        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpSymbolsRewriter(document, _keepNameOnInspector, _dict);

            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
            var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

            await document.WriteSyntaxTreeAsync(newTree, context.CancellationToken);
        }
    }
}