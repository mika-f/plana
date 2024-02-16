// ------------------------------------------------------------------------------------------
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

internal class CSharpSymbolsWalker(IDocument document, IPlanaSecureRandom random, bool isRenameNamespaces, bool isRenameClasses, bool isRenameProperties, bool isRenameFields, bool isRenameMethods, bool isRenameMethodsWithEvents, bool isRenameVariables, Dictionary<ISymbol, string> dict)
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


    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (isRenameClasses && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitClassDeclaration(node);
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

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (isRenameMethods && !node.HasAnnotationComment())
        {
            var isNetworking = node.HasAnnotationComment(NetworkingAnnotation);
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && !Messages.Contains(symbol.Name))
                SetIdentifier(symbol, isNetworking ? "M" : "_");
        }

        base.VisitMethodDeclaration(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        var hasAnnotationComment = node.Parent?.Parent is CSharpSyntaxNode declaration && declaration.HasAnnotationComment();
        if (isRenameVariables && !hasAnnotationComment)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitParameter(node);
    }

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

    private void SetIdentifier(ISymbol symbol, string prefix = "_")
    {
        if (dict.ContainsKey(symbol))
            return;

        var identifier = $"{prefix}0x{random.GetGlobalUniqueAlphaNumericalString(8)}";
        dict.Add(symbol, identifier);
    }

    private string SetPropertyIdentifier(IPropertySymbol symbol)
    {
        if (dict.TryGetValue(symbol.OriginalDefinition, out var val))
            return val;

        var original = symbol.OriginalDefinition;
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