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
    public enum IsoCase
    {
        Case1           = 0,    // no command data, no response data

        Case2Short      = 1,    // no command data, expected response data
        Case3Short      = 2,    // command data, no response data
        Case4Short      = 3,    // command data, expected response data

        Case2Extended   = 4,    // no command data, expected response data
        Case3Extended   = 5,    // command data, no response data
        Case4Extended   = 6     // command data, expected response data
    }
}