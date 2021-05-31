using System.Collections.Generic;

namespace rbkApiModules.Authentication
{
    public class AuthenticationModuleOptions
    {
        private bool _seedClaims = false;
        private List<string> _authenticationGroups = new();
        private SeedClaimDescriptions _claimDescriptions = new();
        private readonly AuthenticationMailConfiguration _authenticationMailConfiguration;

        public bool SeedClaims => _seedClaims;
        public List<string> AuthenticationGroups => _authenticationGroups;
        public SeedClaimDescriptions ClaimDescriptions => _claimDescriptions;

        public AuthenticationMailConfiguration AuthenticationMailConfiguration => _authenticationMailConfiguration;

        public AuthenticationModuleOptions(AuthenticationMailConfiguration mailConfig)
        {
            _authenticationMailConfiguration = mailConfig;
        }

        public AuthenticationModuleOptions SeedAuthenticationClaims()
        {
            _seedClaims = true;

            return this;
        }

        public AuthenticationModuleOptions UseDefaultClaimDescriptions()
        {
            _claimDescriptions.ManageRoles = "Gerenciar regras de acesso";
            _claimDescriptions.ManageUserRoles = "Atribuir regras de acesso a usuários";
            _claimDescriptions.OverrideUserClaims = "Permitir/bloquear acessos individuais a usuários";

            return this;
        }

        public AuthenticationModuleOptions UseCustomClaimDescriptions(SeedClaimDescriptions descriptions)
        {
            _claimDescriptions = descriptions;

            return this;
        }

        public AuthenticationModuleOptions AddAuthenticationGroup(string group)
        {
            _authenticationGroups.Add(group);

            return this;
        }

        public AuthenticationModuleOptions ConfigureAuthenticationMails(AuthenticationMailConfiguration mailConfig)
        {
            _authenticationMailConfiguration.SMTPHost = mailConfig.SMTPHost;
            _authenticationMailConfiguration.Port = mailConfig.Port;
            _authenticationMailConfiguration.SSL = mailConfig.SSL;
            _authenticationMailConfiguration.SenderName = mailConfig.SenderName;
            _authenticationMailConfiguration.SenderMail = mailConfig.SenderMail;
            _authenticationMailConfiguration.SenderPassword = mailConfig.SenderPassword;
            _authenticationMailConfiguration.MainColor = mailConfig.MainColor;
            _authenticationMailConfiguration.FontColor = mailConfig.FontColor;
            _authenticationMailConfiguration.B64Logo = mailConfig.B64Logo;
            _authenticationMailConfiguration.AccountDetailsUrl = mailConfig.AccountDetailsUrl;
            _authenticationMailConfiguration.PasswordResetUrl = mailConfig.PasswordResetUrl;
            _authenticationMailConfiguration.ConfirmationSuccessUrl = mailConfig.ConfirmationSuccessUrl;
            _authenticationMailConfiguration.ConfirmationFailedUrl = mailConfig.ConfirmationFailedUrl;

            return this;
        }
    }

    public class AuthenticationMailConfiguration
    {
        public string SMTPHost { get; set; }
        public int Port { get; set; }
        public bool SSL { get; set; }
        public string SenderName { get; set; }
        public string SenderMail { get; set; }
        public string SenderPassword { get; set; }
        public string MainColor { get; set; }
        public string FontColor { get; set; }
        public string B64Logo { get; set; }
        public string AccountDetailsUrl { get; set; }
        public string PasswordResetUrl { get; set; }
        public string ConfirmationSuccessUrl { get; set; }
        public string ConfirmationFailedUrl { get; set; }
    }

    public class SeedClaimDescriptions
    {
        public string ManageRoles { get; set; }
        public string ManageUserRoles { get; set; }
        public string OverrideUserClaims { get; set; }
    }
}
