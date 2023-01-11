using Stateless;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

public class ApplicationClaims
{
    public const string WORKFLOW_ADMIN = "WORKFLOW_ADMIN";
    public const string TECHNICIAN = "TECHNICIAN";
}

public class ChangeRequest
{
    public ChangeRequestWorkflow.State State { get; set; }
    public string Id { get; set; }
}


public class ChangeRequestWorkflow
{
    public readonly StateMachine<State, Trigger> _machine;

    public enum Trigger
    {
        CRIAR_ATRAVES_DO_REQUISITANTE,
        CRIAR_ATRAVES_DO_ADMINISTRADOR,
        ENVIAR_PARA_AVALIACAO,
        CANCELAR,
        AVALIAR_SOLICITACAO_PELO_ADMINISTRADOR,
        SOLICITAR_INFORMACOES_ADICIONAIS,
        APROVAR_AVALIACAO_FINAL,
        AVALIAR_PELO_TECNICO,
        APROVAR_PARA_EXECUCAO,
        INICIAR_EXECUCAO,
        CONCLUIR_EXECUCAO,
        INICIAR_ANALISE_DA_EXECUCAO_PELO_SOLICITANTE,
        REPROVAR_PELO_SOLICITANTE,
        APROVAR_PELO_SOLICITANTE,
        INICIAR_ANALISE_DA_EXECUCAO_PELO_ADMINISTRADOR,
        REPROVAR_PELO_ADMINISTRADOR,
        APROVAR_PELO_ADMINISTRADOR,
        DESIGNAR_TECNICO_DE_DOCUMENTACAO,
        INICIAR_REVISAO,
        CONCLUIR_CADASTRO_SINDOTEC,
        CONCLUIR_REVISAO,
        INICIAR_ANALISE_DA_EXECUCAO_PELO_TECNICO
    }

    public enum State
    {
        CRIADA_NO_SISTEMA_PELO_REQUISITANTE,
        CRIADA_NO_SISTEMA_PELO_ADMINISTRADOR,
        RASCUNHO,
        AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR,
        EM_AVALIACAO_PELO_ADMINISTRADOR,
        AGUARDANDO_INFORMACOES_EXTRAS_DA_AVALIACAO_INICIAL_SOLICITADAS_PELO_ADMINISTRADOR,
        AGUARDANDO_AVALIACAO_DO_TECNICO,
        EM_AVALIACAO_PELO_TECNICO,
        AGUARDANDO_EXECUCAO,
        AguardandoInformacoesExtrasDaAvaliacaoInicialSolicitadasPeloTecnico,
        EM_EXECUCAO,
        AGUARDANDO_ANALISE_DA_EXECUCAO_PELO_SOLICITANTE,
        EmAnaliseDaExecucaoPeloSolicitante,
        EmAnaliseDaExecucaoPeloAdministrador,
        AguardandoCadastroNoSindotec,
        AguardandoRevisao,
        EmRevisao,
        CANCELADA,
        AguardandoAvaliacaoDaExecucaoPeloAdministrador,
        Concluded,
    }

    public ChangeRequestWorkflow()
    {
        _machine = new StateMachine<State, Trigger>(State.RASCUNHO);

        _machine.Configure(State.RASCUNHO)
            // .Permit(Trigger.ENVIAR_PARA_AVALIACAO, State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR)
            //.PermitIf<ChangeRequest, DateTime>(Trigger.ENVIAR_PARA_AVALIACAO, State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR, (changeRequest, date) =>
            //{
            //    Debug.WriteLine($"Validating {changeRequest.Id} at {date.ToShortDateString()}");

            //    return changeRequest.Id == "019";
            //})
            .PermitIf(Trigger.APROVAR_PARA_EXECUCAO, State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR, new NamedGuard(String.Empty, (args) =>
            {
                var changeRequest = (ChangeRequest)args[0];
                var date = (DateTime)args[1];

                Debug.WriteLine($"Validating {changeRequest.Id} at {date}");

                return changeRequest.Id == "019";
            }))
            .OnDeactivate(() => Debug.WriteLine($"State {State.RASCUNHO} deactivated"))
            .OnExit(transition => Debug.WriteLine($"Exited from {transition.Source} and entered {transition.Destination} by firing {transition.Trigger}"));

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .OnEntry(transition => Debug.WriteLine($"Entered {transition.Destination} from {transition.Source} by firing {transition.Trigger}"))
            // .OnEntryFrom(Trigger.ENVIAR_PARA_AVALIACAO, (transition) => Debug.WriteLine($"Entered {transition.Destination} from {transition.Source} by firing ONLY {transition.Trigger}"))
            .OnActivate(() => Debug.WriteLine($"State {State.AGUARDANDO_AVALIACAO_DO_ADMINISTRADOR} activated"))
            // .OnEntryFrom(_machine.SetTriggerParameters<ChangeRequest>(Trigger.ENVIAR_PARA_AVALIACAO), OnEnteredThisState)

            .Permit(Trigger.SOLICITAR_INFORMACOES_ADICIONAIS, State.AGUARDANDO_INFORMACOES_EXTRAS_DA_AVALIACAO_INICIAL_SOLICITADAS_PELO_ADMINISTRADOR)
            .Permit(Trigger.APROVAR_PELO_ADMINISTRADOR, State.AGUARDANDO_AVALIACAO_DO_TECNICO);

        _machine.Configure(State.AGUARDANDO_INFORMACOES_EXTRAS_DA_AVALIACAO_INICIAL_SOLICITADAS_PELO_ADMINISTRADOR)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.ENVIAR_PARA_AVALIACAO, State.EM_AVALIACAO_PELO_ADMINISTRADOR);

        _machine.Configure(State.AGUARDANDO_AVALIACAO_DO_TECNICO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_ANALISE_DA_EXECUCAO_PELO_TECNICO, State.EM_AVALIACAO_PELO_TECNICO);

        _machine.Configure(State.EM_AVALIACAO_PELO_TECNICO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.SOLICITAR_INFORMACOES_ADICIONAIS, State.AguardandoInformacoesExtrasDaAvaliacaoInicialSolicitadasPeloTecnico)
            .Permit(Trigger.APROVAR_PARA_EXECUCAO, State.AGUARDANDO_EXECUCAO);

        _machine.Configure(State.AguardandoInformacoesExtrasDaAvaliacaoInicialSolicitadasPeloTecnico)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.ENVIAR_PARA_AVALIACAO, State.EM_AVALIACAO_PELO_TECNICO);

        _machine.Configure(State.AGUARDANDO_EXECUCAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_EXECUCAO, State.EM_EXECUCAO);

        _machine.Configure(State.EM_EXECUCAO)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_EXECUCAO, State.AguardandoAvaliacaoDaExecucaoPeloAdministrador)
            .OnEntryFrom(Trigger.INICIAR_EXECUCAO, (args) => Console.WriteLine(args.Length))
            .OnEntry(transition => Console.WriteLine());

        _machine.Configure(State.AguardandoAvaliacaoDaExecucaoPeloAdministrador)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_ANALISE_DA_EXECUCAO_PELO_SOLICITANTE, State.EmAnaliseDaExecucaoPeloSolicitante);

        _machine.Configure(State.EmAnaliseDaExecucaoPeloSolicitante)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.APROVAR_PELO_SOLICITANTE, State.AguardandoAvaliacaoDaExecucaoPeloAdministrador)
            .Permit(Trigger.REPROVAR_PELO_SOLICITANTE, State.AguardandoRevisao);

        _machine.Configure(State.AguardandoAvaliacaoDaExecucaoPeloAdministrador)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.APROVAR_PELO_ADMINISTRADOR, State.Concluded)
            .Permit(Trigger.REPROVAR_PELO_ADMINISTRADOR, State.AguardandoRevisao)
            .Permit(Trigger.DESIGNAR_TECNICO_DE_DOCUMENTACAO, State.AguardandoCadastroNoSindotec);

        _machine.Configure(State.AguardandoCadastroNoSindotec)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_CADASTRO_SINDOTEC, State.AguardandoAvaliacaoDaExecucaoPeloAdministrador);

        _machine.Configure(State.AguardandoRevisao)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.INICIAR_REVISAO, State.EmRevisao);

        _machine.Configure(State.EmRevisao)
            .Permit(Trigger.CANCELAR, State.CANCELADA)
            .Permit(Trigger.CONCLUIR_REVISAO, State.AguardandoAvaliacaoDaExecucaoPeloAdministrador);

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