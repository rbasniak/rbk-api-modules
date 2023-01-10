namespace rbkApiModules.Identity.Core;

public interface IAuthenticationMailService
{
    /// <summary>
    /// Envia o e-mail de confirmação para o novo usuário com o link de confirmação do e-mail cadastrado
    /// </summary>
    void SendConfirmationMail(string receiverName, string receiverEmail, string activationCode);

    /// <summary>
    /// Envia o e-mail de sucesso de registro do usuário
    /// </summary>
    void SendConfirmationSuccessMail(string receiverName, string receiverEmail);

    /// <summary>
    /// Envia o e-mail de redefinição de senha com o link de contendo o código para criação de uma nova senha
    /// </summary>
    void SendPasswordResetMail(string receiverName, string receiverEmail, string resetCode);

    /// <summary>
    /// Envia o e-mail de sucesso ma redefinição de senha
    /// </summary>
    void SendPasswordResetSuccessMail(string receiverName, string receiverEmail);
}