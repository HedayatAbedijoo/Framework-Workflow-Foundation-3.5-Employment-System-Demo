namespace WFTools.Services.Persistence.Ado
{
    /// <summary>
    /// Interface used for providing DbCommand text and DbParameter names
    /// to the resource accessor.
    /// </summary>
    public interface IAdoNameResolver
    {
        /// <summary>
        /// Resolve <see cref="CommandNames" /> to their database-specific command text.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="CommandNames" /> indicating which command needs to be resolved.
        /// </param>
        string ResolveCommandName(CommandNames commandName);

        /// <summary>
        /// Resolve <see cref="ParameterNames" /> to their database-specific parameter name.
        /// </summary>
        /// <param name="commandName">
        /// A <see cref="CommandNames" /> indicating which command the parameter
        /// name needs to be resolved for.
        /// </param>
        /// <param name="parameterName">
        /// A <see cref="ParameterNames" /> indicating which parameter needs to be resolved.
        /// </param>
        /// <returns>
        /// </returns>
        string ResolveParameterName(CommandNames commandName, ParameterNames parameterName);
    }
}
