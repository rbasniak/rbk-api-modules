using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Utilities.Passwords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para login de usuário
    /// </summary>
    public class UserLogin
    {
        public class Command : IRequest<CommandResponse>, IPassword
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        /// <summary>
        /// Validador para o comando de login
        /// </summary>
        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Username)
                    .IsRequired()
                    // FIXME: .MustHasLengthBetween(ModelConstants.Authentication.Username.MinLength, ModelConstants.Authentication.Username.MaxLength)
                    .MustAsync(ExistOnDatabase).WithMessage("Credenciais inválidas.")
                    .WithName("Usuário")
                    .DependentRules(() =>
                    {
                        RuleFor(a => a.Username)
                            .MustAsync(BeConfirmed).WithMessage("Usuário não confirmado.");
                        RuleFor(a => a.Password)
                        .IsRequired()
                        // FIXME: .MustHasLengthBetween(ModelConstants.Authentication.Password.MinLength, ModelConstants.Authentication.Password.MaxLength)
                        .MustAsync(MatchPassword).WithMessage("Credenciais inválidas.")
                        .WithName("Senha");
                    });


            }

            /// <summary>
            /// Validador que verifica se o nome de usuário informado existe no banco de dados
            /// </summary>
            public async Task<bool> ExistOnDatabase(Command command, string username, CancellationToken cancelation)
            {
                var query = _context.Set<BaseUser>().Select(x => new { x.Email, x.Username, x.Password });

                return await query.AnyAsync(x => EF.Functions.Like(x.Username, username) || EF.Functions.Like(x.Email, username));
            }

            /// <summary>
            /// Validador que verifica se o usuário informado teve o e-mail confirmado
            /// </summary>
            public async Task<bool> BeConfirmed(Command command, string username, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => (EF.Functions.Like(x.Username, username) || EF.Functions.Like(x.Email, username)) && x.IsConfirmed);
            }


            /// <summary>
            /// Validador que verifica se a senha informada bate com o hash da senha no banco de dados
            /// </summary>
            public async Task<bool> MatchPassword(Command command, string password, CancellationToken cancelation)
            {
                var user = await _context.Set<BaseUser>().Select(x => new { x.Username, x.Password, x.Email })
                    .SingleAsync(x => EF.Functions.Like(x.Username, command.Username) || EF.Functions.Like(x.Email, command.Username));

                return PasswordHasher.VerifyPassword(password, user.Password);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IJwtFactory _jwtFactory;
            private readonly JwtIssuerOptions _jwtOptions;

            public Handler(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions,
                DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
                _jwtFactory = jwtFactory;
                _jwtOptions = jwtOptions.Value;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var claims = new Dictionary<string, string[]>();

                var user = await _context.Set<BaseUser>()
                    .Include(x => x.Roles)
                        .ThenInclude(x => x.Role)
                            .ThenInclude(x => x.Claims)
                                .ThenInclude(x => x.Claim)
                    .Include(x => x.Claims)
                        .ThenInclude(x => x.Claim)
                    .SingleAsync(x => EF.Functions.Like(x.Username, request.Username) || EF.Functions.Like(x.Email, request.Username));

                claims.Add(JwtClaimIdentifiers.Roles, user.GetAccessClaims().ToArray());

                claims.Add(JwtClaimIdentifiers.AuthenticationGroup, new string[] { user.AuthenticationGroup });

                claims.Add(JwtClaimIdentifiers.Avatar, new string[] { user.Avatar });

                claims.Add(JwtClaimIdentifiers.DisplayName, new string[] { String.IsNullOrEmpty(user.DisplayName) ? user.Username : user.DisplayName});

                var refreshToken = user.RefreshToken;

                if (user.RefreshTokenValidity < DateTime.UtcNow)
                {
                    refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

                    user.SetRefreshToken(refreshToken, _jwtOptions.RefreshTokenLife);
                }

                _context.SaveChanges();

                var jwt = TokenGenerator.Generate(_jwtFactory, user.Username, claims, refreshToken);

                return (null, jwt);
            }
        }
    }
}