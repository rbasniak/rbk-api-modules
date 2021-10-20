using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Notifications;
using rbkApiModules.Notifications.Models;
using rbkApiModules.Payment.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class Seed
    {
        public static void CreateEntities(DatabaseContext context)
        {
            var freePlan = new Plan("Free plan", 0, 0.0, true, true, "idPaypal", "idSandbox");
            context.Add(freePlan);

            var paidPlan = new Plan("Paid plan", 30, 0.0, true, true, "idPaypal", "idSandbox");
            context.Add(paidPlan);

            var client = new ClientUser("client", "client@clinet.com", "123123", true, new Client("John Doe", new DateTime(1999, 1, 1), freePlan));
            client.Confirm();
            context.Set<BaseUser>().Add(client);

            var manager = new ManagerUser("manager", "manager@manage.com", "123123", false, new Manager("Jane Doe", paidPlan));
            manager.Confirm();
            context.Set<BaseUser>().Add(manager);

            manager.Manager.SetTrialKey(new TrialKey(paidPlan, 30));

            var claims = new List<Claim>
            {
                new Claim("SAMPLE_CLAIM", "Exemplo de Controle de Acesso", "client"),
                new Claim("CAN_BUY", "Pode realizar compras", "client"),
                new Claim("SAMPLE_CLAIM", "Exemplo de Controle de Acesso", "manager"),
                new Claim("CAN_VIEW_REPORTS", "Pode visualizar relatórios", "manager")
            };

            foreach (var claim in claims)
            {
                if (!context.Set<Claim>().Any(x => x.Name == claim.Name && x.AuthenticationGroup == claim.AuthenticationGroup))
                {
                    context.Set<Claim>().Add(claim);
                }
            }

            context.SaveChanges();

            var notification1 = new Notification("Solicitações", "Aguardando aprovação", "Documento xyz está aguardando aprovação.", "route1", "", "manager", NotificationType.Success);
            context.Add(notification1);

            var notification2 = new Notification("Solicitações", "Aguardando revisão", "Documento xyz está aguardando revisão", "route1", "", "manager", NotificationType.Help);
            context.Add(notification2);

            var notification3 = new Notification("Alertas", "Falha crítica no sistema", "Verificamos que existe uma falha crítica no setor de cadastramento.", "", "", "manager", NotificationType.Danger);
            context.Add(notification3);

            var notification4 = new Notification("Sistema", "Nova versão disponível", "A versão 2.0 já se encontra disponível para download. Acesse o link para saber mais detalhes", "", "https://www.google.com.br", "manager", NotificationType.Info);
            context.Add(notification4);

            var notification5 = new Notification("Solicitações", "Pendencias | Documento xyz", "Existem pendências que precisam ser revistas no documento xyz", "route1", "", "manager", NotificationType.Warning);
            context.Add(notification5);

            var notification6 = new Notification("Solicitações", "Pendencias | Documento xyz", "Existem pendências que precisam ser revistas no documento xyz", "route1", "", "client", NotificationType.Warning);
            context.Add(notification6);

            context.SaveChanges();

            var libraGroup = new StateGroup("LIBRA");

            var inicial = new State(libraGroup, "Criada", "CREATED", "");
            var rascunho = new State(libraGroup, "Rascunho", "DRAFT", "");
            var aguardandoAprovacao = new State(libraGroup, "Aguardando Aprovação", "WAITING_APPROVAL", "");
            var emRevisao = new State(libraGroup, "Em  Revisão", "UNDER_REVISION", "");
            var aprovada = new State(libraGroup, "Aprovada", "APPROVED", "");
            var cancelada = new State(libraGroup, "Cancelada", "CANCELED", "");
            var finalizada = new State(libraGroup, "Finalizada", "FINALIZED", "");

            var enviarParaAprovacao = new Event("Enviar para Aprovação", "SEND_TO_APPROVAL", null);
            var aprovar = new Event("Aprovar", "APPROVE", null);
            var reprovar = new Event("Reprovar", "REPROVE", null);
            var cancelar = new Event("Cancelar", "CANCEL", null);
            var finalizar = new Event("Finalizar", "FINALIZE", null);
            var criar = new Event("Criar", "CREATE", null);

            var t1 = new Transition(inicial, criar, rascunho, "Criada no sistema", true);
            var t2 = new Transition(rascunho, enviarParaAprovacao, aguardandoAprovacao, "Enviada para aprovação inicial", true);
            var t3 = new Transition(aguardandoAprovacao, aprovar, aprovada, "Aprovada", true);
            var t4 = new Transition(aguardandoAprovacao, reprovar, emRevisao, "Enviada para revisão", true);
            var t5 = new Transition(emRevisao, enviarParaAprovacao, aguardandoAprovacao, "Enviada para aprovação de revisão", true);
            var t6 = new Transition(aprovada, finalizar, finalizada, "Finalizada", true);

            var t7 = new Transition(rascunho, cancelar, cancelada, "Cancelada", true);
            var t8 = new Transition(aguardandoAprovacao, cancelar, cancelada, "Cancelada", true);
            var t9 = new Transition(emRevisao, cancelar, cancelada, "Cancelada", true);
            var t10 = new Transition(aprovada, cancelar, cancelada, "Cancelada", true);

            context.Add(libraGroup);
            context.AddRange(new[] { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 });

            var blog1 = new Blog("Blog 1");
            var blog2 = new Blog("Blog 2");

            var editor1 = new Editor("Editor 1", blog1);
            var editor2 = new Editor("Editor 2", blog1);

            var post1 = new Post("Post 1", blog1);

            context.Add(editor1);
            context.Add(editor2);
            context.Add(post1);

            context.Add(blog1);
            context.Add(blog2);
        }
    }
}
