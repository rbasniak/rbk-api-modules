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
    private readonly ListOfValuesRepository _listOfValues;

    public ChangeRequestController([FromServices] RelationalContext rlContext, ListOfValuesRepository listOfValues)
    {
        _rlContext = rlContext;
        _listOfValues = listOfValues;
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

    [HttpGet("list/relational")]
    public async Task<ActionResult> ListRelational()
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

        var rate = entities.Count / sw.Elapsed.TotalMilliseconds;

        return Ok($"Loaded {entities.Count} entities from relational store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    [HttpGet("list/event-sourcing")]
    public async Task<ActionResult> ListEventSourcing()
    {
        if (!_listOfValues.IsInitialized)
        {
            InitializeLoV();
        }

        var sw = Stopwatch.StartNew();

        var entities = new List<string>();

        sw.Stop();

        var rate = entities.Count / sw.Elapsed.TotalMilliseconds;

        return Ok($"Loaded {entities.Count} entities from relational store in {sw.Elapsed.TotalSeconds:0.00}s ({rate:0.0}ms per entity)");
    }

    private void InitializeLoV()
    {
        _listOfValues.ChangeRequestPriorities = _rlContext.Set<ChangeRequestPriority>().AsNoTracking().ToList();
        _listOfValues.ChangeRequestSources = _rlContext.Set<ChangeRequestSource>().AsNoTracking().ToList();
        _listOfValues.ChangeRequestTypes = _rlContext.Set<ChangeRequestType>().AsNoTracking().ToList();
        _listOfValues.DocumentCategories = _rlContext.Set<DocumentCategory>().AsNoTracking().ToList();
        _listOfValues.AttachmentTypes = _rlContext.Set<AttachmentType>().AsNoTracking().ToList();
        _listOfValues.FicCategories = _rlContext.Set<FicCategory>().AsNoTracking().ToList();
        _listOfValues.Disciplines = _rlContext.Set<Discipline>().AsNoTracking().ToList();
        _listOfValues.Platforms = _rlContext.Set<Platform>().AsNoTracking().ToList();
        _listOfValues.Uns = _rlContext.Set<Un>().AsNoTracking().ToList();
    }
}
