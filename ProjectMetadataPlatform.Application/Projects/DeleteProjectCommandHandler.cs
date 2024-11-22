using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

/// <summary>
/// Handles the DeleteProjectCommand.
/// </summary>
public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Project?>
{
    private readonly IProjectsRepository _projectsRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteProjectCommandHandler"/> class.
    /// </summary>
    /// <param name="projectsRepository">The repository of projects.</param>
    public DeleteProjectCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    /// <summary>
    /// Handles the DeleteProjectCommand.
    /// </summary>
    /// <param name="request">The DeleteProjectCommand.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the deleted project, or null if the project was not archived.</returns>
    /// <exception cref="ArgumentException">Thrown when the project is not archived.</exception>
    public async Task<Project?> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectAsync(request.Id);

        return project switch
        {
            null => throw new ArgumentException("Project not found."),
            { IsArchived: false } => throw new ArgumentException("Project is not archived."),
            _ => await _projectsRepository.DeleteProjectAsync(project)
        };
    }
}
