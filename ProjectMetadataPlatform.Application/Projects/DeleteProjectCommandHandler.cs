using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProjectMetadataPlatform.Application.Interfaces;
using ProjectMetadataPlatform.Domain.Projects;

namespace ProjectMetadataPlatform.Application.Projects;

public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Project?>
{
    private readonly IProjectsRepository _projectsRepository;

    public DeleteProjectCommandHandler(IProjectsRepository projectsRepository)
    {
        _projectsRepository = projectsRepository;
    }

    public async Task<Project?> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _projectsRepository.GetProjectAsync(request.Id);
        if (project is not { IsArchived: true })
        {
            throw new ArgumentException("Project is not archived.");
        }

        return await _projectsRepository.DeleteProjectAsync(project);
    }
}
