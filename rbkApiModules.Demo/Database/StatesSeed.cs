using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class StatesSeed
    {
        public static void Seed(DatabaseContext context)
        {
            var libraGroup = new StateGroup("LIBRA");

            var inicial = new State(libraGroup, "Criada", "CREATED", "");
            var rascunho = new State(libraGroup, "Rascunho", "DRAFT", "");
            var aguardandoAprovacao = new State(libraGroup, "Aguardando Aprovação", "WAITING_APPROVAL", "");
            var emRevisao = new State(libraGroup, "Em  Revisão", "UNDER_REVISION", "");
            var aprovada = new State(libraGroup, "Aprovada", "APPROVED", "");
            var cancelada = new State(libraGroup, "Cancelada", "CANCELED", "");
            var finalizada = new State(libraGroup, "Finalizada", "FINALIZED", "");

            var enviarParaAprovacao = new Event("Enviar para Aprovação", "SEND_TO_APPROVAL");
            var aprovar = new Event("Aprovar", "APPROVE");
            var reprovar = new Event("Reprovar", "REPROVE");
            var cancelar = new Event("Cancelar", "CANCEL");
            var finalizar = new Event("Finalizar", "FINALIZE");
            var criar = new Event("Criar", "CREATE");

            var t1 = new Transition(inicial, criar, rascunho, "Criada no sistema", true);
            var t2 = new Transition(rascunho, enviarParaAprovacao, aguardandoAprovacao, "Enviada para aprovação inicial", true);
            var t3 = new Transition(aguardandoAprovacao, aprovar, aprovada, "Aprovada", true);
            var t4 = new Transition(aguardandoAprovacao, reprovar, emRevisao, "Enviada para revisão", true);
            var t5 = new Transition(emRevisao, enviarParaAprovacao, aguardandoAprovacao, "Enviada para aprovação de revisão", true);
            var t6 = new Transition(aprovada, finalizar, finalizada, "Finalizada", true);

            var t7  = new Transition(rascunho, cancelar, cancelada, "Cancelada", true);
            var t8  = new Transition(aguardandoAprovacao, cancelar, cancelada, "Cancelada", true);
            var t9  = new Transition(emRevisao, cancelar, cancelada, "Cancelada", true);
            var t10 = new Transition(aprovada, cancelar, cancelada, "Cancelada", true);

            context.Add(libraGroup);
            context.AddRange(new[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 });
        }
    }
}
