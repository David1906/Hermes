﻿using System.ComponentModel;

namespace Hermes.Types;

public enum SfcResponseType
{
    // TODO: Translate these
    [Description("Tarea ejecutada con exito.")]
    Ok,

    [Description("El servidor de SFC no responde, por favor contacte al departameto de IT.")]
    Timeout,

    [Description("El flujo de la unidad no corresponde a esta estación.")]
    WrongStation,

    [Description("Error de SFC no identificado.")]
    Unknown,

    [Description("Error al escanear el número de serie.")]
    ScanError
}