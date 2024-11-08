using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Hermes.Common.Messages;

public class SplashClosedMessage() : ValueChangedMessage<bool>(true);