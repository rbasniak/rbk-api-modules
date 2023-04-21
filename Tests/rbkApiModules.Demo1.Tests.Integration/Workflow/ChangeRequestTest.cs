using rbkApiModules.Demo1.Tests.Integration.Workflow;
using Stateless;
using System.Diagnostics;

public class ApplicationClaims
{
    public const string WORKFLOW_ADMIN = "WORKFLOW_ADMIN";
    public const string TECHNICIAN = "TECHNICIAN";
}

public class ChangeRequest
{
    public ChangeRequestWorkflow.State State { get; set; }
    public string Id { get; set; }
    public string CurrentOwner { get; set; }
    public string InternalNumber { get; set; }
}


public class ChangeRequestWorkflow
{
    public readonly StateMachine<State, Trigger> _machine;

    public enum Trigger
    {
        APROVAR_AVALIACAO_FINAL = 1,
        APROVAR_PARA_EXECUCAO = 2,
        APROVAR_PELO_ADMINISTRADOR = 3,
        APROVAR_PELO_SOLICITANTE = 4,
        CANCELAR = 5,
        CONCLUIR_CADASTRO_SINDOTEC = 6,
        CONCLUIR_EXECUCAO = 7,
        CONCLUIR_REVISAO = 8,
        CRIAR_ATRAVES_DO_ADMINISTRADOR = 9,
        CRIAR_ATRAVES_DO_REQUISITANTE = 10,
        DESIGNAR_TECNICO_DE_DOCUMENTACAO = 11,
        ENVIAR_PARA_AVALIACAO_DA_EXECUCAO = 12,
        ENVIAR_PARA_AVALIACAO_INICIAL = 13,
        INICIAR_ANALISE_DA_EXECUCAO_PELO_ADMINISTRADOR = 14,
        INICIAR_ANALISE_DA_EXECUCAO_PELO_SOLICITANTE = 15,
        INICIAR_ANALISE_PARA_EXECUCAO_PELO_TECNICO = 16,
        INICIAR_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR = 17,
        INICIAR_AVALIACAO_DA_EXECUCAO_PELO_TECNICO = 18,
        INICIAR_AVALIACAO_INICIAL = 19,
        INICIAR_EXECUCAO = 20,
        INICIAR_REVISAO = 21,
        REPROVAR_EXECUCAO_PELO_ADMINISTRADOR = 22,
        REPROVAR_EXECUCAO_PELO_SOLICITANTE = 23,
        SOLICITAR_INFORMACOES_ADICIONAIS_PARA_ANALISE_INICIAL = 24,
        SOLICITAR_INFORMACOES_ADICIONAIS_PARA_EXECUCAO = 25,
        SOLICITAR_INFORMACOES_ADICIONAIS_PARA_REVISAO = 26,
    }

    public enum State
    {
        AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_SOLICITANTE = 1,
        AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR = 2,
        AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_ADMINISTRADOR = 3,
        AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_TECNICO_EXECUTANTE = 4,
        AGUARDANDO_CADASTRO_NO_SINDOTEC = 5,
        AGUARDANDO_EXECUCAO = 6,
        AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_INICIAL = 7,
        AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_DO_TECNICO = 8,
        AGUARDANDO_REVISAO = 9,
        CANCELADA = 10,
        CONCLUIDA = 11,
        CRIADA_NO_SISTEMA_PELO_ADMINISTRADOR = 12,
        CRIADA_NO_SISTEMA_PELO_REQUISITANTE = 13,
        EXECUCAO_EM_ANALISE_PELO_SOLICITANTE = 14,
        EXECUCAO_EM_ANALISE_PELO_ADMINISTRADOR = 15,
        EM_AVALIACAO_PELO_ADMINISTRADOR = 16,
        EM_AVALIACAO_PELO_TECNICO_EXECUTANTE = 17,
        EM_EXECUCAO = 18,
        EM_REVISAO = 19,
        RASCUNHO = 20,
    }

    public ChangeRequestWorkflow(FakeNotificationService notificationService)
    {
        _machine = new StateMachine<State, Trigger>(State.RASCUNHO);

        _machine.Configure(State.RASCUNHO)
            // .Permit(Trigger.ENVIAR_PARA_AVALIACAO, State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR)
            //.PermitIf<ChangeRequest, DateTime>(Trigger.ENVIAR_PARA_AVALIACAO, State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR, (changeRequest, date) =>
            //{
            //    Debug.WriteLine($"Validating {changeRequest.Id} at {date.ToShortDateString()}");

            //    return changeRequest.Id == "019";
            //})
            .PermitIf(Trigger.APROVAR_PARA_EXECUCAO, State.AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_ADMINISTRADOR, new GuardDefinition(String.Empty, (args) =>
            {
                var changeRequest = (ChangeRequest)args[0];
                var date = (DateTime)args[1];

                Debug.WriteLine($"Validating {changeRequest.Id} at {date}");

                return changeRequest.Id == "019";
            }))
            // .OnDeactivate(() => Debug.WriteLine($"State {State.RASCUNHO} deactivated"))
            .OnExit(transition => Debug.WriteLine($"Exited from {transition.Source} and entered {transition.Destination} by firing {transition.Trigger}"));

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_ADMINISTRADOR)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .OnEntry(transition => Debug.WriteLine($"Entered {transition.Destination} from {transition.Source} by firing {transition.Trigger}"))
            // .OnEntryFrom(Trigger.ENVIAR_PARA_AVALIACAO, (transition) => Debug.WriteLine($"Entered {transition.Destination} from {transition.Source} by firing ONLY {transition.Trigger}"))
            // .OnEntryFrom(_machine.SetTriggerParameters<ChangeRequest>(Trigger.ENVIAR_PARA_AVALIACAO), OnEnteredThisState)

            .Permit(Trigger.SOLICITAR_INFORMACOES_ADICIONAIS_PARA_ANALISE_INICIAL, State.AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_INICIAL)
            .Permit(Trigger.APROVAR_PELO_ADMINISTRADOR, State.AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_TECNICO_EXECUTANTE);

        _machine.Configure(State.AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_INICIAL)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.ENVIAR_PARA_AVALIACAO_INICIAL, State.EM_AVALIACAO_PELO_ADMINISTRADOR);

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DO_PEDIDO_PELO_TECNICO_EXECUTANTE)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_ANALISE_PARA_EXECUCAO_PELO_TECNICO, State.EM_AVALIACAO_PELO_TECNICO_EXECUTANTE);

        _machine.Configure(State.EM_AVALIACAO_PELO_TECNICO_EXECUTANTE)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.SOLICITAR_INFORMACOES_ADICIONAIS_PARA_EXECUCAO, State.AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_DO_TECNICO)
            .Permit(Trigger.APROVAR_PARA_EXECUCAO, State.AGUARDANDO_EXECUCAO);

        _machine.Configure(State.AGUARDANDO_INFORMACOES_EXTRAS_PARA_AVALIACAO_DO_TECNICO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.ENVIAR_PARA_AVALIACAO_DA_EXECUCAO, State.EM_AVALIACAO_PELO_TECNICO_EXECUTANTE);

        _machine.Configure(State.AGUARDANDO_EXECUCAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_EXECUCAO, State.EM_EXECUCAO);

        _machine.Configure(State.EM_EXECUCAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_EXECUCAO, State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR)
            .OnEntryFrom(Trigger.INICIAR_EXECUCAO, (t) => Console.WriteLine(t.Parameters.Length))
            .OnEntry(transition => Console.WriteLine());

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_ANALISE_DA_EXECUCAO_PELO_SOLICITANTE, State.EXECUCAO_EM_ANALISE_PELO_SOLICITANTE);

        _machine.Configure(State.EXECUCAO_EM_ANALISE_PELO_SOLICITANTE)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.APROVAR_PELO_SOLICITANTE, State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR)
            .Permit(Trigger.REPROVAR_EXECUCAO_PELO_SOLICITANTE, State.AGUARDANDO_REVISAO);

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.APROVAR_PELO_ADMINISTRADOR, State.CONCLUIDA)
            .Permit(Trigger.REPROVAR_EXECUCAO_PELO_ADMINISTRADOR, State.AGUARDANDO_REVISAO)
            .Permit(Trigger.DESIGNAR_TECNICO_DE_DOCUMENTACAO, State.AGUARDANDO_CADASTRO_NO_SINDOTEC);

        _machine.Configure(State.AGUARDANDO_CADASTRO_NO_SINDOTEC)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_CADASTRO_SINDOTEC, State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR);

        _machine.Configure(State.AGUARDANDO_REVISAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_REVISAO, State.EM_REVISAO);

        _machine.Configure(State.EM_REVISAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_REVISAO, State.AGUARDANDO_AVALIACAO_DA_EXECUCAO_PELO_ADMINISTRADOR);

    }

    public void OnEnteredThisState(ChangeRequest request)
    {
        Debug.WriteLine($"Request {request.Id}");
    }

    public void EnterThatState(ChangeRequest request, DateTime date)
    {
        // This is how a trigger with parameter is used, the parameter is supplied to the state machine as a parameter to the Fire method.
        _machine.Fire(Trigger.APROVAR_PARA_EXECUCAO, request, date);
    }
}