using MediatR;
using Domain.Shared;
using Domain.Users;

namespace Application.Users;

public class Edit
{
    public class Command : IRequest<UserResultDTO.Me>
    {
        public UserCommandDTO User { get; set; }
        public string UserId { get; set; }

        public Command(UserCommandDTO user, string userId)
        {
            User = user;
            UserId = userId;
        }
    }

    public class Handler : IRequestHandler<Command, UserResultDTO.Me>
    {
        private readonly IUserRepository _userRepository;

        public Handler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResultDTO.Me> Handle(Command request, CancellationToken cancellationToken)
        {
            var inputItem = request.User;
            var user = await _userRepository.FindAsync(request.UserId);

            if (user == null)
                throw new NotFoundException();

            user.Edit(
                new UserName(inputItem.Username!),
                new UserEmail(inputItem.Email!)
            );

            var result = await _userRepository.UpdateAsync(user);

            return UserResultDTO.Me.CreateResultDTO(result);
        }
    }
}
