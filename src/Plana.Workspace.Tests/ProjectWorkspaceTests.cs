using Plana.Testing;

namespace Plana.Workspace.Tests
{
    public class ProjectWorkspaceTests
    {
        [Fact]
        public async Task LoadCsprojWorkspace()
        {
            var container = new PlanaContainer();
            await container.RunAsync("../../../../Plana/Plana.csproj");

            var workspace = container.Workspace;

            Assert.NotNull(workspace);
            Assert.Equal(typeof(ProjectWorkspace), workspace.GetType());

            var projects = await workspace.GetProjectsAsync(new CancellationToken());
            Assert.Single(projects);

            var project = projects[0];
            Assert.Equal(5, project.Documents.Count);
        }

        [Fact]
        public async Task LoadSlnWorkspace()
        {
            var container = new PlanaContainer();
            await container.RunAsync();

            var csproj = Directory.GetFiles(Path.GetDirectoryName(container.Workspace.Path)!, "*.csproj", SearchOption.AllDirectories).Where(w => !w.Contains("Plana.Integrations")).ToList();
            var cs = Directory.GetFiles(Path.GetDirectoryName(container.Workspace.Path)!, "*.cs", SearchOption.AllDirectories).Where(w => !w.Contains("bin") && !w.Contains("obj") && !w.Contains("Plana.Integrations")).ToList();
            var workspace = container.Workspace;

            Assert.NotNull(workspace);
            Assert.Equal(typeof(SolutionWorkspace), workspace.GetType());

            var projects = await workspace.GetProjectsAsync(new CancellationToken());
            var documents = projects.SelectMany(w => w.Documents);

            Assert.Equal(csproj.Count, projects.Count);
            Assert.Equal(cs.Count, documents.Count());
        }
    }
}