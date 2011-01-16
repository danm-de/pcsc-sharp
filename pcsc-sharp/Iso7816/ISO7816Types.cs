/*
Copyright (C) 2010
    Daniel Mueller <daniel@danm.de>

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

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

    public enum IsoCase : int
    {
        Case1           = 0,    // no command data, no response data

        Case2Short      = 1,    // no command data, expected response data
        Case3Short      = 2,    // command data, no response data
        Case4Short      = 3,    // command data, expected response data

        Case2Extended   = 4,    // no command data, expected response data
        Case3Extended   = 5,    // command data, no response data
        Case4Extended   = 6     // command data, expected response data
    }

    public enum SW1Code: byte
    {
        NormalDataResponse          = 0x61,
        WarningNVDataNotChanged     = 0x62,
        WarningNVDataChanged        = 0x63,
        ErrorNVDataNotChanged       = 0x64,
        ErrorNVDataChanged          = 0x65,
        ErrorSecurity               = 0x66,
        ErrorLengthIncorrect        = 0x67,
        ErrorFunctionNotSupported   = 0x68,
        ErrorCmdNotAllowed          = 0x69,
        ErrorP1P2Incorrect          = 0x6A,
        ErrorParameterIncorrect     = 0x6B,
        ErrorP3Incorrect            = 0x6C,
        ErrorInsNotSupported        = 0x6D,
        ErrorClassNotSupported      = 0x6E,
        ErrorNoPreciseDiagnostic    = 0x6F,
        
        Normal                      = 0x90,

        ErrorPurseBalance           = 0x91,
        ErrorMemory                 = 0x92,
        ErrorFile                   = 0x94,
        ErrorAuthorization          = 0x98,
        NormalGetResponse           = 0x9F
    }

    public enum InstructionCode: byte
    {
        [DescriptionAttribute("ERASE BINARY")]
        EraseBinary = 0x0E,
        [DescriptionAttribute("VERIFY")]
        Verify = 0x20,
        [DescriptionAttribute("MANAGE CHANNEL")]
        ManageChannel = 0x70,
        [DescriptionAttribute("EXTERNAL AUTHENTICATE")]
        ExternalAuthenticate = 0x82,
        [DescriptionAttribute("GET CHALLENGE")]
        GetChallenge = 0x84,
        [DescriptionAttribute("INTERNAL AUTHENTICATE")]
        InternalAuthenticate = 0x88,
        [DescriptionAttribute("SELECT FILE")]
        SelectFile = 0xA4,
        [DescriptionAttribute("READ BINARY")]
        ReadBinary = 0xB0,
        [DescriptionAttribute("READ RECORD(S)")]
        ReadRecord = 0xB2,
        [DescriptionAttribute("GET RESPONSE")]
        GetResponse = 0xC0,
        [DescriptionAttribute("ENVELOPE")]
        Envelope = 0xC2,
        [DescriptionAttribute("GET DATA")]
        GetData = 0xCA,
        [DescriptionAttribute("WRITE BINARY")]
        WriteBinary = 0xD0,
        [DescriptionAttribute("WRITE RECORD")]
        WriteRecord = 0xD2,
        [DescriptionAttribute("UPDATE BINARY")]
        UpdateBinary = 0xD6,
        [DescriptionAttribute("PUT DATA")]
        PutData = 0xDA,
        [DescriptionAttribute("UPDATE DATA")]
        UpdateData = 0xDC,
        [DescriptionAttribute("APPEND RECORD")]
        AppendRecord = 0xE2
    }

    public enum FileType
    {
        Dedicated,
        Elementary,
    }

    public enum ExtendedFileType
    {
        WorkingElementary,
        InternalElementary,
        Proprietary,
        Dedicated
    }

    public enum FileStructureType: byte
    {
        NoInformation               = 0x0,
        Transparent                 = 0x1,
        LinearFixed                 = 0x2,
        LinearFixedSimpleTlv        = 0x3,
        LinearVariable              = 0x4,
        LinearVariableSimpleTlv     = 0x5,
        Cyclic                      = 0x6,
        CyclicSimpleTlv             = 0x7
    }
    public enum FileShareMode : byte
    {
        Shareable                   = 0x40,
        NotShareable                = 0x0
    }

    public enum ClaHighPart : byte
    {
        Iso0x           = 0x00,
        Rfu1x           = 0x10,
        Rfu2x           = 0x20,
        Rfu3x           = 0x30,
        Rfu4x           = 0x40,
        Rfu5x           = 0x50,
        Rfu6x           = 0x60,
        Rfu7x           = 0x70,
        Iso8x           = 0x80,
        Iso9x           = 0x90,
        IsoAx           = 0xA0,
        IsoBx           = 0xB0,
        IsoCx           = 0xC0,
        ProprietaryDx   = 0xD0,
        ProprietaryEx   = 0xE0,
        ProprietaryFx   = 0xF0
    }
    public enum SecureMessagingFormat : byte
    { 
        None                        = 0x0,
        Proprietary                 = 0x4,
        CmdHeaderNotAuthenticated   = 0x8,
        CmdHeaderAuthenticated      = 0xC
    }

    public enum TlvDataType
    {
        Simple,
        Ber
    }
  
    /*
    public enum FileIdentifer : long
    {
        MasterFile      = 0x3F00
    }
    */
}