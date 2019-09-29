﻿using System;
using System.Collections.Generic;

namespace CodeAnalyzer.Interface
{
    public interface IProcessingModule
    {
        ICommonResults Execute(IEnumerable<IFileSource> fileSources);
        int? GetMaxMainProgressValue();

        event Action<ProcessingModuleEventData> OnProgress;
    }
}