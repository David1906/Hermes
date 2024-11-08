using CommunityToolkit.Mvvm.Messaging.Messages;
using System;

namespace Hermes.Common.Messages;

public class NavigateMessage(Type type) : ValueChangedMessage<Type>(type);