using MediatR;
using Domain.Todos;
using Domain.Shared;

namespace Application.Todos;

public class Delete
{
    public class Command : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }

        public Command(Guid id, string userId)
        {
            Id = id;
            UserId = userId;
        }
    }

    public class Handler : IRequestHandler<Command, Unit>
    {
        private readonly ITodoRepository _todoRepository;

        public Handler(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var todo = await _todoRepository.FindAsync(request.Id);

            if (todo == null)
                throw new NotFoundException();
            if (todo.OwnerUserId != request.UserId)
                throw new BadRequestException();

            await _todoRepository.RemoveAsync(request.Id);

            return Unit.Value;
        }
    }
}
