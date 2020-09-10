using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para renovar um token expirado
    /// </summary>
    public class RenewAccessToken
    {
        public class Command : IRequest<CommandResponse>
        {
            public string RefreshToken { get; set; }
        }

        /// <summary>
        /// Validador para o comando de renovação de um token expirado
        /// </summary>
        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.RefreshToken)
                    .IsRequired()
                    .MustAsync(RefreshTokenExistOnDatabase).WithMessage("Refresh token não existe no banco de dados")
                    .MustAsync(TokenMustBeWithinValidity).WithMessage("Refresh token fora da validade");
            }

            /// <summary>
            /// Validador que verifica se o token está dentro da validade
            /// </summary>
            public async Task<bool> TokenMustBeWithinValidity(string refreshToken, CancellationToken cancelation)
            {
                var user = await _context.Set<BaseUser>()
                    .SingleAsync(x => x.RefreshToken == refreshToken);

                return user.RefreshTokenValidity > DateTime.UtcNow;
            }

            /// <summary>
            /// Validador que verifica se o token existe no banco de dados
            /// </summary>
            public async Task<bool> RefreshTokenExistOnDatabase(string refreshToken, CancellationToken cancelation)
            {
                return await _context.Set<BaseUser>().AnyAsync(x => x.RefreshToken == refreshToken);
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IJwtFactory _jwtFactory;
            private readonly JwtIssuerOptions _jwtOptions;

            public Handler(IJwtFactory jwtFactory, DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
                _jwtFactory = jwtFactory;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {

                var user = await _context.Set<BaseUser>()
                    .Include(x => x.Roles)
                        .ThenInclude(x => x.Role)
                            .ThenInclude(x => x.Claims)
                                .ThenInclude(x => x.Claim)
                    .Include(x => x.Claims)
                        .ThenInclude(x => x.Claim)
                    .SingleOrDefaultAsync(x => x.RefreshToken == request.RefreshToken && x.RefreshTokenValidity > DateTime.UtcNow);

                var claims = new Dictionary<string, string[]>();
                claims.Add(JwtClaimIdentifiers.Role, user.GetAccessClaims().ToArray());

                var jwt = TokenGenerator.Generate(_jwtFactory, user.Username, claims, user.RefreshToken);

                return (null, jwt);
            }
        }
    }
}