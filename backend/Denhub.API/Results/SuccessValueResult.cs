﻿using System.Collections.Generic;

namespace Denhub.API.Results {
    public class SuccessValueResult<T> : ValueResult<T> {
        public override ResultType Type => ResultType.Ok;
        public override List<string> Errors => new();
        public override T Value { get; }

        public SuccessValueResult(T value) {
            Value = value;
        }
    }
}