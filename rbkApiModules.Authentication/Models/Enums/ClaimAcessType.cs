using System.ComponentModel;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Tipos de acesso quando uma claim é adicionada diretamente a um usuário
    /// </summary>
    public enum ClaimAccessType
    {
        [Description("Permitir")]
        Allow = 1,

        [Description("Bloquear")]
        Block = 0
    }
}
