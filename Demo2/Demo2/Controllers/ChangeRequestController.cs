using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Demo2.Domain.Events;
using Microsoft.AspNetCore.Authorization;
using Demo2.Domain.Events.Repositories;
using Demo2.Domain;
using System.Diagnostics;
using Demo2.Domain.Events.MyImplementation.Database;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using Demo2.Domain.Eventsourcing.Projectors;
using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using System;

namespace Demo2.Controllers;

[ApiController]
[Route("api/change-request")]
public class ChangeRequestController: BaseController
{
    private readonly RelationalContext _rlContext;
    private readonly EventSourcingContext _esContext;
    private readonly ListOfValuesRepository _listOfValues;
    private readonly IChangeRequestDomainModelRepository _domainChnageRequestRepository;

    public ChangeRequestController(RelationalContext rlContext, EventSourcingContext esContext,
        ListOfValuesRepository listOfValues, IChangeRequestDomainModelRepository domainChnageRequestRepository)
    {
        _rlContext = rlContext;
        _esContext = esContext;
        _listOfValues = listOfValues;
        _domainChnageRequestRepository = domainChnageRequestRepository;

    }

    [HttpGet("seed/{amount}")]
    public async Task<ActionResult> Seed([FromServices] IEventStore repository, int amount)
    {
        try
        {
            var result = await ChangeRequestSeeder.Generate(_rlContext, repository, amount);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(ex.ToBetterString());
        }
    }

    [HttpGet("list/relational/all")]
    public ActionResult ListRelational()
    {
        var sw = Stopwatch.StartNew();

        var entities = _rlContext.Set<ChangeRequest>()
            .Include(x => x.Priority)
            .Include(x => x.Source)
            .Include(x => x.Type)
            .Include(x => x.Disciplines).ThenInclude(x => x.Discipline)
            .Include(x => x.Platform)

            .Include(x => x.Documents).ThenInclude(x=> x.Category)
            .Include(x => x.Fics).ThenInclude(x => x.Category)
            .Include(x => x.Attachments).ThenInclude(x => x.Type)
            .Include(x => x.EvidenceAttachments).ThenInclude(x => x.Type)

            .ToList();

        sw.Stop();

        var rate = sw.Elapsed.TotalMilliseconds/ entities.Count;

        return Ok($"Loaded {entities.Count} entities from relational store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    [HttpGet("list/event-sourcing/all")]
    public async Task<ActionResult> ListEventSourcing()
    {
        if (!_listOfValues.IsInitialized)
        {
            InitializeLoV();
        }

        var sw = Stopwatch.StartNew();

        var entities = await _domainChnageRequestRepository.GetAll();

        sw.Stop();

        var rate = sw.Elapsed.TotalMilliseconds / entities.Count();

        return Ok($"Loaded {entities.Count()} entities from event store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    [HttpGet("list/relational/one-by-one")]
    public ActionResult ListRelationalIndividual()
    {
        var ids = _rlContext.Set<ChangeRequest>().Select(x => x.Id).ToArray();

        var sw = Stopwatch.StartNew();

        foreach (var id in ids)
        {
            var entity = _rlContext.Set<ChangeRequest>()
                .Include(x => x.Priority)
                .Include(x => x.Source)
                .Include(x => x.Type)
                .Include(x => x.Disciplines)
                    .ThenInclude(x => x.Discipline)
                .Include(x => x.Platform)

                .Include(x => x.Documents)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Fics)
                    .ThenInclude(x => x.Category)
                .Include(x => x.Attachments)
                    .ThenInclude(x => x.Type)
                .Include(x => x.EvidenceAttachments)
                    .ThenInclude(x => x.Type)

                .First(x => x.Id == id);
        }

        sw.Stop();

        var rate = sw.Elapsed.TotalMilliseconds / ids.Length;

        return Ok($"Loaded {ids.Length} entities from relational store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    [HttpGet("list/event-sourcing/one-by-one")]
    public async Task<ActionResult> ListEventSourcingIndividual()
    {
        if (!_listOfValues.IsInitialized)
        {
            InitializeLoV();
        }

        var ids = _esContext.Events.Select(x => x.AggregateId).Distinct().ToArray();

        var sw = Stopwatch.StartNew();

        foreach (var id in ids)
        {
            var entity = await _domainChnageRequestRepository.FindAsync(id);
        }

        sw.Stop();

        var rate = sw.Elapsed.TotalMilliseconds / ids.Length;

        return Ok($"Loaded {ids.Length} entities from event store store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    private void InitializeLoV()
    {
        _listOfValues.ChangeRequestPriorities = _rlContext.Set<ChangeRequestPriority>().AsNoTracking().ToDictionary();
        _listOfValues.ChangeRequestSources = _rlContext.Set<ChangeRequestSource>().AsNoTracking().ToDictionary();
        _listOfValues.ChangeRequestTypes = _rlContext.Set<ChangeRequestType>().AsNoTracking().ToDictionary();
        _listOfValues.DocumentCategories = _rlContext.Set<DocumentCategory>().AsNoTracking().ToDictionary();
        _listOfValues.AttachmentTypes = _rlContext.Set<AttachmentType>().AsNoTracking().ToDictionary();
        _listOfValues.FicCategories = _rlContext.Set<FicCategory>().AsNoTracking().ToDictionary();
        _listOfValues.Disciplines = _rlContext.Set<Discipline>().AsNoTracking().ToDictionary();
        _listOfValues.Platforms = _rlContext.Set<Platform>().AsNoTracking().ToDictionary();
        _listOfValues.Uns = _rlContext.Set<Un>().AsNoTracking().ToDictionary();
    }
}

public static class Extensions
{
    public static Dictionary<Guid, T> ToDictionary<T>(this IEnumerable<T> items) where T : BaseEntity
    {
        var results = new Dictionary<Guid, T>();

        foreach (var item in items)
        {
            results.Add(item.Id, item);
        }

        return results;
    }
}
