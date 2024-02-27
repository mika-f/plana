// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

using Plana.Composition.Abstractions.Analysis;
using Plana.Logging.Abstractions;

namespace Plana.Workspace;

internal class PlanaWorkspaceContext
{
    private readonly WorkspaceKind _kind;
    private readonly ILogger? _logger;
    private readonly Dictionary<DocumentId, CSharpSyntaxTree> _originals = new();
    private readonly Dictionary<DocumentId, (CSharpSyntaxNode Node, Func<Document, Task> Callback)> _changes = new();
    private readonly MSBuildWorkspace _workspace;

    private Project? _project;
    private Solution? _solution;

    public string FullName
    {
        get
        {
            return _kind switch
            {
                WorkspaceKind.Solution => _solution!.FilePath ?? "",
                WorkspaceKind.Project => _project!.FilePath ?? "",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public PlanaWorkspaceContext(MSBuildWorkspace workspace, Solution solution, ILogger? logger)
    {
        _workspace = workspace;
        _solution = solution;
        _logger = logger;
        _kind = WorkspaceKind.Solution;
    }

    public PlanaWorkspaceContext(MSBuildWorkspace workspace, Project project, ILogger? logger)
    {
        _workspace = workspace;
        _project = project;
        _logger = logger;
        _kind = WorkspaceKind.Project;
    }

    public async Task<IReadOnlyCollection<IProject>> GetProjectsAsync(CancellationToken ct)
    {
        var projects = new List<Project>();

        switch (_kind)
        {
            case WorkspaceKind.Solution:
                projects.AddRange(_solution!.Projects);
                break;

            case WorkspaceKind.Project:
                projects.Add(_project!);
                break;
        }

        var items = new List<IProject>();

        foreach (var project in projects)
        {
            var item = await CSharpProject.CreateProject(this, project, _logger, ct);
            items.Add(item);
        }

        return items.AsReadOnly();
    }

    public bool TryRegisterOriginalDocument(DocumentId id, CSharpSyntaxTree tree, out CSharpSyntaxTree original)
    {
        if (_originals.TryGetValue(id, out var o))
        {
            original = o;
            return true;
        }

        _originals.Add(id, tree);
        original = tree;

        return false;
    }

    public async Task<Document> WriteChangesToDocumentAsync(DocumentId id, SyntaxNode root, CancellationToken ct)
    {
        var source = new TaskCompletionSource<Document>();

        await WriteChangesToDocumentAsync(id, root, ct, document =>
        {
            source.SetResult(document);
            return Task.CompletedTask;
        });

        await FlushChangesAsync(ct);

        return await source.Task;
    }

    public Task WriteChangesToDocumentAsync(DocumentId id, SyntaxNode root, CancellationToken ct, Func<Document, Task> callback)
    {
        _changes.Add(id, ((CSharpSyntaxNode)root, callback));

        return Task.CompletedTask;
    }

    public async Task FlushChangesAsync(CancellationToken ct)
    {
        if (_changes.Count == 0)
            return;

        switch (_kind)
        {
            case WorkspaceKind.Solution:
            {
                foreach (var (id, (root, callback)) in _changes)
                {
                    ct.ThrowIfCancellationRequested();

                    _solution = _solution!.WithDocumentSyntaxRoot(id, root);

                    await callback.Invoke(_solution.GetDocument(id)!);
                }

                break;
            }

            case WorkspaceKind.Project:
            {
                foreach (var (id, (root, callback)) in _changes)
                {
                    ct.ThrowIfCancellationRequested();

                    var newSolution = _project!.Solution.WithDocumentSyntaxRoot(id, root);

                    _project = newSolution.GetProject(_project.Id)!;

                    await callback.Invoke(_project.GetDocument(id)!);
                }

                break;
            }

            default:
                throw new ArgumentOutOfRangeException();
        }

        _changes.Clear();
    }

    private enum WorkspaceKind
    {
        Solution,

        Project
    }
}