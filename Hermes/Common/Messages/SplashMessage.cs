using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Hermes.Common.Messages;

public class SplashMessage(string text) : ValueChangedMessage<string>(text);