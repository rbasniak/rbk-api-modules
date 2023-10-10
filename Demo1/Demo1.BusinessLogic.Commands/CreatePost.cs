using FluentValidation;
using MediatR;
using System.Text.Json.Serialization;
using Demo1.Database.Domain;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Commons.Core.Localization;
using AutoMapper;
using Demo1.DataTransfer;

namespace Demo1.BusinessLogic.Commands;

public class CreatePost
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, IHasReadingModel<Models.Read.Post>
    {
        [JsonIgnore]
        public OperationType Mode => OperationType.AddOrUpdate;

        public string Title { get; set; }
        public string Body { get; set; }
        public Guid BlogId { get; set; }
        public Guid? AuthorId { get; set; }
        public string UniqueInApplication { get; set; }
        public string UniqueInTenant { get; set; }
    }

    public class Validator: AbstractValidator<Request>, IDomainEntityValidator<Post>
    {
        private readonly DatabaseContext _context;
        private readonly ILocalizationService _localization;

        public Validator(DatabaseContext context, ILocalizationService localization)
        {
            _localization = localization;
            _context = context; 
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public Handler(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var blog = await _context.Blogs.FindAsync(request.BlogId);
            
            var author = request.AuthorId != null ? await _context.Authors.FindAsync(request.AuthorId) : null;

            var post = new Post(request.Identity.Tenant, blog, author, request.Title, request.Body, request.UniqueInTenant, request.UniqueInApplication);

            _context.Add(post);

            _context.SaveChanges();

            var temp1 = _mapper.Map<SimpleNamedEntity>(blog);
            var temp2 = _mapper.Map<SimpleNamedEntity>(author);
            // var temp3 = _mapper.Map<PostDetails>(post);

            return CommandResponse.Success();
        }
    }
}