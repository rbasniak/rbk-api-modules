//using AspNetCoreApiTemplate.Utilities;
//using AspNetCoreApiTemplate.Utilities.Authentication;
//using FluentValidation;
//using MediatR;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using rbkApiModules.Infrastructure;
//using System;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace rbkApiModules.Authentication
//{
//    /// <summary>
//    /// Comando para criar usuários
//    /// </summary>
//    public class CreateUser
//    {
//        public class Command : IRequest<CommandResponse>, IPassword
//        {
//            public string Username { get; set; }
//            public string Password { get; set; }
//        }

//        public class Validator : AbstractValidator<Command>
//        {
//            private readonly DbContext _context;

//            public Validator(DbContext context)
//            {
//                _context = context;

//                CascadeMode = CascadeMode.Stop;

//                RuleFor(a => a.Username)
//                    .IsRequired()
//                    // FIXME: .MustHasLengthBetween(ModelConstants.Authentication.Username.MinLength, ModelConstants.Authentication.Username.MaxLength)
//                    .MustAsync(NotExistOnDatabase).WithMessage("Já existe um usuário cadastrado com este nome.")
//                    .WithName("Usuário");

//                RuleFor(a => a.Password)
//                    .IsRequired()
//                    // FIXME: .MustHasLengthBetween(ModelConstants.Authentication.Password.MinLength, ModelConstants.Authentication.Password.MaxLength)
//                    .WithName("Senha");
//            }

//            /// <summary>
//            /// Validador que verifica se o nome de usuário informado já existe no banco de dados
//            /// </summary>
//            public async Task<bool> NotExistOnDatabase(Command command, string username, CancellationToken cancelation)
//            {
//                var query = _context.Set<User>().Select(x => new { x.Username });

//                return !await query.AnyAsync(x => EF.Functions.Like(x.Username, username));
//            }
//        }

//        public class Handler : BaseCommandHandler<Command, DbContext>
//        {
//            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
//            {
//            }

//            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
//            {
//                var user = new User(request.Username, request.Password);

//                await _context.Set<User>().AddAsync(user);

//                await _context.SaveChangesAsync();

//                return (user.Id, user);
//            }
//        }
//    }
//}
