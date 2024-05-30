using System.Collections.Generic;

namespace ProjectMetadataPlatform.Api.Plugins.Models;

public record GetPluginListResponse(IEnumerable<GetPluginResponse> Plugins);