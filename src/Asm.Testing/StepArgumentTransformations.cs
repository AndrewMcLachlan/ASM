using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TechTalk.SpecFlow;

namespace Asm.Testing
{
    [Binding]
    public class StepArgumentTransformations
    {
        public const string NullString = "<NULL>";

        [StepArgumentTransformation(NullString)]
        public string ToNull()
        {
            return null;
        }
    }
}
