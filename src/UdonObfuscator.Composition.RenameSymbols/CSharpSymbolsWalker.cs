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
    private static readonly RandomNumberGenerator Rnd = RandomNumberGenerator.Create();

    public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        if (isRenameNamespaces)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitNamespaceDeclaration(node);
    }

    public override void VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        if (isRenameClasses)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitClassDeclaration(node);
    }

    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (isRenameClasses)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null)
                SetIdentifier(symbol);
        }

        base.VisitEnumDeclaration(node);
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        if (isRenameFields || isRenameVariables)
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

    private void SetIdentifier(ISymbol symbol)
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