using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed partial class GuiContainer
    {
        private Stack<(uint identity, bool shouldCall)>? _defineClauses;
        private Stack<(uint identity, bool shouldCall)> DefineClauses => _defineClauses ??= new Stack<(uint, bool)>();

        // Always call a matching EndChild() for each BeginChild() call, regardless of its return value [as with Begin:
        // this is due to legacy reason and inconsistent with most BeginXXX functions apart from the regular Begin()
        // which behaves like BeginChild().]

        internal bool PushDefine(bool beginResult, uint identity, bool isSpecialCase = false)
        {
            // if returns true, always call End
            if (beginResult)
            {
                DefineClauses.Push((identity, true));
                return true;
            }

            // otherwise, only call End if it's a special case (Begin and BeginChild)
            DefineClauses.Push((identity, isSpecialCase));
            return false;
        }
        
        internal bool PopDefine(uint identity)
        {
            if (_defineClauses == null)
            {
                Throw($"There is no matching Start call to this End call");
            }

            var (matchedIdentity, shouldCall) = DefineClauses.Pop();
            if (identity != matchedIdentity)
            {
                var name = ElementContextIdentityToName[identity];
                var matchedName = ElementContextIdentityToName[matchedIdentity];
                Throw($"Expected {matchedName}.End but got {name}.End instead");
            }
            return shouldCall;
        }

        internal void AssertAllDefinesAreBalanced()
        {
            if (_defineClauses != null && _defineClauses.Count > 0)
            {
                Throw($"ElementContext.End was not called to match {_defineClauses.Count} Start calls");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void Throw(string message)
        {
            throw new InvalidOperationException(message);
        }
        
    }
}