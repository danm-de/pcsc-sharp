namespace PCSC.Iso7816
{
    /*****************************************************
     * Case     * Command data  * Expected response data *
     *****************************************************
     * 1        * No data       * No data                *
     * 2        * No data       * Data                   *
     * 3        * Data          * No data                *
     * 4        * Data          * Data                   *
     *****************************************************/

    /// <summary>APDU message structure for the command-response pair.</summary>
    /// <remarks>
    ///     <list type="table">
    ///         <listheader><term>ISO case</term><description>Command data, Expected response data</description></listheader>
    ///         <item><term>1</term><description>Command: no data, Expected response: no data</description></item>
    ///         <item><term>2</term><description>Command: no data, Expected response:  data</description></item>
    ///         <item><term>3</term><description>Command: data, Expected response: no data</description></item>
    ///         <item><term>4</term><description>Command: data, Expected response: data</description></item>
    ///     </list>
    ///     <para>In the card capabilities, the card states that the Lc and the Le field should either be short or extended. A short command has one byte for each length field, an extended command has two bytes for each length field.</para>
    /// </remarks>
    public enum IsoCase
    {
        /// <summary>No command data. No response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued to 0.</description></item>
        ///         <item><description>Le is valued to 0.</description></item>
        ///         <item><description>No data byte is present.</description></item>
        ///     </list>
        /// </remarks>
        Case1 = 0,

        /// <summary>No command data. Expected response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued to 0.</description></item>
        ///         <item><description>Le is valued from 1 to 256.</description></item>
        ///         <item><description>No data byte is present.</description></item>
        ///     </list>
        /// </remarks>
        Case2Short = 1,

        /// <summary>Command data. No response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued from 1 to 255.</description></item>
        ///         <item><description>Le is valued to 0.</description></item>
        ///         <item><description>Lc data bytes are present.</description></item>
        ///     </list>
        /// </remarks>
        Case3Short = 2,

        /// <summary>Command data. Expected response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued from 1 to 255.</description></item>
        ///         <item><description>Le is valued from 1 to 256.</description></item>
        ///         <item><description>Lc data bytes are present.</description></item>
        ///     </list>
        /// </remarks>
        Case4Short = 3,

        /// <summary>No command data. Expected response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued to 0.</description></item>
        ///         <item><description>Le is valued from 1 to 65536.</description></item>
        ///         <item><description>No data byte is present.</description></item>
        ///     </list>
        /// </remarks>
        Case2Extended = 4,

        /// <summary>Command data. No response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued from 1 to 65536.</description></item>
        ///         <item><description>Le is valued to 0.</description></item>
        ///         <item><description>Lc data bytes are present.</description></item>
        ///     </list>
        /// </remarks>
        Case3Extended = 5,

        /// <summary>Command data. Expected response data.</summary>
        /// <remarks>
        ///     <list type="bullet">
        ///         <item><description>Lc is valued from 1 to 65535.</description></item>
        ///         <item><description>Le is valued from 1 to 65536.</description></item>
        ///         <item><description>Lc data bytes are present.</description></item>
        ///     </list>
        /// </remarks>
        Case4Extended = 6
    }
}