﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;

namespace Plana.Composition.RenameSymbols;

internal class CSharpSymbolsWalker(ISolution solution, IDocument document, IPlanaSecureRandom random, bool isRenameNamespaces, bool isRenameClasses, bool isRenameProperties, bool isRenameFields, bool isRenameMethods, bool isRenameVariables, Dictionary<ISymbol, string> dict)
    : CSharpSyntaxWalker
{
    private static readonly List<string> Messages =
    [
        "Awake",
        "FixedUpdate",
        "LateUpdate",
        "OnAnimatorIK",
        "OnAnimatorMove",
        "OnApplicationFocus",
        "OnApplicationPause",
        "OnApplicationQuit",
        "OnAudioFilterRead",
        "OnBecameInvisible",
        "OnBecameVisible",
        "OnCollisionEnter",
        "OnCollisionEnter2D",
        "OnCollisionExit",
        "OnCollisionExit2D",
        "OnCollisionStay",
        "OnCollisionStay2D",
        "OnConnectedToServer",
        "OnControllerColliderHit",
        "OnDestroy",
        "OnDisable",
        "OnDisconnectedFromServer",
        "OnDrawGizmos",
        "OnDrawGizmosSelected",
        "OnEnable",
        "OnFailedToConnect",
        "OnFailedToConnectToMasterServer",
        "OnGUI",
        "OnJoinBreak",
        "OnJoinBreak2D",
        "OnMasterServerEvent",
        "OnMouseDown",
        "OnMouseDrag",
        "OnMouseEnter",
        "OnMouseExit",
        "OnMouseOver",
        "OnMouseUp",
        "OnMouseUpAsButton",
        "OnNetworkInstantiate",
        "OnParticleCollision",
        "OnParticleSystemStopped",
        "OnParticleTrigger",
        "OnParticleUpdateJobScheduled",
        "OnPlayerConnected",
        "OnPlayerDisconnected",
        "OnPostRender",
        "OnPreCull",
        "OnPreRender",
        "OnRenderImage",
        "OnRenderObject",
        "OnSerializeNetworkView",
        "OnTransformChildrenChanged",
        "OnTransformParentChanged",
        "OnTriggerEnter",
        "OnTriggerEnter2D",
        "OnTriggerExit",
        "OnTriggerExit2D",
        "OnTriggerStay",
        "OnTriggerStay2D",
        "OnValidate",
        "OnWillRenderObject",
        "Reset",
        "Start",
        "Update"
    ];

    private static AnnotationComment NetworkingAnnotation => new("networking");

    private string SetIdentifier(ISymbol symbol, string prefix = "_", string suffix = "")
    {
        if (dict.TryGetValue(symbol, out var val))
            return val;

        var identifier = $"{prefix}0x{random.GetGlobalUniqueAlphaNumericalString(8)}{suffix}";
        dict.Add(symbol, identifier);

        return identifier;
    }

    public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        if (isRenameProperties && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitEnumMemberDeclaration(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        var hasAnnotationComment = node.Parent?.Parent is CSharpSyntaxNode declaration && declaration.HasAnnotationComment();
        if (isRenameVariables && !hasAnnotationComment)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
            {
                var identifier = SetIdentifier(symbol);

                // if IParameterSymbol is inside TypeDeclarationSyntax.ParameterList, it acts as IPropertySymbol or IFieldSymbol
                var unknown = node.Parent?.Parent;
                if (unknown is TypeDeclarationSyntax decl)
                {
                    var t = document.SemanticModel.GetDeclaredSymbol(decl);
                    var symbols = document.SemanticModel.LookupSymbols(decl.ParameterList!.FullSpan.End);
                    var act = symbols.FirstOrDefault(w => w.ContainingType?.Equals(t, SymbolEqualityComparer.Default) == true && w.Name == symbol.Name);

                    if (act != null)
                        dict.TryAdd(act, identifier);
                }
            }
        }

        base.VisitParameter(node);
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        var hasAnnotationComment = node.Parent?.Parent is CSharpSyntaxNode declaration && declaration.HasAnnotationComment();
        if ((isRenameFields || isRenameVariables) && !hasAnnotationComment)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                switch (symbol)
                {
                    case IFieldSymbol when isRenameFields:
                    case ILocalSymbol when isRenameVariables:
                        SetIdentifier(symbol);
                        break;
                }
        }

        base.VisitVariableDeclarator(node);
    }

    public override void VisitForEachStatement(ForEachStatementSyntax node)
    {
        if (isRenameVariables && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitForEachStatement(node);
    }

    private string KeepOriginalName(ISymbol symbol, string? name = null)
    {
        dict.Add(symbol, name ?? symbol.Name);
        return name ?? symbol.Name;
    }

    #region classes

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (isRenameClasses && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
            {
                bool IsAttribute(INamedTypeSymbol t)
                {
                    if (t.BaseType == null)
                        return false;

                    if (t.BaseType.Equals(typeof(Attribute).ToSymbol(document.SemanticModel), SymbolEqualityComparer.Default))
                        return true;

                    return false;
                }

                if (IsAttribute(symbol))
                {
                    var identifier = SetIdentifier(symbol, suffix: "Attribute");
                    var constructorName = identifier.Substring(0, identifier.Length - "Attribute".Length);

                    var constructors = symbol.Constructors;
                    foreach (var constructor in constructors)
                        dict.TryAdd(constructor, constructorName);
                }
                else
                {
                    SetIdentifier(symbol);
                }
            }
        }

        base.VisitClassDeclaration(node);
    }

    public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        if (isRenameClasses && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitInterfaceDeclaration(node);
    }

    public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        if (isRenameClasses && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitRecordDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (isRenameClasses && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitEnumDeclaration(node);
    }

    #endregion

    #region methods

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (isRenameMethods && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetMethodIdentifier(symbol);
        }

        base.VisitMethodDeclaration(node);
    }

    private string SetMethodIdentifier(IMethodSymbol symbol)
    {
        if (dict.TryGetValue(symbol.OriginalDefinition, out var val))
            return val;

        var original = symbol.OriginalDefinition;

        // check methods declared in source
        if (original.OverriddenMethod is not null)
        {
            var overridden = original.OverriddenMethod;
            var isExternalDefinition = overridden.Locations.Any(w => w.IsInMetadata);
            if (isExternalDefinition)
                return KeepOriginalName(symbol, overridden.Name);

            if (overridden.IsAnyDeclarationIsNotInWorkspace(solution))
                return KeepOriginalName(symbol, overridden.Name);
        }

        if (original.Locations.Any(w => w.IsInMetadata))
            return KeepOriginalName(symbol);

        if (original.IsAnyDeclarationIsNotInWorkspace(solution))
            return KeepOriginalName(symbol);

        var @interface = symbol.GetInterfaceSymbol();
        if (@interface is IMethodSymbol s)
        {
            var infer = dict.GetValueOrDefault(s) ?? dict.GetValueOrDefault(s.OriginalDefinition);
            if (string.IsNullOrWhiteSpace(infer))
                infer = SetMethodIdentifier(s);

            dict.Add(symbol, infer);

            return infer;
        }

        if (original.Equals(symbol, SymbolEqualityComparer.Default))
        {
            var identifier = $"_0x{random.GetGlobalUniqueAlphaNumericalString(8)}";
            dict.Add(original, identifier);

            return identifier;
        }

        // not match signature
        if (!MeaningEqualitySymbolComparator.Default.Equals(symbol, original))
        {
            var identifier = SetMethodIdentifier(original);
            dict.Add(symbol, identifier);

            return identifier;
        }

        return string.Empty;
    }

    #endregion

    #region properties

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (isRenameProperties && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetPropertyIdentifier(symbol);
        }

        base.VisitPropertyDeclaration(node);
    }

    private string SetPropertyIdentifier(IPropertySymbol symbol)
    {
        if (dict.TryGetValue(symbol.OriginalDefinition, out var val))
            return val;

        var original = symbol.OriginalDefinition;

        if (original.IsAnyDeclarationIsNotInWorkspace(solution))
            return KeepOriginalName(original);

        var @interface = symbol.GetInterfaceSymbol();
        if (@interface is IPropertySymbol s)
        {
            var infer = dict.GetValueOrDefault(s) ?? dict.GetValueOrDefault(s.OriginalDefinition);
            if (string.IsNullOrWhiteSpace(infer))
                infer = SetPropertyIdentifier(s);

            dict.Add(symbol, infer);

            return infer;
        }

        if (original.Equals(symbol, SymbolEqualityComparer.Default))
        {
            var identifier = $"_0x{random.GetGlobalUniqueAlphaNumericalString(8)}";
            dict.Add(original, identifier);

            return identifier;
        }

        throw new InvalidOperationException();
    }

    #endregion

    #region namespace

    public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
    {
        if (isRenameNamespaces && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetNamespaceIdentifier(symbol);
        }

        base.VisitFileScopedNamespaceDeclaration(node);
    }

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        if (isRenameNamespaces && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetNamespaceIdentifier(symbol);
        }

        base.VisitNamespaceDeclaration(node);
    }

    private void SetNamespaceIdentifier(INamespaceSymbol symbol)
    {
        if (dict.ContainsKey(symbol))
            return;

        if (symbol.IsAnyDeclarationIsNotInWorkspace(solution))
            return;

        if (symbol.ContainingNamespace.IsGlobalNamespace)
        {
            // root namespace
            var original = symbol.OriginalDefinition;
            if (original.Equals(symbol, SymbolEqualityComparer.Default))
            {
                var identifier = $"_0x{random.GetGlobalUniqueAlphaNumericalString(8)}";
                dict.Add(original, identifier);

                return;
            }
        }
        else
        {
            var stack = new Stack<INamespaceSymbol>();
            var current = symbol;

            while (true)
            {
                if (current.IsGlobalNamespace)
                    break;

                stack.Push(current);
                current = current.ContainingNamespace;
            }


            while (stack.Count > 0)
            {
                var s = stack.Pop();
                if (dict.ContainsKey(s.OriginalDefinition))
                    continue;

                var identifier = $"_0x{random.GetGlobalUniqueAlphaNumericalString(8)}";
                dict.Add(s.OriginalDefinition, identifier);
            }

            return;
        }

        throw new InvalidOperationException();
    }

    #endregion
}