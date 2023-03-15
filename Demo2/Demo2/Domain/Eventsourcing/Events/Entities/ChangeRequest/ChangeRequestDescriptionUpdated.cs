﻿using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestDescriptionUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string description) : base(username, changeRequestId)
        {
            Description = description;
        }

        public string Description { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Description = Description;
        }
    }
}