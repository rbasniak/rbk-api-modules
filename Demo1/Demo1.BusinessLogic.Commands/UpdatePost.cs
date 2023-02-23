﻿using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Core.Localization;

namespace Demo1.BusinessLogic.Commands;

public class UpdatePost
{
    public class Command : CreatePost.Command, IHasReadingModel<Models.Read.Post>
    {
        public Guid Id { get; set; }
    }

    public class Validator: AbstractValidator<Command>
    {
        public Validator(DbContext context, ILocalizationService localization)
        {
            RuleFor(x => x.Id).MustExistInDatabase<Command, Post>(context, localization);
        }
    }

    public class Handler : IRequestHandler<Command, AuditableCommandResponse>
    {
        private readonly DatabaseContext _context;

        public Handler(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<AuditableCommandResponse> Handle(Command command, CancellationToken cancellation)
        {
            var post = await _context.Posts
                .Include(x => x.Blog)
                .Include(x => x.Author)
                .FirstAsync(x => x.Id == command.Id);

            post.Update(command.Title, command.Body);

            await _context.SaveChangesAsync();

            return AuditableCommandResponse.Success(post, post.Id, post.BlogId);
        }
    }
}