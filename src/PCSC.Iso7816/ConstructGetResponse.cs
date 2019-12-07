namespace PCSC.Iso7816
{
    /// <summary>
    /// Creates a GET RESPONSE command after receiving SW1=0x61 (More data available)
    /// </summary>
    /// <param name="initialCommand">The initial command that has been sent to the card</param>
    /// <param name="previousResponse">The received response</param>
    /// <param name="le">The expected size</param>
    /// <returns>A GET RESPONSE APDU</returns>
    public delegate CommandApdu ConstructGetResponse(
        CommandApdu initialCommand,
        ResponseApdu previousResponse,
        int le);
}
