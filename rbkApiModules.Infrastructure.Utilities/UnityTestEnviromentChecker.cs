namespace rbkApiModules.Infrastructure.Utilities
{
    /// <summary>
    /// Classe contendo um indicador para mostrar se o código está sendo executado no ambiente de testes
    /// </summary>
    public static class UnityTestEnviromentChecker
    {
        /// <summary>
        /// Indicador de que o código está sendo executado no ambiente de testes
        /// </summary>
        public static bool OnTestEnviroment { get; set; }
    }
}
