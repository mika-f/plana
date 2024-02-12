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
    }
}