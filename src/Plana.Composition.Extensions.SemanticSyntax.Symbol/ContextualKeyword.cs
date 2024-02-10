// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions.SemanticSyntax.Symbol;

public abstract class ContextualKeyword(string keyword)
{
    public string Keyword => keyword;

    private static List<ContextualKeyword> Keywords =>
    [
        PartialKeyword,
        VarKeyword,
        DynamicKeyword,
        NameofKeyword,
        YieldKeyword,
        UnmanagedKeyword,
        GlobalKeyword,
        AddKeyword,
        RemoveKeyword,
        GetKeyword,
        SetKeyword,
        ValueKeyword,
        AsyncKeyword,
        AwaitKeyword,
        FromKeyword,
        SelectKeyword,
        WhereKeyword,
        GroupKeyword,
        IntoKeyword,
        OrderByKeyword,
        JoinKeyword,
        LetKeyword,
        ByKeyword,
        AscendingKeyword,
        DescendingKeyword,
        EqualsKeyword,
        AliasKeyword
    ];

    public abstract bool IsAllowInThisContext(CSharpSyntaxNode node);

    public static bool IsAllowIdentifierInThisContext(string identifier, CSharpSyntaxNode node)
    {
        var keyword = Keywords.FirstOrDefault(w => w.Keyword == identifier);
        return keyword == null || keyword.IsAllowInThisContext(node);
    }

    private class CombinedContextualKeyword : ContextualKeyword
    {
        private readonly ContextualKeyword[] _contextual;

        public CombinedContextualKeyword(params ContextualKeyword[] contextual) : base(contextual.FirstOrDefault()?.Keyword ?? "__internal__")
        {
            if (contextual.Length < 1)
                throw new ArgumentException("contextual must be greater than or equals to 1");

            var keyword = contextual[0].Keyword;
            if (contextual.Any(w => w.Keyword != keyword))
                throw new ArgumentException("all of ContextualKeyword must have same keyword");

            _contextual = contextual;
        }

        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return _contextual.All(w => w.IsAllowInThisContext(node));
        }
    }

    private class AsyncContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "async" => IsAllowAsyncKeywordInThisContext(node),
                "await" => IsAllowAwaitKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowAsyncKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // async keyword is always allowed as CSharpSyntaxNode
        }

        private static bool IsAllowAwaitKeywordInThisContext(CSharpSyntaxNode node)
        {
            var n = node.FirstAncestor<CSharpSyntaxNode>(w => w is MethodDeclarationSyntax or ParenthesizedLambdaExpressionSyntax or AnonymousMethodExpressionSyntax);
            var modifier = SyntaxFactory.Token(SyntaxKind.AsyncKeyword);

            return n switch
            {
                MethodDeclarationSyntax m => m.HasModifier(modifier),
                ParenthesizedLambdaExpressionSyntax l => l.HasModifier(modifier),
                AnonymousMethodExpressionSyntax a => a.HasModifier(modifier),
                _ => false
            };
        }
    }

    private class EventDeclarationContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "add" => IsAllowAddKeywordInThisContext(node),
                "remove" => IsAllowRemoveKeywordInThisContext(node),
                "value" => IsAllowValueKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowAddKeywordInThisContext(CSharpSyntaxNode n)
        {
            return true; // add keyword is always allowed as CSharpSyntaxNode
        }

        private static bool IsAllowRemoveKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // remove keyword is always allowed as CSharpSyntaxNode
        }

        private static bool IsAllowValueKeywordInThisContext(CSharpSyntaxNode node)
        {
            var accessor = node.FirstAncestorOrSelf<AccessorDeclarationSyntax>();
            if (accessor == null)
                return true;

            var isAddAccessor = accessor.Keyword.IsEquivalentTo(SyntaxFactory.Token(SyntaxKind.AddKeyword));
            var isRemoveAccessor = accessor.Keyword.IsEquivalentTo(SyntaxFactory.Token(SyntaxKind.RemoveKeyword));
            return !(isAddAccessor || isRemoveAccessor);
        }
    }

    private class PropertyDeclarationContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "get" => IsAllowGetKeywordInThisContext(node),
                "set" => IsAllowSetKeywordInThisContext(node),
                "value" => IsAllowValueKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowGetKeywordInThisContext(CSharpSyntaxNode n)
        {
            return true; // get keyword is always allowed as CSharpSyntaxNode
        }

        private static bool IsAllowSetKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // set keyword is always allowed as CSharpSyntaxNode
        }

        private static bool IsAllowValueKeywordInThisContext(CSharpSyntaxNode node)
        {
            var accessor = node.FirstAncestorOrSelf<AccessorDeclarationSyntax>();
            if (accessor == null)
                return true;

            return !accessor.Keyword.IsEquivalentTo(SyntaxFactory.Token(SyntaxKind.SetKeyword));
        }
    }

    private class LinqQueryContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "from" => IsAllowFromKeywordInThisContext(node),
                "select" => IsAllowSelectKeywordInThisContext(node),
                "where" => IsAllowWhereKeywordInThisContext(node),
                "group" => IsAllowGroupKeywordInThisContext(node),
                "into" => IsAllowIntoKeywordInThisContext(node),
                "orderby" => IsAllowOrderByKeywordInThisContext(node),
                "join" => IsAllowJoinKeywordInThisContext(node),
                "let" => IsAllowLetKeywordInThisContext(node),
                "ascending" => IsAllowAscendingKeywordInThisContext(node),
                "descending" => IsAllowDescendingKeywordInThisContext(node),
                "equals" => IsAllowEqualsKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsNodeInLinqQuerySyntax(CSharpSyntaxNode node)
        {
            var expression = node.FirstAncestor<QueryExpressionSyntax>();
            if (expression != null)
                return false;

            return true;
        }

        private static bool IsAllowFromKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowSelectKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowWhereKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowGroupKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowIntoKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowOrderByKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowJoinKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowLetKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowAscendingKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowDescendingKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }

        private static bool IsAllowEqualsKeywordInThisContext(CSharpSyntaxNode node)
        {
            return IsNodeInLinqQuerySyntax(node);
        }
    }

    private class PartialDeclarationContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "partial" => IsAllowPartialKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowPartialKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // alias keyword is always allowed as CSharpSyntaxNode
        }
    }

    private class LocalVariableDeclarationContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "var" => IsAllowDynamicKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowDynamicKeywordInThisContext(CSharpSyntaxNode node)
        {
            if (node is TypeSyntax)
                return false;

            return true;
        }
    }

    private class DynamicContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "dynamic" => IsAllowDynamicKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowDynamicKeywordInThisContext(CSharpSyntaxNode node)
        {
            if (node is TypeSyntax)
                return false;

            return true;
        }
    }

    private class NameofOperatorContext(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "nameof" => IsAllowAliasKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowAliasKeywordInThisContext(CSharpSyntaxNode _)
        {
            return false; // do not allow nameof in all context for safety
        }
    }

    private class EnumeratorContextKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "yield" => IsAllowYieldKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowYieldKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // yield keyword is always allowed as CSharpSyntaxNode
        }
    }

    private class GenericsDeclarationKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "unmanaged" => IsAllowUnmanagedKeywordInThisContext(node),
                "where" => IsAllowWhereKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowUnmanagedKeywordInThisContext(CSharpSyntaxNode node)
        {
            var clause = node.FirstAncestor<TypeParameterConstraintClauseSyntax>();

            if (clause != null && node.Parent is TypeConstraintSyntax)
                return false;

            return true;
        }

        private static bool IsAllowWhereKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // where keyword is maybe always allowed as CSharpSyntaxNode
        }
    }

    private class GlobalNamespaceDeclarationKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "global" => IsAllowGlobalKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowGlobalKeywordInThisContext(CSharpSyntaxNode node)
        {
            var qualified = node.FirstAncestor<AliasQualifiedNameSyntax>();
            if (qualified == null)
                return true;

            var alias = node.Parent;
            return alias != node;
        }
    }

    private class ExternDeclarationKeyword(string keyword) : ContextualKeyword(keyword)
    {
        public override bool IsAllowInThisContext(CSharpSyntaxNode node)
        {
            return Keyword switch
            {
                "alias" => IsAllowAliasKeywordInThisContext(node),
                _ => throw new ArgumentOutOfRangeException(nameof(keyword))
            };
        }

        private static bool IsAllowAliasKeywordInThisContext(CSharpSyntaxNode _)
        {
            return true; // alias keyword is always allowed as CSharpSyntaxNode
        }
    }


    #region keywords

    #region partial declaration

    public static ContextualKeyword PartialKeyword => new PartialDeclarationContextKeyword("partial");

    #endregion

    #region local variable declaratons

    public static ContextualKeyword VarKeyword => new LocalVariableDeclarationContextKeyword("var");

    #endregion

    #region dynamic type declaration

    public static ContextualKeyword DynamicKeyword => new DynamicContextKeyword("dynamic");

    #endregion

    #region nameof operator

    public static ContextualKeyword NameofKeyword => new NameofOperatorContext("nameof");

    #endregion

    #region enumerator

    public static ContextualKeyword YieldKeyword => new EnumeratorContextKeyword("yield");

    #endregion

    #region generics

    public static ContextualKeyword UnmanagedKeyword => new GenericsDeclarationKeyword("unmanaged");

    #endregion

    #region global namespace

    public static ContextualKeyword GlobalKeyword => new GlobalNamespaceDeclarationKeyword("global");

    #endregion

    #region event declaration

    public static ContextualKeyword AddKeyword => new EventDeclarationContextKeyword("add");

    public static ContextualKeyword RemoveKeyword => new EventDeclarationContextKeyword("remove");

    #endregion

    #region property declaration

    public static ContextualKeyword GetKeyword => new PropertyDeclarationContextKeyword("get");

    public static ContextualKeyword SetKeyword => new PropertyDeclarationContextKeyword("set");

    public static ContextualKeyword ValueKeyword => new CombinedContextualKeyword(new PropertyDeclarationContextKeyword("value"), new EventDeclarationContextKeyword("value"));

    #endregion

    #region async programming

    public static ContextualKeyword AsyncKeyword => new AsyncContextKeyword("async");

    public static ContextualKeyword AwaitKeyword => new AsyncContextKeyword("await");

    #endregion

    #region LINQ (query syntax)

    public static ContextualKeyword FromKeyword => new LinqQueryContextKeyword("from");

    public static ContextualKeyword SelectKeyword => new LinqQueryContextKeyword("select");

    public static ContextualKeyword WhereKeyword => new CombinedContextualKeyword(new LinqQueryContextKeyword("where"), new GenericsDeclarationKeyword("where"));

    public static ContextualKeyword GroupKeyword => new LinqQueryContextKeyword("group");

    public static ContextualKeyword IntoKeyword => new LinqQueryContextKeyword("into");

    public static ContextualKeyword OrderByKeyword => new LinqQueryContextKeyword("orderby");

    public static ContextualKeyword JoinKeyword => new LinqQueryContextKeyword("join");

    public static ContextualKeyword LetKeyword => new LinqQueryContextKeyword("let");

    public static ContextualKeyword ByKeyword => new LinqQueryContextKeyword("by");

    public static ContextualKeyword AscendingKeyword => new LinqQueryContextKeyword("ascending");

    public static ContextualKeyword DescendingKeyword => new LinqQueryContextKeyword("descending");

    public static ContextualKeyword EqualsKeyword => new LinqQueryContextKeyword("equals");

    #endregion

    #region extern declaration

    public static ContextualKeyword AliasKeyword => new ExternDeclarationKeyword("alias");

    #endregion

    #endregion
}