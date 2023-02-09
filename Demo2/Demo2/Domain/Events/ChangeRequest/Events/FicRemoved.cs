﻿using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public partial class ChangeRequestEvents
{
    public class FicRemoved
    {
        public class V1 : DomainEvent
        {
            public V1(Guid changeRequestId, Guid ficId) : base(changeRequestId)
            {
                FicId = ficId;
            }

            public Guid FicId { get; set; }

            public override string Description => "Usuário removeu uma FIC";
        }
    }
}