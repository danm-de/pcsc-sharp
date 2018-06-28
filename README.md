# PC/SC wrapper classes for .NET

<!-- toc -->

## Introduction
The _pcsc-sharp_ library is wrapper that provides access to the 
_**P**ersonal **C**omputer/**S**mart **C**ard Resource Manager_
using the system's native PC/SC API. It implements partial ISO7816 
support and is written to run on both Windows and Unix (Mono using
PCSC Lite).

_pcsc-sharp_ **is not** a fully featured library for accessing vendor specific protocols. 
You must implement those protocols / applications yourself. 
For example: You can use _pcsc-sharp_ to access NXP's Mirfare RFID chips, 
but _pcsc-sharp_ does not provide any APDUs to request KEYs, authorize, etc.

You can find PC/SC specific documentation here:
* Windows: [Smart Card Resource Manager API](https://msdn.microsoft.com/en-us/library/windows/desktop/aa380149(v=vs.85).aspx)
* Linux: [PCSC lite project](https://pcsclite.apdu.fr)

## Supported Operating systems
- Windows (winscard.dll) 
  * Windows 10 64-bit Professional (_confirmed_)
  * Windows 10 32-bit Professional
  * Windows 7 64-bit
  * Windows 7 32-bit

- Linux (PC/SC lite)
  * Ubuntu Linux 64-bit (_confirmed_)
  * Ubuntu Linux 32-bit

- MacOS X (_unconfirmed_, _software prerequisites unclear_)

## Quick start

### Establish the resource manager context
Each operation requires a valid context. See [SCardEstablishContext](https://msdn.microsoft.com/en-us/library/windows/desktop/aa379479(v=vs.85).aspx) for more information.

```csharp
var contextFactory = ContextFactory.Instance;
using (var context = contextFactory.Establish(SCardScope.System)) {
   // your code
}
```

Basic rules / best practices:
- One context per smartcard / reader.
- One context per ```SCardMonitor ```.
- The context must be disposed of after usage to free system resources.
- The context must not be disposed of if your application still accesses the smartcard / reader.

### List all connected smartcard readers
See [SCardListReaders](https://msdn.microsoft.com/en-us/library/windows/desktop/aa379793(v=vs.85).aspx).

```csharp
var contextFactory = ContextFactory.Instance;
using (var context = contextFactory.Establish(SCardScope.System)) {
    Console.WriteLine("Currently connected readers: ");
    var readerNames = context.GetReaders();
    foreach (var readerName in readerNames) {
        Console.WriteLine("\t" + readerName);
    }
}
```

### Send ISO7816 APDUs

```csharp
var contextFactory = ContextFactory.Instance;
using (var ctx = contextFactory.Establish(SCardScope.System)) {
    using (var isoReader = new IsoReader(ctx, "ACME Smartcard reader", SCardShareMode.Shared, SCardProtocol.Any, false)) {
        
        var apdu = new CommandApdu(IsoCase.Case2Short, isoReader.ActiveProtocol) {
            CLA = 0x00, // Class
            Instruction = InstructionCode.GetChallenge,
            P1 = 0x00, // Parameter 1
            P2 = 0x00, // Parameter 2
            Le = 0x08 // Expected length of the returned data
        };

        var response = isoReader.Transmit(apdu);
        Console.WriteLine("SW1 SW2 = {0:X2} {1:X2}", response.SW1, response.SW2);
        // ..
    }
}
```

### Read reader attributes

```csharp
using (var ctx = ContextFactory.Instance.Establish(SCardScope.System)) {
    using (var reader = ctx.ConnectReader("OMNIKEY CardMan 5x21 0", SCardShareMode.Shared, SCardProtocol.Any)) {
        var cardAtr = reader.GetAttrib(SCardAttribute.AtrString);
        Console.WriteLine("ATR: {0}", BitConverter.ToString(cardAtr));
        Console.ReadKey();
    }
}
```

### Monitor reader events

```csharp
var monitorFactory = MonitorFactory.Instance;
var monitor = monitorFactory.Create(SCardScope.System);

// connect events here..
monitor.StatusChanged += (sender, args) => 
    Console.WriteLine($"New state: {args.NewState}");

monitor.Start("OMNIKEY CardMan 5x21-CL 0");

Console.ReadKey(); // Press any key to exit

monitor.Cancel();
monitor.Dispose();
```

### More example code

Checkout the [Examples](https://github.com/danm-de/pcsc-sharp/tree/master/Examples) directory.

## Build from source
_pcsc-sharp_ uses the great [FAKE](https://fake.build/) DSL for build tasks 
and the awesome [Paket](https://fsprojects.github.io/Paket/) for NuGet 
package management. To build the solution, simply start the ```build.cmd``` 
on Windows or the ```build``` shell script on Unix. Add ```NuGet``` as command 
line argument to create NuGet packages.

If you want to open the solution with your favorite IDE, restore the NuGet packages first.

On Windows run
```shell
 paket.cmd restore 
```
On Unix/Linux run
```shell
 ./paket restore
```

to download NuGet dependencies before opening the solution in Visual Studio or
MonoDevelop.

