﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Composition.RenameSymbols;

internal class CSharpSymbolsWalker(IDocument document, bool isRenameNamespaces, bool isRenameClasses, bool isRenameProperties, bool isRenameFields, bool isRenameMethods, bool isRenameMethodsWithEvents, bool isRenameVariables, Dictionary<ISymbol, string> dict) : CSharpSyntaxWalker
{
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

    private void SetIdentifier(ISymbol symbol)
    {
        if (dict.ContainsKey(symbol))
            return;


        var counter = dict.Count;
        var identifier = $"_0x{counter:x8}";

        dict.Add(symbol, identifier);
    }
}