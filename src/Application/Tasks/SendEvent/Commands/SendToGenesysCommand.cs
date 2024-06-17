using MediatR;
using Shared.Wrapper;

namespace Application.Tasks.SendEvent.Commands;

public sealed record SendEventCommand : IRequest<Result>;