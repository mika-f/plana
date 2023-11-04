// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Composition.RenameSymbols;

internal class CSharpSymbolsWalker(IDocument document, bool isRenameNamespaces, bool isRenameClasses, bool isRenameProperties, bool isRenameFields, bool isRenameMethods, bool isRenameMethodsWithEvents, bool isRenameVariables, Dictionary<ISymbol, string> dict) : CSharpSyntaxWalker
{
    private const string AnnotateDisableNextSyntax = "/* udon-obfuscator:disable */";
    private const string AnnotateNetworkingNextSyntax = "/* udon-obfuscator:networking */";

    private static readonly List<string> Messages = new()
    {
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
    };

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        if (isRenameNamespaces && !HasAnnotationComment(node, AnnotateDisableNextSyntax))
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitNamespaceDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (isRenameClasses && !HasAnnotationComment(node, AnnotateDisableNextSyntax))
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitClassDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (isRenameClasses && !HasAnnotationComment(node, AnnotateDisableNextSyntax))
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitEnumDeclaration(node);
    }

    public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        if (isRenameProperties && !HasAnnotationComment(node, AnnotateDisableNextSyntax))
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitEnumMemberDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (isRenameMethods && !HasAnnotationComment(node, AnnotateDisableNextSyntax))
        {
            var isNetworking = HasAnnotationComment(node, AnnotateNetworkingNextSyntax);
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && !Messages.Contains(symbol.Name))
                SetIdentifier(symbol, isNetworking ? "M" : "_");
        }

        base.VisitMethodDeclaration(node);
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        var declaration = node.Parent?.Parent;
        var hasAnnotationComment = declaration != null && HasAnnotationComment((CSharpSyntaxNode)declaration, AnnotateDisableNextSyntax);
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

        var hex = "abcedf0123456789".ToCharArray();
        var identifier = $"{prefix}0x{RandomNumberGenerator.GetString(hex, 8)}";

        while (dict.ContainsValue(identifier))
            identifier = $"{prefix}0x{RandomNumberGenerator.GetString(hex, 0)}";

        dict.Add(symbol, identifier);
    }

    private bool HasAnnotationComment(CSharpSyntaxNode node, string comment)
    {
        if (node.HasLeadingTrivia)
        {
            var trivia = node.GetLeadingTrivia();
            return trivia.ToFullString().Trim() == comment;
        }

        return false;
    }
}