namespace PCSC
{
	internal static class SCardErrorExtensionMethods
	{
		internal static void ThrowIfNotSuccess(this SCardError sc) {
			if (sc == SCardError.Success) {
				return;
			}

			sc.Throw();
		}

		internal static void Throw(this SCardError sc) {
			switch (sc) {
				case SCardError.Success:
					throw new SuccessException(sc);
				case SCardError.InvalidHandle:
					throw new InvalidContextException(sc);
				case SCardError.InvalidParameter:
					throw new InvalidProtocolException(sc);
				case SCardError.InvalidValue:
					throw new InvalidValueException(sc);
				case SCardError.NoService:
					throw new NoServiceException(sc);
				case SCardError.NoSmartcard:
					throw new NoSmartcardException(sc);
				case SCardError.NoReadersAvailable:
					throw new NoReadersAvailableException(sc);
				case SCardError.NotReady:
					throw new NotReadyException(sc);
				case SCardError.ReaderUnavailable:
					throw new ReaderUnavailableException(sc);
				case SCardError.SharingViolation:
					throw new SharingViolationException(sc);
				case SCardError.UnknownReader:
					throw new UnknownReaderException(sc);
				case SCardError.UnsupportedCard:
					throw new UnsupportedFeatureException(sc);
				case SCardError.CommunicationError:
					throw new CommunicationErrorException(sc);
				case SCardError.InternalError:
					throw new InternalErrorException(sc);
				case SCardError.UnpoweredCard:
					throw new UnpoweredCardException(sc);
				case SCardError.UnresponsiveCard:
					throw new UnresponsiveCardException(sc);
				case SCardError.RemovedCard:
					throw new RemovedCardException(sc);
				case SCardError.InsufficientBuffer:
					throw new InsufficientBufferException(sc);
                case SCardError.WinErrorInsufficientBuffer:
                    throw new WinErrorInsufficientBufferException(sc);
                default:
					throw new PCSCException(sc); // Unexpected / unknown error
			}
		}
	}
}