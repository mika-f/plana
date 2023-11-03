// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Composition.Abstractions;
using UdonObfuscator.Composition.Abstractions.Attributes;

namespace UdonObfuscator.Composition.RenameSymbols;

[ObfuscatorAlgorithm("rename-symbols")]
public class SymbolObfuscator : IObfuscatorAlgorithm
{
    private static readonly ObfuscatorAlgorithmOption<bool> Namespace = new("--rename-namespaces", "rename namespaces", () => true);
    private static readonly ObfuscatorAlgorithmOption<bool> ClassName = new("--rename-classes", "rename classes", () => true);
    private static readonly ObfuscatorAlgorithmOption<bool> Properties = new("--rename-properties", "rename properties", () => true);
    private static readonly ObfuscatorAlgorithmOption<bool> Fields = new("--rename-fields", "rename fields", () => true);
    private static readonly ObfuscatorAlgorithmOption<bool> Methods = new("--rename-methods", "rename methods without referencing by SendCustomEvent", () => true);
    private static readonly ObfuscatorAlgorithmOption<bool> WithSendCustomEvent = new("--with-send-custom-event", "rename all methods", () => false);
    private static readonly ObfuscatorAlgorithmOption<bool> Variables = new("--rename-variables", "rename local variables", () => true);

    private bool _isEnableClassNameRenaming;
    private bool _isEnableFieldsRenaming;
    private bool _isEnableMethodsRenaming;
    private bool _isEnableNamespaceRenaming;
    private bool _isEnablePropertiesRenaming;
    private bool _isEnableVariablesRenaming;
    private bool _withSendCustomEvent;

    public IReadOnlyCollection<IObfuscatorAlgorithmOption> Options => new List<IObfuscatorAlgorithmOption> { Namespace, ClassName, Properties, Fields, Methods, WithSendCustomEvent, Variables }.AsReadOnly();

    public void BindParameters(IObfuscatorParameterBinder binder)
    {
        _isEnableNamespaceRenaming = binder.GetValue(Namespace);
        _isEnableClassNameRenaming = binder.GetValue(ClassName);
        _isEnablePropertiesRenaming = binder.GetValue(Properties);
        _isEnableFieldsRenaming = binder.GetValue(Fields);
        _isEnableMethodsRenaming = binder.GetValue(Methods);
        _withSendCustomEvent = binder.GetValue(WithSendCustomEvent);
        _isEnableVariablesRenaming = binder.GetValue(Variables);
    }
}