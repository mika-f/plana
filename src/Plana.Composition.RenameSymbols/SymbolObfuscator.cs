﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions.Algorithm;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Attributes;

namespace Plana.Composition.RenameSymbols;

[ObfuscatorAlgorithm("rename-symbols")]
public class SymbolObfuscator : IObfuscatorAlgorithm
{
    private static readonly ObfuscatorAlgorithmOption<bool> Namespace = new("--rename-namespaces", "rename namespaces", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> ClassName = new("--rename-classes", "rename classes", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> Properties = new("--rename-properties", "rename properties", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> Fields = new("--rename-fields", "rename fields", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> Methods = new("--rename-methods", "rename methods without referencing by SendCustomEvent", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> WithSendCustomEvent = new("--with-send-custom-event", "rename all methods", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> Variables = new("--rename-variables", "rename local variables", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> KeepNameOnInspector = new("--enum-attributes", "add UnityEngine.InspectorName to enum members", () => false);

    private readonly Dictionary<ISymbol, string> _dict = [];

    private bool _isEnableClassNameRenaming;
    private bool _isEnableFieldsRenaming;
    private bool _isEnableMethodsRenaming;
    private bool _isEnableNamespaceRenaming;
    private bool _isEnablePropertiesRenaming;
    private bool _isEnableVariablesRenaming;
    private bool _withSendCustomEvent;
    private bool _keepNameOnInspector;

    public IReadOnlyCollection<IObfuscatorAlgorithmOption> Options => new List<IObfuscatorAlgorithmOption> { Namespace, ClassName, Properties, Fields, Methods, WithSendCustomEvent, Variables, KeepNameOnInspector }.AsReadOnly();

    public string Name => "Rename Symbols";

    public void BindParameters(IObfuscatorParameterBinder binder)
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

    public async Task ObfuscateAsync(List<IProject> projects, CancellationToken ct)
    {
        foreach (var document in projects.SelectMany(w => w.Documents))
        {
            var walker = new CSharpSymbolsWalker(
                document,
                _isEnableNamespaceRenaming,
                _isEnableClassNameRenaming,
                _isEnablePropertiesRenaming,
                _isEnableFieldsRenaming,
                _isEnableMethodsRenaming,
                _withSendCustomEvent,
                _isEnableVariablesRenaming,
                _dict
            );

            var oldNode = await document.SyntaxTree.GetRootAsync(ct);
            walker.Visit(oldNode);
        }

        foreach (var document in projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpSymbolsRewriter(document, _keepNameOnInspector, _dict);

            var oldNode = await document.SyntaxTree.GetRootAsync(ct);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
            var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

            await document.WriteSyntaxTreeAsync(newTree, ct);
        }
    }
}