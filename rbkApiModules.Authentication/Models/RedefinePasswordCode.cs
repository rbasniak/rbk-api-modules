using rbkApiModules.Utilities;
using System;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe para armazenar um código de restauração de senha de um usuário
    /// </summary>
    public class RedefinePasswordCode
    {
        protected RedefinePasswordCode()
        {
        }

        public RedefinePasswordCode(DateTime creationDate)
        {
            CreationDate = creationDate;
            Hash = GenerateHash();
        }

        public virtual DateTime? CreationDate { get; private set; }

        public virtual string Hash { get; private set; }

        private static string GenerateHash()
        {
            string hash = string.Empty;

            for (int i = 0; i < 10; i++)
            {
                hash += Guid.NewGuid().EncodeId();
            }

            return hash;
        }
    }
}
