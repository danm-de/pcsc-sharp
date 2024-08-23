# Changelog

Version numbering info: a.b.c
a = Major API change
b = New Feature
c = Bug fix

## 7.0.0

2024-08-23  Daniel Mueller <daniel@danm.de>

* Changed target frameworks to: netstandard2.0, netstandard2.1 and net8.0.
* Refactor build process (use Directory.Build.props).
* Updated NuGet dependencies.
* Breaking change in *IsoReader*: [Set DefaultMaxReceiveSize to 255](https://github.com/danm-de/pcsc-sharp/pull/118)
  by [Ben Schoenfeld](https://github.com/bschoenfeld).
* IsoReader.MaxReceiveSize changed property setter from *protected* to *public*.

## 6.2.0

2023-12-30  Daniel Mueller <daniel@danm.de>

* Bugfix ["CPU Spike while Monitoring Devices where not all devices are part of the user's SID"](https://github.com/danm-de/pcsc-sharp/pull/116) reported by @kevmoens
  SCardListReaders vs. SCardGetStatusChange - number of devices is different when using Microsoft Windows Hello for Business 2.
* Add .NET 8.0 support
* Possible breaking change for .NET8 users: [Serializable] has been deprecated
  Exceptions are not longer serializable when using .NET8 runtime.
* Update nuget package dependencies
* Remove FAKE/Paket for being to complicated / superfluous for this library

## 6.1.3

2022-12-19  Daniel Mueller <daniel@danm.de>

* Bugfix: [memory reservation problem when connecting a smart card reader
  with 3 virtual readers.](https://github.com/danm-de/pcsc-sharp/pull/115) by [Stefan Siedler](https://github.com/DadOfTheDead)

## 6.1.2

2022-12-01  Daniel Mueller <daniel@danm.de>

* Bugfix: [fixes exception handling in DeviceMonitor](https://github.com/danm-de/pcsc-sharp/pull/114) by [Stefan Siedler](https://github.com/DadOfTheDead)

## 6.1.1

2022-12-01  Daniel Mueller <daniel@danm.de>

* Bugfix: fix typos (wrong enum value InvalidHandle->InvalidHandleWindows)
* Adopted suggestion from Stefan Siedler: Remove counters in DeviceMonitor.
  This is necessary to prevent a NoServiceException when the user continuously
  connects and disconnects USB readers.

## 6.1.0

2022-11-30  Daniel Mueller <daniel@danm.de>

* Added suggestion from Stefan Siedler (Fresenius Medial Care): problem on
  Windows 10: during USB reader disconnect, DeviceMonitor will throw ShutDown
  & InvalidHandleWindows instead of NoService error code.
* Multi-target Nuget package with support for .netstandard2.0, .net6 and .net7
* Try to fix [P/Invoke call in MacOsxNativeMethods.GetSymFromLib(string)](https://github.com/danm-de/pcsc-sharp/pull/110)
  Thanks to @pazimor and Dmitry Kalinin for reporting.

## 6.0.0

2022-03-09  Daniel Mueller <daniel@danm.de>

* PCSC: change build targets to netstandard2.0
* Test / Example projects: update target framework to net6.0
* Update NuGet packages: System.Reactive 5.0.0

## 5.1.0

2021-12-06  Daniel Mueller <daniel@danm.de>

* PCSC: new instruction for INC and DEC
* [Mifare examples updated](https://github.com/danm-de/pcsc-sharp/pull/108) by @alexander-ts

## 5.0.0

2019-12-07  Daniel Mueller <daniel@danm.de>

* Update NuGet packages: all test projects (NUnit, FakeItEasy, ...)
* Update NuGet packages: System.Reactive >= 4.3.0
* Added new build targets: netstandard2.1, netcoreapp3.0, netcoreapp3.1,
  net472, net48
* PCSC.Iso7816: BREAKING CHANGE: if the card signals "more data available"
  (GET RESPONSE required), it copies the CLA byte from the previous command.
* PCSC.Iso7816: new method Transmit(CommandApdu, ConstructGetResponse)
  Gives control on how to create a GET RESPONSE command.
* [Bug fix via pull request](https://github.com/danm-de/pcsc-sharp/issues/97)
  Thanks to @gjermund-stensrud for reporting

## 4.2.0

2019-07-03  Daniel Mueller <daniel@danm.de>

* IsoReader: added new constructor to supply MaxReceiveSize for GET_RESPONSE
  command. Per default it has a value of 128 bytes only. For a larger (Le)
  size the user had to inherit from IsoReader and override the Property
  MaxReceiveSize. Thanks to David Mohr for reporting.

## 4.1.1

2019-05-26  Daniel Mueller <daniel@danm.de>

* Added SourceLink supported

## 4.1.0

2019-05-26  Daniel Mueller <daniel@danm.de>

* [Fixes bug](https://github.com/danm-de/pcsc-sharp/issues/87)
  Thanks to Jesse de Wit @JssDWt and @melihercan for reporting
* [Fixes bug](https://github.com/danm-de/pcsc-sharp/issues/90)
* [Fixes bug](https://github.com/danm-de/pcsc-sharp/pull/88)
  Thanks to Alexander Gräf @graealex

## 4.0.3

2019-02-18  Daniel Mueller <daniel@danm.de>

* [Fixes bug](https://github.com/danm-de/pcsc-sharp/issues/86)
  Thanks to Jesse de Wit @JssDWt for reporting

## 4.0.2

2019-02-04  Daniel Mueller <daniel@danm.de>

* [Added supported for Mac OS](https://github.com/danm-de/pcsc-sharp/pull/85) by Pedro Marinho Rodrigues Pinto
  Thanks to @pedromrpinto

## 4.0.1

2018-09-24  Daniel Mueller <daniel@danm.de>

* [Fix converting IntPtr to enum on .NET Core](https://github.com/danm-de/pcsc-sharp/pull/78)
  Thanks to @BtbN

## 4.0.0

2018-02-25  Daniel Mueller <daniel@danm.de>

* [PCSC, PCSC.Reactive converted to .netstandard 2.0](https://github.com/danm-de/pcsc-sharp/pull/67)
  [Thanks to Christoph Fink @chrfin](https://github.com/danm-de/pcsc-sharp/issues/32)
* [ISCardContext.ReEstablish() removed; Connect(..) and ConnectReader(..) added](https://github.com/danm-de/pcsc-sharp/issues/58)
* new types: ICardHandle, CardHandle, ICardReader, CardReader
* MonitorFactory: removed obsolete methods
* ISO7816 moved to PCSC.Iso7816 (separate assemlby / nuget)
* IIsoReader: removed properties CurrentContext and Reader

## 3.8.0

2017-10-08  Daniel Mueller <daniel@danm.de>

* [Merged pull request](https://github.com/danm-de/pcsc-sharp/pull/55)
  Corrected tests (32/64 bit issue), code cleanup.
  Thanks to @glenebob
* [Feature request](https://github.com/danm-de/pcsc-sharp/pull/57)
  New constructor for Iso7816.Response opened by Scott Stephens
  (Thanks for the pull request @scottstephens)

## 3.7.0

2017-06-22  Daniel Mueller <daniel@danm.de>

* [Improvement: MacOS X support](https://github.com/danm-de/pcsc-sharp/pull/52/) added by Glen Parker
  Thanks to @glenebob

## 3.6.1

2017-03-28  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/44) ISCardReader.Status(..) returns wrong state value on Windows. Thanks to Mark @mkeldridge

## 3.6.0

2016-11-16  Daniel Mueller <daniel@danm.de>

* Update build & test toolset (NUnit,Fake,..)
* Update System.Reactive (.netstandard)
* [Added DeviceMonitor & DeviceMonitorFactory as requested in #31](https://github.com/danm-de/pcsc-sharp/issues/31)
  Thanks to @olegsavelos for providing valid sample code in
* IMonitorFactory: marked .Start(..) as obsolete (it encouraged
  users doing bad things - it's a factory anyway, not a Starter)
* pcsc-sharp-rx: corrected observable creation (removed .Replay)

## 3.5.3

2016-11-03  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/37)
  Dispose() or finalizer should not throw if SCardContext.Release()
  returns ERROR_INVALID_HANDLE.
  Thanks to @SGN-JSE

## 3.5.2

2016-11-02  Daniel Mueller <daniel@danm.de>

* [Pull request](https://github.com/danm-de/pcsc-sharp/pull/36)
  Added platform check for Windows CE (tested by Erik Kralj)
  Thanks to Erik Kralj

## 3.5.1

2016-05-24  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/28)
  Thanks to Morne van der Westhuizen

## 3.5.0

2016-04-19  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/25)
  Thanks to Paul McCay!
* New nuget package for PCSC.Reactive
  Adds reactive extensions (observables) to monitor smard card events.
* API change: Factories for SCardContext and SCardMonitor
  (IContextFactory and IMonitorFactory)
* SCardMonitor new constructor for context factory usage.

## 3.4.1

2016-02-29  Daniel Mueller <daniel@danm.de>

* [Better error handling for insufficient buffers](https://github.com/danm-de/pcsc-sharp/issues/20)

## 3.4.0.0

2016-02-11  Daniel Mueller <daniel@danm.de>

* Added better error handling in IsoReader.

## 3.3.1.0

2016-01-21  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/19)
  Thanks to @andrew7webb!
* Removed public IsNullOrWhiteSpace extension method for string.
* Made InvalidApduException public.
* Code cleanups.

## 3.3.0.0

2015-10-28  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/11)
  Thanks to @EmptySamurai!
* [Bugfix](https://github.com/danm-de/pcsc-sharp/issues/15)
  Thanks to @anzun!
* WARNING: API change:
  SCardContext.GetReaders(), SCardContext.GetReaders(string[]) and
  SCardContextGetReaderGroups() do not throw PCSCException anymore on
  SCARD_E_NO_READERS_AVAILABLE (Group contains no readers)
* SCardContext and SCardMonitor throw more specific exceptions
* Exceptions are serializable

## 3.2.0.0

2015-07-31  Daniel Mueller <daniel@danm.de>

* [Bugfix](https://github.com/danm-de/pcsc-sharp/pull/14)
  Thanks to [Mat](https://github.com/mtausig)!
* Raised .Net framework to version 4.0.0
* Introduction "paket" and FAKE as NuGet and psake replacements
* New folder structure for solution / project files.

## 3.1.0.4

2015-03-17  Daniel Mueller <daniel@danm.de>

* Refactored SCardMonitor to be "more" thread safe
* [Bugfix](https://github.com/danm-de/pcsc-sharp/pull/7)
  Thanks to [@EmptySamurai](https://github.com/EmptySamurai)!

## 3.1.0.3

2013-11-29  Daniel Mueller <daniel@danm.de>

* CommandApdu: Instruction property -> added unchecked statement
* New example: Mifare1kTest (LoadKey, Auth, Read & Update Binary)

## 3.1.0.2

2013-10-23  Daniel Mueller <daniel@danm.de>

* Bugfix: Added new internal authenticate instruction code. (Michael Kuenzli)

## 3.1.0.1

2013-05-09  Daniel Mueller <daniel@danm.de>

* Decreased .Net Framework version from 4.0 to .Net 3.5.

## 3.0.0.0

2013-05-09  Daniel Mueller <daniel@danm.de>

* WARNING: major API changes!
* Changed license to BSD 2-Clause
* Update to .Net Framework 4.0 (die Windows XP, die)
* Removed Iso8825 completely. I hope nobody besides me was using it :-)
* Code cleanup (using the awesome [Resharper](http://www.jetbrains.com/resharper/)
* SCardContext: implemented the correct IDisposable pattern.
  GC will release the context during finalize! If you access/use the
  SCardContext.Handle in your own managed/unmanaged libraries calls make
  sure that you keep a reference to the context instance.
  Changed the static properties "Infinite" and "MaxAtrSize" to instance
  properties.
* New interface ISCardContext to ease unit testing.
* SCardReader: implemented the correct IDisposable pattern.
  Changed type of property "CurrentContext" to ISCardContext.
  Changed constructor to accept ISCardContext instead of SCardContext.
  Removed static property "Infinite" (duplicate of ISCardContext.Infinite)
* SCardMonitor: implemented the correct IDisposable pattern.
  Constructor got new parameter "releaseContextOnDispose".
  Changed constructor to accept ISCardContext instead of SCardContext.
* New interface ISCardMonitor to ease unit testing.
* Apdu: Apdu.Case and Apdu.Protocol are not longer virtual, removed
  protected variables proto and isocase.
* CommandApdu: removed protected variables cla, ins, p1, p2, lc, le
  and data (use property setter!).
* IsoCard: constructor got new parameter "disconnectReaderOnDispose".
* SCardReaderState: implemented the correct IDisposable pattern.
  Renamed property ATR to Atr.
* CardStatusEventArgs & StatusChangeEventArgs: renamed property ATR to Atr.
  Both inherit from CardEventArgs.
* SCardAttr: renamed to SCardAttribute
* SCardAttribute: Renamed various enum member names (replaced C-style
  abbreviation with full name).
* SCardClass: Renamed various enum member names (replaced C-style
  abbreviation with full name).
* SCardError: Renamed various enum member names (replaced C-style
  abbreviation with full name).
* ResponseApduList: Renamed to ResponseApduEnumerator.
* Response: Removed property ResponseApduList.
* ResponseApdu: Fixed a bug in ResponseApdu.ToArray().
  Removed protected variables response and length.
* IsoCard: renamed to IsoReader (which is an appropriate name)
* New interface IIsoReader
* InstructionByte: removed protected variable "ins". Use the property
  setter instead.
* RecordInfo: renamed const FILE_STRUCTURE_IS_MASK to FILE_STRUCTURING_MASK

## 2.0.0.4

2012-05-06  Daniel Mueller <daniel@danm.de>

* Bugfix in ResponseADPU.SW2. Thanks to Guillaume M for reporting.

## 2.0.0.2

2011-01-23  Daniel Mueller <daniel@danm.de>

* Changed SCardContext.cs: -> unchecked((IntPtr)(Int32)0xFFFFFFFF); to avoid OverflowException in Windows 7 (32bit).

## 2.0.0.1

2011-01-19  Daniel Mueller <daniel@danm.de>

* Added unchecked() to correct long/IntPtr type castings. Thanks to Miah Cottrell for reporting!

## 2.0.0.0

2011-01-16  Daniel Mueller <daniel@danm.de>

* Tested with Windows 7 64bit edition
* Tested with Ubuntu 10.10 64bit edition
* Bugfix, thanks to Dariusz Marzoch for reporting!

## pre 2.0.0.0

2011-01-12  Daniel Mueller <daniel@danm.de>

* New major version 2.0.0.0 and GUID because of API change.
* Changed 'UIntPtr' to 'IntPtr', 'UInt32' to 'Int32'.
* Changed value types of various enums from 'long' to 'int'.
* Tested with Windows 7 64bit Professional. Linux tests needed.

2010-11-10  Daniel Mueller <daniel@danm.de>

* Corrected string termination for SCardConnect() calls.
  Thanks to Dr. Ludovic Rousseau!
* Removed IsValid()-checks from some context specific methods.
* PCSCliteAPI.cs: changed pszReader in SCARD_READERSTATE to IntPtr
  after getting various segmentation faults because of Mono's strange
  IntPtr/UIntPtr conversion. What Am I doing wrong?? ;-(

2010-11-05  Daniel Mueller <daniel@danm.de>

* Added new classes for ISO8825 / not ready to use!

2010-11-01  Daniel Mueller <daniel@danm.de>

* Added a few new classes for ISO7816 and ISO8825
  WARNING: Iso7816/Iso8825 is experimental / not ready to use!

2010-10-18  Daniel Mueller <daniel@danm.de>

* CommandApdu.cs: ExpectedResponseLength -> fix: returned size with two
  additional bytes for SW1 and SW2.
* ResponseApdu.cs: Removed locking.
* Added new class Response.cs

2010-10-13  Daniel Mueller <daniel@danm.de>

* Camal Case renaming of Apdu, CommandApdu and various methods.
* First implementation of ResponseApdu
* Added a few new Transmit() methods in ISCardReader and SCardReader

2010-10-12  Daniel Mueller <daniel@danm.de>

* Formated the source code with Visual Studio

2010-10-11  Daniel Mueller <daniel@danm.de>

* Changed the SCardReader.Transmit method, some
  parameters do not need the 'ref' statement.
* Added documentation and examples.

2010-10-08  Daniel Mueller <daniel@danm.de>

* Added two more SetAttrib methods in SCardReader.
* Added a lot more documentation.

2010-10-07  Daniel Mueller <daniel@danm.de>

* Added a public member 'Handle' in SCardContext.
* Added a lot more documentation.

2010-10-06  Daniel Mueller <daniel@danm.de>

* Added new/missing members in SCardError.
* Added a lot more documentation.

2010-10-05  Daniel Mueller <daniel@danm.de>

* Corrected spelling in PCSC.SCRState
  Anavailable -> Unavailable
* Added more documentation.
* Corrected ISCardReader

2010-09-30  Daniel Mueller <daniel@danm.de>

* Try to fix the "library not found" issue on OSX.
* Added some more ISO7816 classes (CommandAPDU).

2010-01-24  Daniel Mueller <daniel@danm.de>

* Initial release pcsc-sharp

2007-06-04  Daniel Mueller <daniel@danm.de>

* Initial release Mono-PCSClite
