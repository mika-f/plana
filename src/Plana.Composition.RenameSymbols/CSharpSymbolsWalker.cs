// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;
using Plana.Composition.Extensions.Unity;

namespace Plana.Composition.RenameSymbols;

internal class CSharpSymbolsWalker(IDocument document, bool isRenameNamespaces, bool isRenameClasses, bool isRenameProperties, bool isRenameFields, bool isRenameMethods, bool isRenameMethodsWithEvents, bool isRenameVariables, Dictionary<ISymbol, string> dict) : CSharpSyntaxWalker
{
    private static AnnotationComment NetworkingAnnotation => new("networking");

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        if (isRenameNamespaces && !node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitNamespaceDeclaration(node);
    }

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
            if (!node.IsUnityMessagingMethod(document.SemanticModel))
            {
                var symbol = document.SemanticModel.GetDeclaredSymbol(node)!;
                SetIdentifier(symbol, isNetworking ? "M" : "_");
            }
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
                SetIdentifier(symbol);
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

        var hex = "abcedf0123456789".ToCharArray();
        var identifier = $"{prefix}0x{RandomNumberGenerator.GetString(hex, 8)}";

        while (dict.ContainsValue(identifier))
            identifier = $"{prefix}0x{RandomNumberGenerator.GetString(hex, 0)}";

        dict.Add(symbol, identifier);
    }
}